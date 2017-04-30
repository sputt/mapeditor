Imports Revsoft.TextEditor.Document

Public Class EditorWindow

    Private EditorFilePath As String
    Private ScriptEditor As ScriptEditor

    Public Sub New(filePath As String)
        InitializeComponent()
        EditorFilePath = filePath
        Title = IO.Path.GetFileName(filePath)
        ScriptEditor = ScriptEditorHost.Child

        Dim provider = New FileSyntaxModeProvider(IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "Editor"))
        HighlightingManager.Manager.AddSyntaxModeFileProvider(provider)
        ScriptEditor.SetHighlighting("Zelda Script")
        ScriptEditor.LoadFile(filePath)
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

End Class
