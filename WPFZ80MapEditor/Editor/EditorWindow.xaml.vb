Imports System.IO
Imports System.Text.RegularExpressions
Imports Revsoft.TextEditor.Document
Imports WPFZ80MapEditor.ValueConverters

Public Class EditorWindow

    Private EditorFilePath As String
    Private ScriptEditor As ScriptEditor
    Private _FilePath As String
    Private _Replacements As IDictionary(Of String, String)

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


    Public Sub New(Owner As Window, ScriptName As String, filePath As String, IsNew As Boolean, Replacements As IDictionary(Of String, String))
        InitializeComponent()
        Me.Owner = Owner
        Me.IsNew = IsNew
        Me._FilePath = filePath
        Me._Replacements = Replacements

        EditorFilePath = filePath
        Title = ScriptName
        ScriptEditor = ScriptEditorHost.Child

        Dim provider = New FileSyntaxModeProvider(IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "Editor"))
        HighlightingManager.Manager.AddSyntaxModeFileProvider(provider)
        ScriptEditor.SetHighlighting("Zelda Script")
        ScriptEditor.ActiveTextAreaControl.HorizontalScroll.Enabled = False

        Dim Reader = New StreamReader(filePath)
        Dim ScriptText = Reader.ReadToEnd()
        Reader.Close()

        For Each Rep In Replacements
            Dim RegexObj As New Regex("\b" & Rep.Key & "\b")
            ScriptText = RegexObj.Replace(ScriptText, Rep.Value)
        Next

        ScriptEditor.Text = ScriptText

        'ScriptEditor.LoadFile(filePath)
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
                                            RandomFile, IsNew, Replacements)
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
End Class
