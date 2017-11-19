Imports System.IO
Imports System.Text.RegularExpressions
Imports Revsoft.TextEditor.Document
Imports WPFZ80MapEditor.ValueConverters

Public Class EditorWindow

    Private EditorFilePath As String
    Private ScriptEditor As ScriptEditor
    Private _FilePath As String
    Private _Replacements As IDictionary(Of String, String)
    Private _Map As MapData

    Public Property ScriptName As String

    Public Property IsNew As Boolean
        Get
            Return GetValue(IsNewProperty)
        End Get

        Set(ByVal value As Boolean)
            SetValue(IsNewProperty, value)
        End Set
    End Property

    Public Shared ReadOnly IsNewProperty As DependencyProperty =
                           DependencyProperty.Register("IsNew",
                           GetType(Boolean), GetType(EditorWindow),
                           New PropertyMetadata(False))


    Public Sub New(Owner As Window, ScriptName As String, filePath As String, IsNew As Boolean, Replacements As IDictionary(Of String, String), Map As MapData)
        InitializeComponent()
        Me.Owner = Owner
        Me.IsNew = IsNew
        Me._FilePath = filePath
        Me._Replacements = Replacements
        Me._Map = Map

        EditorFilePath = filePath
        Title = ScriptName
        ScriptEditor = ScriptEditorHost.Child

        Dim provider = New FileSyntaxModeProvider(IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "Editor"))
        HighlightingManager.Manager.AddSyntaxModeFileProvider(provider)
        ScriptEditor.SetHighlighting("Zelda Script")

        Dim Reader = New StreamReader(filePath)
        Dim ScriptText = Reader.ReadToEnd()
        Reader.Close()

        For Each Rep In Replacements
            Dim RegexObj As New Regex("\b" & Rep.Key & "\b")
            ScriptText = RegexObj.Replace(ScriptText, Rep.Value)
        Next

        ScriptEditor.Text = ScriptText
        ScriptEditor.ActiveTextAreaControl.HorizontalScroll.Enabled = False
        ScriptEditor.ActiveTextAreaControl.HorizontalScroll.Visible = False
    End Sub

    Private Sub SaveCanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = True
        e.Handled = True
    End Sub

    Private Sub SaveExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        ScriptEditor.SaveFile(EditorFilePath)
        e.Handled = True
    End Sub

    Private Sub UndoCanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = Not (ScriptEditor Is Nothing) AndAlso ScriptEditor.Document.UndoStack.CanUndo
        e.Handled = True
    End Sub

    Private Sub UndoExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        ScriptEditor.Document.UndoStack.Undo()
        e.Handled = True
    End Sub

    Private Sub RedoCanExecute(sender As Object, e As CanExecuteRoutedEventArgs)
        e.CanExecute = Not (ScriptEditor Is Nothing) AndAlso ScriptEditor.Document.UndoStack.CanRedo
        e.Handled = True
    End Sub

    Private Sub RedoExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        ScriptEditor.Document.UndoStack.Redo()
        e.Handled = True
    End Sub

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        For Each Rep In _Replacements
            Dim RegexObj As New Regex("\b" & Rep.Value & "\b")
            ScriptEditor.Text = RegexObj.Replace(ScriptEditor.Text, Rep.Key)
        Next
        ScriptEditor.SaveFile(EditorFilePath)
        ScriptName = NewScriptTextBox.Text
        ScriptName = ScriptName.Replace(" ", "_").ToUpper()
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub

    Public Shared Function OpenEditor(Map As MapData, Script As ZScript, IsNew As Boolean) As ZScript
        Dim Converter As New ZScriptToObjectIDConverter()
        Dim RandomFile = System.IO.Path.GetTempFileName() & ".zcr"

        Dim Stream = New StreamWriter(RandomFile)
        If Not IsNew Then
            Stream.Write(Script.ScriptContents)
        Else
            Stream.Write(";; Edit your script here")
        End If
        Stream.Close()


        Dim SlotConverter As New SlotPrefixConverter()

        Dim Replacements As New Dictionary(Of String, String)
        For Each Obj In Map.ZAll
            If Obj.NamedSlot IsNot Nothing Then
                Replacements(Obj.NamedSlot) = SlotConverter.Convert({Obj.NamedSlot, Map},
                                                                    GetType(String), Nothing, Nothing)
            End If
        Next
        Dim ScriptWindow = New EditorWindow(Application.Current.MainWindow, If(IsNew, "New Script", Script.Args(1).Value),
                                            RandomFile, IsNew, Replacements, Map)
        Dim Result = ScriptWindow.ShowDialog()
        If Result Then
            Dim Reader = New StreamReader(RandomFile)
            Dim Contents = Reader.ReadToEnd()
            Reader.Close()

            If IsNew Then
                Dim Idx = 1
                Do While Map.ZScript.Any(Function(Scr)
                                             Return Scr.Args(0).Value = Map.MapPrefix & "SCRIPT_" & Idx
                                         End Function)
                    Idx += 1
                Loop

                Dim ScriptId = Map.MapPrefix & "SCRIPT_" & Idx
                Script = ZScript.FromDef(Map.Scenario.ScriptDef, {ScriptId, Map.MapPrefix & ScriptWindow.ScriptName & "_SCRIPT"})
                Script.ScriptContents = Contents
                Map.ZScript.Add(Script)
            Else
                Script.ScriptContents = Contents
            End If

            Return Script
        Else
            Return Nothing
        End If
    End Function

    Function GrabTextUpToCommaOrEnd(WordList As IList(Of String), Start As Integer) As Tuple(Of String, Integer)
        Dim Result = ""
        While Start < WordList.Count
            If WordList(Start) = "," Then
                Return New Tuple(Of String, Integer)(Result, Start + 1)
            End If
            Result += WordList(Start)
            Start += 1
        End While
        Return New Tuple(Of String, Integer)(Result, Start)
    End Function

    Private Sub PlugCameraButton_Click(sender As Object, e As RoutedEventArgs)

        Dim CurLine = Me.ScriptEditor.ActiveTextAreaControl.Caret.Line
        Dim LineText = Me.ScriptEditor.ActiveTextAreaControl.Document.GetLineSegment(CurLine)

        Dim ScreenCommands = {"trigger_screen", "trigger_screen_fast", "scroll_screen"}
        If LineText.Words.Any(Function(WordSegment)
                                  Return ScreenCommands.Contains(WordSegment.Word)
                              End Function) Then

            Dim WordList = LineText.Words.
                Where(Function(WordSegment)
                          Return WordSegment.Type = TextWordType.Word
                      End Function).
                      Select(Function(WordSegment)
                                 Return WordSegment.Word
                             End Function).ToList()

            Dim Idx = -1
            Dim j = 0
            While Idx = -1 And j < ScreenCommands.Length
                Idx = WordList.IndexOf(ScreenCommands(j))
                j += 1
            End While
            Dim ScreenCommand = WordList(Idx)

            Dim Result = GrabTextUpToCommaOrEnd(WordList, Idx + 2)
            Idx = Result.Item2
            Dim X = SPASMHelper.Eval(Result.Item1)

            Result = GrabTextUpToCommaOrEnd(WordList, Idx)
            Idx = Result.Item2
            Dim Y = SPASMHelper.Eval(Result.Item1)

            Dim Selector As New SelectorWindow()
            Selector.DataContext = Me._Map
            Selector.X = X * 2
            Selector.Y = Y * 2
            Selector.Owner = Me
            Dim SelectResult = Selector.ShowDialog()

            If SelectResult Then
                Dim Doc = Me.ScriptEditor.ActiveTextAreaControl.Document
                Doc.Replace(LineText.Offset, LineText.Length,
                        "  " & ScreenCommand & "(" & CInt(Selector.X / 2) & ", " & CInt(Selector.Y / 2) & ", " & GrabTextUpToCommaOrEnd(WordList, Idx).Item1)
            End If
        End If
    End Sub

    Private Sub PlugTileButton_Click(sender As Object, e As RoutedEventArgs)
        Dim CurLine = Me.ScriptEditor.ActiveTextAreaControl.Caret.Line
        Dim LineText = Me.ScriptEditor.ActiveTextAreaControl.Document.GetLineSegment(CurLine)

        Dim TileCommands = {"change_tile", "prevent_tile", "change_prevent_tile"}
        If LineText.Words.Any(Function(WordSegment)
                                  Return TileCommands.Contains(WordSegment.Word)
                              End Function) Then

            Dim WordList = LineText.Words.
                Where(Function(WordSegment)
                          Return WordSegment.Type = TextWordType.Word
                      End Function).
                      Select(Function(WordSegment)
                                 Return WordSegment.Word
                             End Function).ToList()

            Dim Idx = -1
            Dim j = 0
            While Idx = -1 And j < TileCommands.Length
                Idx = WordList.IndexOf(TileCommands(j))
                j += 1
            End While
            Dim TileCommand = WordList(Idx)

            Dim Result = GrabTextUpToCommaOrEnd(WordList, Idx + 2)
            Idx = Result.Item2
            Dim X = SPASMHelper.Eval(Result.Item1)

            Result = GrabTextUpToCommaOrEnd(WordList, Idx)
            Idx = Result.Item2
            Dim Y = SPASMHelper.Eval(Result.Item1)

            Result = GrabTextUpToCommaOrEnd(WordList, Idx)
            Idx = Result.Item2
            Dim Tile = SPASMHelper.Eval(Result.Item1)

            Dim Selector As New TileSelector()
            Dim TileModel As New TileSelectorModel

            Dim Offset = CInt(Math.Floor(X / 16)) + Y

            Dim Entry = New TilegroupEntry(Offset, Tile)
            TileModel.StartingTileChange = New TilegroupSelection(Me._Map.Tileset, {Entry})

            TileModel.BackupTile = Me._Map.TileData(Offset)
            TileModel.SelectedMap = Me._Map.Clone()
            TileModel.SelectedTileset = Me._Map.Tileset
            TileModel.SelectedTile = New TileSelection(Me._Map.Tileset, Me._Map.TileData(Offset))
            TileModel.Scenario = Me._Map.Scenario
            Selector.DataContext = TileModel
            Selector.Owner = Me

            Dim SelectResult = Selector.ShowDialog()
            If SelectResult Then
                For i = 0 To 255
                    If Me._Map.TileData(i) <> TileModel.SelectedMap.TileData(i) Then
                        Dim NewX = (i Mod 16) * 16
                        Dim NewY = Math.Floor(i / 16) * 16

                        Dim Doc = Me.ScriptEditor.ActiveTextAreaControl.Document
                        Doc.Replace(LineText.Offset, LineText.Length,
                                "  " & TileCommand & "(" & CInt(NewX) & ", " & CInt(NewY) & ", " & TileModel.SelectedMap.TileData(i) & ");")
                    End If
                Next
            End If
        End If
    End Sub
End Class
