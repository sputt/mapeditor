﻿#ExternalChecksum("..\..\..\XLayerContainer.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","1E8C2A2F8029E34664023A1AE944B62D")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Forms.Integration
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Windows.Shell
Imports WPFZ80MapEditor


'''<summary>
'''XLayerContainer
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class XLayerContainer
    Inherits System.Windows.Controls.ListBox
    Implements System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector
    
    
    #ExternalSource("..\..\..\XLayerContainer.xaml",8)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents LayerContainer As XLayerContainer
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\XLayerContainer.xaml",12)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Paste As System.Windows.Input.CommandBinding
    
    #End ExternalSource
    
    Private _contentLoaded As Boolean
    
    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")>  _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = true
        Dim resourceLocater As System.Uri = New System.Uri("/WPFZ80MapEditor;component/xlayercontainer.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\XLayerContainer.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.LayerContainer = CType(target,XLayerContainer)
            Return
        End If
        If (connectionId = 2) Then
            Me.Paste = CType(target,System.Windows.Input.CommandBinding)
            
            #ExternalSource("..\..\..\XLayerContainer.xaml",13)
            AddHandler Me.Paste.CanExecute, New System.Windows.Input.CanExecuteRoutedEventHandler(AddressOf Me.Paste_CanExecute)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\XLayerContainer.xaml",14)
            AddHandler Me.Paste.Executed, New System.Windows.Input.ExecutedRoutedEventHandler(AddressOf Me.Paste_Executed)
            
            #End ExternalSource
            Return
        End If
        Me._contentLoaded = true
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")>  _
    Sub System_Windows_Markup_IStyleConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IStyleConnector.Connect
        Dim eventSetter As System.Windows.EventSetter
        If (connectionId = 3) Then
            eventSetter = New System.Windows.EventSetter()
            eventSetter.Event = System.Windows.UIElement.PreviewMouseLeftButtonDownEvent
            
            #ExternalSource("..\..\..\XLayerContainer.xaml",76)
            eventSetter.Handler = New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.PreviewMouseEvent)
            
            #End ExternalSource
            CType(target,System.Windows.Style).Setters.Add(eventSetter)
            eventSetter = New System.Windows.EventSetter()
            eventSetter.Event = System.Windows.UIElement.PreviewMouseRightButtonUpEvent
            
            #ExternalSource("..\..\..\XLayerContainer.xaml",77)
            eventSetter.Handler = New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.PreviewMouseEvent)
            
            #End ExternalSource
            CType(target,System.Windows.Style).Setters.Add(eventSetter)
        End If
    End Sub
End Class

