Imports Revsoft.TextEditor.Document

Public Class EditorWindow

    Private EditorFilePath As String
    Private ScriptEditor As ScriptEditor

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


    Public Sub New(Owner As Window, ScriptName As String, filePath As String, IsNew As Boolean)
        InitializeComponent()
        Me.Owner = Owner
        Me.IsNew = IsNew

        EditorFilePath = filePath
        Title = ScriptName
        ScriptEditor = ScriptEditorHost.Child

        Dim provider = New FileSyntaxModeProvider(IO.Path.Combine(IO.Directory.GetCurrentDirectory(), "Editor"))
        HighlightingManager.Manager.AddSyntaxModeFileProvider(provider)
        ScriptEditor.SetHighlighting("Zelda Script")
        ScriptEditor.ActiveTextAreaControl.HorizontalScroll.Enabled = False
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

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        ScriptEditor.SaveFile(EditorFilePath)
        ScriptName = NewScriptTextBox.Text
        ScriptName = ScriptName.Replace(" ", "_").ToUpper()
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub
End Class
