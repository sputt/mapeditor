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

    Public ReadOnly Property NamedSlots As IEnumerable(Of String)
        Get
            Return From Obj In SelectedMap.ZAll
                   Where Obj.NamedSlot IsNot Nothing
                   Select Obj.NamedSlot
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

    Private Sub Edit_Click(sender As Object, e As RoutedEventArgs)
        Dim Converter As New ZScriptToObjectIDConverter()
        Dim Script = Converter.Convert1({sender.DataContext.Value, SelectedMap}, GetType(ZScript), Nothing, Nothing)
        EditorWindow.OpenEditor(SelectedMap, Script, False)
    End Sub

    Private Sub Delete_Click(sender As Object, e As RoutedEventArgs)
        Dim Converter As New ZScriptToObjectIDConverter()
        Dim Script = Converter.Convert1({sender.DataContext.Value, SelectedMap}, GetType(ZScript), Nothing, Nothing)
        Dim Result = MsgBox("Are you sure you want to delete this script from this map?", vbApplicationModal Or vbYesNo)
        If Result = vbYes Then
            ComboScriptDefinitions.Remove(Script)
            sender.DataContext.Value = Nothing
        End If
    End Sub

    Private _OldScriptSelection As ZScript = Nothing

    Private Sub ComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If e.AddedItems.Contains("Add new...") Then
            Dim Result = EditorWindow.OpenEditor(SelectedMap, Nothing, True)
            If Result IsNot Nothing Then
                ComboScriptDefinitions.Insert(ComboScriptDefinitions.Count - 1, Result)
                e.Handled = True

                sender.SelectedItem = Result
            Else
                e.Handled = True
                sender.SelectedItem = _OldScriptSelection
            End If
        End If
    End Sub

    Private Sub ComboBox_DropDownOpened(sender As Object, e As EventArgs)
        _OldScriptSelection = sender.SelectedItem
    End Sub

    Private Sub CollectionViewSource_Filter(sender As Object, e As FilterEventArgs)

    End Sub

    Private Sub NamedSlotSource_Filter(sender As Object, e As FilterEventArgs)
        Dim EditingObj As IBaseGeneralObject = Me.DataContext
        If EditingObj.Definition.Properties.ContainsKey("SLOT_TYPE") Then
            e.Accepted = SelectedMap.ZAll _
                .Where(Function(Obj)
                           Return Obj.NamedSlot = e.Item
                       End Function) _
                .Where(Function(obj)
                           Return obj.Definition.Macro.ToUpper() = EditingObj.Definition.Properties("SLOT_TYPE").ToUpper()
                       End Function).Any()
        Else
            e.Accepted = True
        End If
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