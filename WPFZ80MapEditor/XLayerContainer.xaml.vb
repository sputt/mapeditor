﻿Imports System.Collections.ObjectModel

Public Class XLayerContainer
    Inherits ListBox

    Public Property Maps As ObservableCollection(Of MapData)
        Get
            Return GetValue(MapsProperty)
        End Get

        Set(ByVal value As ObservableCollection(Of MapData))
            SetValue(MapsProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MapsProperty As DependencyProperty = _
                           DependencyProperty.Register("Maps", _
                           GetType(ObservableCollection(Of MapData)), GetType(XLayerContainer), _
                           New PropertyMetadata(Nothing))

    Public Property MapTemplate As DataTemplate
        Get
            Return GetValue(MapTemplateProperty)
        End Get

        Set(ByVal value As DataTemplate)
            SetValue(MapTemplateProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MapTemplateProperty As DependencyProperty = _
                           DependencyProperty.Register("MapTemplate", _
                           GetType(DataTemplate), GetType(XLayerContainer), _
                           New PropertyMetadata(Nothing))

    Public Property Gap As Double
        Get
            Return GetValue(GapProperty)
        End Get

        Set(ByVal value As Double)
            SetValue(GapProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GapProperty As DependencyProperty = _
                           DependencyProperty.Register("Gap", _
                           GetType(Double), GetType(XLayerContainer), _
                           New FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsArrange))

    Public Sub ListBoxItem_MouseDown(Sender As Object, Args As MouseButtonEventArgs)
        Args.Handled = False
    End Sub

End Class

Public Class MapPositionConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return (value) * (256 + CType(parameter, Integer))
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return Nothing
    End Function
End Class