Public Class SelectorWindow

    Public Shared ReadOnly XProperty As DependencyProperty =
        DependencyProperty.Register("X", GetType(Double), GetType(SelectorWindow), New UIPropertyMetadata(CDbl(-1)))

    Public Property X
        Get
            Return GetValue(XProperty)
        End Get
        Set(value)
            SetValue(XProperty, CDbl(value))
        End Set
    End Property

    Public Property Y
        Get
            Return GetValue(YProperty)
        End Get
        Set(value)
            SetValue(YProperty, CDbl(value))
        End Set
    End Property

    Public Shared ReadOnly YProperty As DependencyProperty =
        DependencyProperty.Register("Y", GetType(Double), GetType(SelectorWindow), New UIPropertyMetadata(CDbl(-1)))

    Private _IsDraggingMisc As Boolean = False
    Private _StartDrag As New Point

    Private Sub Misc_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        Dim Obj As FrameworkElement = sender

        Obj.CaptureMouse()
        _StartDrag = e.GetPosition(Me)

        _IsDraggingMisc = True
        e.Handled = True
    End Sub

    Private Sub Misc_MouseLeftButtonUp(sender As System.Object, e As System.Windows.Input.MouseButtonEventArgs)
        Dim Obj As FrameworkElement = sender

        Obj.ReleaseMouseCapture()
        _IsDraggingMisc = False
        e.Handled = True
    End Sub

    Private Sub Misc_MouseMove(sender As System.Object, e As System.Windows.Input.MouseEventArgs)
        If _IsDraggingMisc Then
            Dim CurPoint As Point = e.GetPosition(Me)
            Dim DragDelta = CurPoint - _StartDrag

            Dim OldX = GetValue(XProperty)
            SetValue(XProperty, Math.Max(0, Math.Min((512 - 192), (OldX + DragDelta.X))))

            Dim OldY = GetValue(YProperty)
            SetValue(YProperty, Math.Max(0, Math.Min((512 - 128), (OldY + DragDelta.Y))))

            _StartDrag = CurPoint
            e.Handled = True
        End If
    End Sub
End Class
