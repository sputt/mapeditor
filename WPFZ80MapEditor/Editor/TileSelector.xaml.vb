Public Class TileSelector
    Private Sub OKButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles OKButton.Click
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub

    Private _InitialSet As Boolean = True

    Private Sub Window_Activated(sender As Object, e As EventArgs)
        If _InitialSet Then
            TileGroupLayer.FloatSelection(DataContext.StartingTileChange)
            TileGroupLayer.Active = True

            TileGroupLayer.Focus()

            _InitialSet = False
        End If
    End Sub

    Private Sub TileGroupLayer_PreviewSelectionChanged(sender As Object)
        Dim CurrSelection = TileGroupLayer.TilegroupSelection
        If CurrSelection Is Nothing OrElse CurrSelection.TilegroupEntries.Count = 0 Then Exit Sub

        TileGroupLayer.DeleteFloatingSelection()
    End Sub

    Private Sub TileGroupLayer_SelectionChanged(sender As Object)
        Dim CurrSelection = TileGroupLayer.TilegroupSelection
        If CurrSelection Is Nothing OrElse CurrSelection.TilegroupEntries.Count = 0 Then Exit Sub

        Dim Model As TileSelectorModel = Me.DataContext
        Dim Entry = New TilegroupEntry(CurrSelection.TilegroupEntries(0).Index, Model.SelectedTile)

        Dim NewSelection = New TilegroupSelection(Model.SelectedTileset, {Entry})
        TileGroupLayer.FloatSelection(NewSelection)
        TileGroupLayer.Active = True
    End Sub

End Class
