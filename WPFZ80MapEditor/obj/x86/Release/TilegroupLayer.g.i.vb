﻿#ExternalChecksum("..\..\..\TilegroupLayer.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","1C7D3A3521B1B6994DF3992867E64398")
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
Imports WPFZ80MapEditor.ValueConverters


'''<summary>
'''TilegroupLayer
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class TilegroupLayer
    Inherits WPFZ80MapEditor.MapLayer
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\TilegroupLayer.xaml",2)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents TilegroupLayer As TilegroupLayer
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\TilegroupLayer.xaml",25)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents Copy As System.Windows.Input.CommandBinding
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\TilegroupLayer.xaml",29)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents ObjectCanvas As System.Windows.Controls.Canvas
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\TilegroupLayer.xaml",30)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents TilegroupSelectionShape As System.Windows.Shapes.Path
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\TilegroupLayer.xaml",63)
    <System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")>  _
    Friend WithEvents SelectionRect As System.Windows.Controls.Border
    
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
        Dim resourceLocater As System.Uri = New System.Uri("/WPFZ80MapEditor;component/tilegrouplayer.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\TilegroupLayer.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
    Friend Function _CreateDelegate(ByVal delegateType As System.Type, ByVal handler As String) As System.[Delegate]
        Return System.[Delegate].CreateDelegate(delegateType, Me, handler)
    End Function
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0"),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.TilegroupLayer = CType(target,TilegroupLayer)
            Return
        End If
        If (connectionId = 2) Then
            Me.Copy = CType(target,System.Windows.Input.CommandBinding)
            
            #ExternalSource("..\..\..\TilegroupLayer.xaml",26)
            AddHandler Me.Copy.CanExecute, New System.Windows.Input.CanExecuteRoutedEventHandler(AddressOf Me.Copy_CanExecute)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\TilegroupLayer.xaml",27)
            AddHandler Me.Copy.Executed, New System.Windows.Input.ExecutedRoutedEventHandler(AddressOf Me.Copy_Executed)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 3) Then
            Me.ObjectCanvas = CType(target,System.Windows.Controls.Canvas)
            Return
        End If
        If (connectionId = 4) Then
            Me.TilegroupSelectionShape = CType(target,System.Windows.Shapes.Path)
            
            #ExternalSource("..\..\..\TilegroupLayer.xaml",37)
            AddHandler Me.TilegroupSelectionShape.MouseLeftButtonDown, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.TilegroupSelectionShape_MouseLeftButtonDown)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\TilegroupLayer.xaml",38)
            AddHandler Me.TilegroupSelectionShape.MouseLeftButtonUp, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.TilegroupSelectionShape_MouseLeftButtonUp)
            
            #End ExternalSource
            
            #ExternalSource("..\..\..\TilegroupLayer.xaml",39)
            AddHandler Me.TilegroupSelectionShape.MouseMove, New System.Windows.Input.MouseEventHandler(AddressOf Me.TilegroupSelectionShape_MouseMove)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 5) Then
            Me.SelectionRect = CType(target,System.Windows.Controls.Border)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class
