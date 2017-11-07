Imports System.Collections.ObjectModel
Imports System.IO
Imports WPFZ80MapEditor.ValueConverters

Public Class ObjectProperties
    Private _SelectedMap As MapData
    Public Property SelectedMap As MapData
        Get
            Return _SelectedMap
        End Get
        Set(value As MapData)
            _SelectedMap = value
            ComboScriptDefinitions.Clear()
            For Each Scr In _SelectedMap.ZScript
                ComboScriptDefinitions.Add(Scr)
            Next
            ComboScriptDefinitions.Add("Add new...")
        End Set
    End Property

    Public ReadOnly Property NamedSlots As IList(Of String)
        Get
            Return (From Obj In SelectedMap.ZAll
                    Where Obj.NamedSlot IsNot Nothing
                    Select Obj.NamedSlot
                    Order By NamedSlot).ToList()
        End Get
    End Property

    Public Property ComboScriptDefinitions As New ObservableCollection(Of Object)

    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub SelectLabel_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub EnableNamedSlot_Click(sender As Object, e As RoutedEventArgs)
        If EnableNamedSlot.IsChecked Then
            NamedSlot.Focus()
        Else
            DataContext.NamedSlot = Nothing
        End If
    End Sub

    Private Function OpenEditor(Script As ZScript, IsNew As Boolean) As Tuple(Of String, String)
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
        For Each Obj In SelectedMap.ZAll
            If Obj.NamedSlot IsNot Nothing Then
                Replacements(Obj.NamedSlot) = SlotConverter.Convert({Obj.NamedSlot, SelectedMap},
                                                                    GetType(String), Nothing, Nothing)
            End If
        Next
        Dim ScriptWindow = New EditorWindow(Application.Current.MainWindow, If(IsNew, "New Script", Script.Args(1).Value),
                                            RandomFile, IsNew, Replacements)
        Dim Result = ScriptWindow.ShowDialog()
        If Result Then
            Return New Tuple(Of String, String)(RandomFile, ScriptWindow.ScriptName)
        Else
            Return Nothing
        End If
    End Function

    Private Sub Edit_Click(sender As Object, e As RoutedEventArgs)
        Dim Converter As New ZScriptToObjectIDConverter()
        Dim Script = Converter.Convert1({sender.DataContext.Value, SelectedMap}, GetType(ZScript), Nothing, Nothing)
        Dim Result = OpenEditor(Script, False)
        If Result IsNot Nothing Then
            Dim Reader = New StreamReader(Result.Item1)
            Dim Contents = Reader.ReadToEnd()
            Script.ScriptContents = Contents
            Reader.Close()
        End If
    End Sub

    Private Sub Delete_Click(sender As Object, e As RoutedEventArgs)
        Dim Converter As New ZScriptToObjectIDConverter()
        Dim Script = Converter.Convert1({sender.DataContext.Value, SelectedMap}, GetType(ZScript), Nothing, Nothing)
        Dim Result = MsgBox("Are you sure you want to delete this script from this map?", vbApplicationModal Or vbYesNo)
        If Result = vbYes Then
            SelectedMap.ZScript.Remove(Script)
            ComboScriptDefinitions.Remove(Script)
            sender.DataContext.Value = Nothing
        End If
    End Sub

    Private _OldScriptSelection As ZScript = Nothing

    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If e.AddedItems.Contains("Add new...") Then
            Dim Result = OpenEditor(Nothing, True)
            If Result IsNot Nothing Then
                Dim Idx = 1
                Do While SelectedMap.ZScript.Any(Function(Scr)
                                                     Return Scr.Args(0).Value = SelectedMap.MapPrefix & "SCRIPT_" & Idx
                                                 End Function)
                    Idx += 1
                Loop

                Dim ScriptId = SelectedMap.MapPrefix & "SCRIPT_" & Idx

                Dim Script As ZScript = ZScript.FromDef(SelectedMap.Scenario.ScriptDef, {ScriptId, SelectedMap.MapPrefix & Result.Item2 & "_SCRIPT"})
                Dim Reader = New StreamReader(Result.Item1)
                Dim Contents = Reader.ReadToEnd()
                Script.ScriptContents = Contents
                Reader.Close()
                SelectedMap.ZScript.Add(Script)
                ComboScriptDefinitions.Insert(ComboScriptDefinitions.Count - 1, Script)
                e.Handled = True

                sender.SelectedItem = Script
            Else
                e.Handled = True
                sender.SelectedItem = _OldScriptSelection
            End If
        End If
    End Sub

    Private Sub ComboBox_DropDownOpened(sender As Object, e As EventArgs)
        _OldScriptSelection = sender.SelectedItem
    End Sub
End Class

Public Class ArgToColumnConverter
    Inherits OneWayConverter(Of Object, Integer)

    Public Overrides Function Convert(Value As Object, Parameter As Object) As Integer
        If TypeOf Value Is ArgNameAndIndex Then
            Return 0
        Else
            Return 1
        End If
    End Function
End Class

Public Class ObjectIDCollection
    Inherits List(Of String)
End Class