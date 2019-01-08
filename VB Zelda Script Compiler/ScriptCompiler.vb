Imports Demo
Imports Irony.Parsing
Imports SPASM

Public Class ZCRCompiler
    Private _ZeldaPath As String
    Private Assembler As IZ80Assembler

    Public Sub New(ZeldaPath As String)
        Me._ZeldaPath = ZeldaPath
        Assembler = New Z80Assembler With {
            .CurrentDirectory = ZeldaPath
        }
        Assembler.Assemble("#include ""scriptengine-new.asm""")
    End Sub

    Public Shared Sub Main()
        Dim Compiler As New ZCRCompiler("C:\Users\sputt_000\Projects\zelda")
        Compiler.Compile("HILL_MAP_05_CLOSE_DOORS", "
	delete object[2];
 ")
    End Sub

    Private Function SelectKeyStatement(Node As ParseTreeNode) As ParseTreeNode
        If Node.ChildNodes(0).Term.Name = "label" Then
            Return Node.ChildNodes(0)
        End If
        Return Node.ChildNodes(0).ChildNodes(0)
    End Function

    Public Function Compile(Name As String, ScriptContents As String) As String
        Dim Parser = New Parser(New ZcrGrammar)
        Dim ParseTree = Parser.Parse(ScriptContents)

        Dim Statements = (From Node In ParseTree.Root.ChildNodes(0).ChildNodes
                          Select SelectKeyStatement(Node)).ToList()

        Statements = RunFunctionifyKeywords(Statements)
        Statements = RunSplitPairsPass(Statements)
        Statements = RunAssignmentPass(Statements)
        Statements = RunLabelPass(Name & "_", Statements)
        Statements = RunFunctionPass(Statements)
        Statements = RunParameterizePass(Statements)

        Dim Result = "_:" & vbCrLf
        For Each Statement In Statements
            Result &= AsString(Statement) & vbCrLf
        Next
        Debug.Write(Result)
        Return Result
    End Function

    Private Function ReplaceStatement(Statements As IList(Of ParseTreeNode), StatementToReplace As ParseTreeNode, NewStatement As ParseTreeNode) As IList(Of ParseTreeNode)
        Dim Idx = Statements.IndexOf(StatementToReplace)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Statements.Remove(StatementToReplace)
        Statements.Insert(Idx, NewStatement)
        Return Statements
    End Function

    Private Function ToStatement(Statement As String) As ParseTreeNode
        Dim Parser = New Parser(New ZcrGrammar)
        Dim ParseTree = Parser.Parse(Statement)
        Return SelectKeyStatement(ParseTree.Root.ChildNodes(0).ChildNodes(0))
    End Function

    Private Function FindChildTerm(Statement As ParseTreeNode, Type As String) As ParseTreeNode
        If Statement.Term.Name = Type Then
            Return Statement
        Else
            For Each Child In Statement.ChildNodes
                Dim Result = FindChildTerm(Child, Type)
                If Result IsNot Nothing Then
                    Return Result
                End If
            Next
            Return Nothing
        End If
    End Function

    Private Function AsString(Statement As ParseTreeNode) As String
        Dim Result = ""
        If Statement.Term.Name = "function-call" Then
            Dim NewParams = From Param In Statement.ChildNodes(2).ChildNodes
                            Select AsString(Param)

            Dim FunctionCall = " " & Statement.ChildNodes(0).Token.Text & "(" & String.Join(", ", NewParams) & ")"
            Return FunctionCall
        End If
        If Statement.Term.Name = "label" Then
            Return Statement.ChildNodes(1).Token.Text & " = $ - -_"
        End If
        If Statement.Token IsNot Nothing Then
            Result &= Statement.Token.Text
        End If
        For Each Child In Statement.ChildNodes
            Result &= AsString(Child)
        Next
        Return Result
    End Function

    Private Iterator Function EnumerateNodes(Statements As IList(Of ParseTreeNode)) As IEnumerable(Of ParseTreeNode)
        For Each Statement In Statements
            Yield Statement
            For Each Child In EnumerateNodes(Statement.ChildNodes)
                Yield Child
            Next
        Next
    End Function

    Private Function RunFunctionifyKeywords(Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim Returns = From Statement In Statements
                      Select Statement
                      Where Statement.Term.Name = "return"
        For Each Ret In Returns
            Result = ReplaceStatement(Result, Ret, ToStatement(" s_return();"))
        Next

        Dim Deletes = From Statement In Statements
                      Select Statement
                      Where Statement.Term.Name = "delete"
        For Each Delete In Deletes
            Dim ObjType = FindChildTerm(Delete, "object-type")
            Dim ArrayAccess = FindChildTerm(Delete, "array-access")
            Result = ReplaceStatement(Result, Delete, ToStatement(String.Format(" s_prevent(kPrev_{0}, {1});",
                                                                                ObjType.ChildNodes(0).Token.Text,
                                                                                AsString(ArrayAccess.ChildNodes(2)))))
        Next
        Return Result
    End Function

    Private Function RunLabelPass(Prefix As String, Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim Labels = From Statement In Statements
                     Select Statement
                     Where Statement.Term.Name = "label"

        Dim Mapping As New Dictionary(Of String, String)
        For Each Label In Labels
            Dim NewLabel = Prefix & Label.ChildNodes(1).Token.Text
            Mapping(Label.ChildNodes(1).Token.Text) = NewLabel
            Result = ReplaceStatement(Result, Label, ToStatement("@" & NewLabel & ":"))
        Next

        Dim LabelRefs = From Ref In EnumerateNodes(Statements)
                        Select Ref
                        Where Ref.Term.Name = "label-ref"

        For Each Ref In LabelRefs
            Ref.ChildNodes(Ref.ChildNodes.Count - 1).Token.Text = If(Ref.ChildNodes.Count = 1, -1, Mapping(Ref.ChildNodes(1).Token.Text))
            Ref.ChildNodes.RemoveAt(0)
        Next

        Return Result
    End Function

    Private Function RunFunctionPass(Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim FunctionCalls = From Statement In Statements
                            Select Statement
                            Where Statement.Term.Name = "function-call" And
                                Statement.ChildNodes.Count > 0 AndAlso Not Statement.ChildNodes(0).Token.Text.StartsWith("s_")

        For Each Func In FunctionCalls
            Func.ChildNodes(0).Token.Text = "s_" & Func.ChildNodes(0).Token.Text
        Next

        Return Result
    End Function

    Private Function RunAssignmentPass(Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim Assignments = From Statement In Statements
                          Select Statement
                          Where Statement.Term.Name = "assignment"

        For Each Assignment In Assignments

            Dim IsPrev As Boolean = (Not FindChildTerm(Assignment, "assignment-type").ChildNodes(0).Token.Text = "=")

            Dim lvalue As ParseTreeNode = FindChildTerm(Assignment, "lvalue").ChildNodes(0)
            Dim NewStatement As ParseTreeNode
            If lvalue.Term.Name = "struct-access" Then
                Dim Is16 = lvalue.ChildNodes(2).Token.Text.EndsWith("_ptr")
                NewStatement = ToStatement(String.Format(" s_{0}_attr(kPrev_{1}, {2}, {1}_{3}_abs, {4}, {5});",
                                                         If(IsPrev, "mod", "set"),
                                                         AsString(FindChildTerm(lvalue, "object-type")),
                                                         AsString(FindChildTerm(lvalue, "array-access").ChildNodes(2)),
                                                         lvalue.ChildNodes(2).Token.Text,
                                                         AsString(Assignment.ChildNodes(2)),
                                                         If(Is16, "true", "false")))
            ElseIf lvalue.Term.Name = "variable-ref" Then
                Dim AddrToSet = Assignment.ChildNodes(2)
                Dim Expr = AsString(AddrToSet)

                Dim StructAccess = FindChildTerm(AddrToSet, "struct-access")
                If StructAccess IsNot Nothing Then
                    Expr = String.Format("{0}_array + (({1}) * {0}_width) + {0}_{2}_abs",
                                         AsString(FindChildTerm(StructAccess, "object-type")),
                                         AsString(FindChildTerm(StructAccess, "array-access").ChildNodes(2)),
                                         AsString(StructAccess.ChildNodes(2)))
                End If
                NewStatement = ToStatement(String.Format(" s_store(REG_{0}, {1});",
                                                         AsString(FindChildTerm(lvalue, "variable-ref").ChildNodes(1)),
                                                         Expr))
            ElseIf lvalue.Term.Name = "array-access" Then
                NewStatement = ToStatement(String.Format(" s_{0}_tile({1}, {2}, {3});",
                                                         If(IsPrev, "mod", "set"),
                                                         AsString(lvalue.ChildNodes(2)),
                                                         AsString(lvalue.ChildNodes(4)),
                                                         AsString(Assignment.ChildNodes(2))))
            Else
                NewStatement = ToStatement("function_call();")
            End If
            Result = ReplaceStatement(Result, Assignment, NewStatement)
        Next
        Return Result
    End Function

    Private Function Assemble(ByVal Code As String) As Byte()
        Dim Output = Assembler.Assemble(Code)
        Dim Data As New List(Of Byte)

        Dim st As New tagSTATSTG
        Output.Stat(st, 0)

        Dim Result() As Byte = New Byte(st.cbSize.QuadPart - 1) {}
        If st.cbSize.QuadPart > 0 Then
            Dim BytesRead As UInteger
            Output.RemoteRead(Result(0), st.cbSize.QuadPart, BytesRead)
        End If
        Return Result
    End Function

    Private Function RunParameterizePass(Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim Calls = From Statement In Statements
                    Select Statement
                    Where Statement.Term.Name = "function-call"

        For Each Func In Calls
            Dim Params = FindChildTerm(Func, "params")
            Dim HasRef = False
            For Idx = 0 To Params.ChildNodes.Count - 1
                Dim Arg = Params.ChildNodes(Idx)
                Dim Ref = FindChildTerm(Arg, "variable-ref")
                If Ref IsNot Nothing Then
                    HasRef = True

                    Dim NewParams = Enumerable.Repeat("0", Params.ChildNodes.Count).ToList()
                    NewParams(Idx) = "-1"
                    Dim MacroInvoke = " " & Func.ChildNodes(0).Token.Text & "(" & String.Join(",", NewParams) & ")"
                    Dim Data = Assemble(MacroInvoke)
                    Dim ParamIdx = 0
                    Do Until Data(ParamIdx) = &HFF
                        ParamIdx += 1
                    Loop

                    Dim StatementIdx = Result.IndexOf(Func)
                    Result.Insert(StatementIdx, ToStatement(String.Format(" s_parameter({0}, REG_{1});", ParamIdx, Ref.ChildNodes(1).Token.Text)))
                End If
            Next

            If HasRef Then
                Dim Args = Params.ChildNodes.Select(Function(Arg As ParseTreeNode)
                                                        Dim VarRef = FindChildTerm(Arg, "variable-ref")
                                                        Return If(VarRef Is Nothing, AsString(Arg), "0")
                                                    End Function)
                Dim NewCall = ToStatement(" " & Func.ChildNodes(0).Token.Text & "(" + String.Join(",", Args) & ");")
                Result = ReplaceStatement(Result, Func, NewCall)
            End If
        Next

        Return Result
    End Function

    Private Function RunSplitPairsPass(Statements As IList(Of ParseTreeNode)) As IList(Of ParseTreeNode)
        Dim Result = New List(Of ParseTreeNode)(Statements)
        Dim Assignments = From Statement In Statements
                          Select Statement
                          Where Statement.Term.Name = "assignment" _
                            AndAlso FindChildTerm(Statement, "pair") IsNot Nothing
        For Each Assignment In Assignments
            Dim NewAssignmentFormat = String.Format("{0}{{0}} {1} {{1}};",
                                          AsString(FindChildTerm(Assignment, "lvalue")),
                                          FindChildTerm(Assignment, "assignment-type").ChildNodes(0).Token.Text)
            Dim CtrStatement = ToStatement(String.Format(NewAssignmentFormat, "_ptr", AsString(FindChildTerm(Assignment, "pair").ChildNodes(3))))
            Result = ReplaceStatement(Result, Assignment, CtrStatement)
            Dim Idx = Result.IndexOf(CtrStatement)
            Result.Insert(Idx, ToStatement(String.Format(NewAssignmentFormat, "_ctr", AsString(FindChildTerm(Assignment, "pair").ChildNodes(1)))))
        Next

        Return Result
    End Function
End Class
