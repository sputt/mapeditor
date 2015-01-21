﻿Imports System.Collections.ObjectModel
Imports System.Collections.Specialized

Public Class MapView
    Inherits FrameworkElement

    Public Property Map As MapData
        Get
            Return GetValue(MapProperty)
        End Get

        Set(ByVal value As MapData)
            SetValue(MapProperty, value)
        End Set
    End Property

    Private Shared Function OnMapChanged(d As DependencyObject, value As Object) As Boolean
        Dim MapView As MapView = d
        AddHandler MapView.Map.TileData.CollectionChanged, AddressOf MapView.OnTileDataCollectionChanged
        Return True
    End Function

    Private Sub OnTileDataCollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
        Me.InvalidateVisual()
    End Sub

    Public Shared ReadOnly MapProperty As DependencyProperty = _
                           DependencyProperty.Register("Map", _
                           GetType(MapData), GetType(MapView), _
                           New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.AffectsRender, AddressOf OnMapChanged))

    Protected Overrides Sub OnRender(DrawingContext As System.Windows.Media.DrawingContext)
        If Map Is Nothing OrElse Map.Tileset Is Nothing Then Exit Sub

        For i = 0 To Map.TileData.Count - 1
            Dim TileIndex = Map.TileData(i) Mod 128

            Dim Bounds As New Rect(16 * (i Mod 16), 16 * (Math.Floor(i / 16)), 16, 16)
            DrawingContext.DrawImage(Map.Tileset.Tiles(TileIndex).Image, Bounds)
        Next

    End Sub

End Class
