Imports Irony.Parsing

<Language("Zelda", "0.3", "Zelda Scripting Language")>
Public Class ZcrGrammar
    Inherits Irony.Parsing.Grammar

#Region "Declare Terminals Here"
    Public Sub New()
        Dim lineComment As New CommentTerminal("line-comment", ";;", vbCr, vbLf)
        NonGrammarTerminals.Add(lineComment)

        Dim number As New NumberLiteral("number")
        Dim identifier As New IdentifierTerminal("identifier")
#End Region


#Region "Declare NonTerminals Here"
        Dim script = New NonTerminal("script")
        Dim statements = New NonTerminal("statements")
        Dim statement = New NonTerminal("statement")
        Dim label = New NonTerminal("label")
        Dim labelRef = New NonTerminal("label-ref")
        Dim variableRef = New NonTerminal("variable-ref")

        Dim assignmentType = New NonTerminal("assignment-type")
        Dim assignment = New NonTerminal("assignment")

        Dim arrayAccess = New NonTerminal("array-access")
        Dim objectType = New NonTerminal("object-type")

        Dim structAccess = New NonTerminal("struct-access")
        Dim lvalue = New NonTerminal("lvalue")

        Dim expression = New NonTerminal("expression")
        Dim simpleExpression = New NonTerminal("simple-expression")
        Dim mathExpression = New NonTerminal("math-expression")
        Dim parenTerm = New NonTerminal("paren-term")
        Dim term = New NonTerminal("term")

        Dim functionCall = New NonTerminal("function-call")
        Dim parameters = New NonTerminal("params")
        Dim operatorTerm = New NonTerminal("operator")
        Dim delete = New NonTerminal("delete")
        Dim pair = New NonTerminal("pair")
        Dim hexNumber = New NonTerminal("hex-number")
#End Region


#Region "Place Rules Here"
        Me.Root = script

        script.Rule = statements

        statements.Rule = MakeStarRule(statements, statement)

        statement.Rule = (label + ":") Or ((assignment Or functionCall Or delete Or ToTerm("return")) + ";")

        label.Rule = ToTerm("@") + identifier

        assignmentType.Rule = ToTerm("=") Or ":=" Or "@="
        assignment.Rule = lvalue + assignmentType + (pair Or expression)

        pair.Rule = ToTerm("(") + expression + "," + expression + ")"

        objectType.Rule = ToTerm("misc") Or "animate" Or "object" Or "enemy"
        arrayAccess.Rule = (objectType + "[" + (simpleExpression Or variableRef) + "]") Or (ToTerm("map") + "[" + simpleExpression + "," + simpleExpression + "]")

        structAccess.Rule = (arrayAccess Or identifier) + "." + identifier

        lvalue.Rule = structAccess Or identifier Or variableRef Or arrayAccess

        hexNumber.Rule = ToTerm("$") + number
        term.Rule = number Or identifier Or "TRUE" Or "FALSE" Or "true" Or "false" Or hexNumber

        expression.Rule = simpleExpression Or labelRef Or variableRef
        simpleExpression.Rule = term Or structAccess Or parenTerm Or mathExpression

        parenTerm.Rule = ToTerm("(") + simpleExpression + ")"
        mathExpression.Rule = simpleExpression + operatorTerm + simpleExpression
        labelRef.Rule = (ToTerm("@") + identifier) Or ToTerm("*")

        variableRef.Rule = ToTerm("{") + identifier + "}"
        operatorTerm.Rule = ToTerm("+") Or "-" Or "*" Or "/" Or "|" Or "&"

        functionCall.Rule = identifier + "(" + parameters + ")"
        parameters.Rule = MakeStarRule(parameters, ToTerm(","), expression)

        delete.Rule = ToTerm("delete") + arrayAccess
#End Region


#Region "Define Keywords"
        Me.MarkReservedWords("return", "TRUE", "true", "FALSE", "false", "delete")
#End Region
    End Sub
End Class
