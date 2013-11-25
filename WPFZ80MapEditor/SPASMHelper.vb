﻿Imports SPASM

Public Class SPASMHelper
    Public Shared Assembler As IZ80Assembler

    Public Shared Labels As New Dictionary(Of String, Integer)

    Public Shared Sub Initialize(Path As String)
        Try
            Assembler = New Z80Assembler
        Catch e As System.Runtime.InteropServices.COMException
            Exit Sub
        End Try

        Assembler.CurrentDirectory = Path

        Assembler.Defines.Add("_MAPEDITOR", "1")
        Assembler.IncludeDirectories.Add(Environment.CurrentDirectory)
        Assembler.IncludeDirectories.Add(Environment.CurrentDirectory & "\Images")
        Assembler.IncludeDirectories.Add(Environment.CurrentDirectory & "\Defaults")
        Assembler.IncludeDirectories.Add(Environment.CurrentDirectory & "\Maps")
        Assembler.Assemble("#include ""objectdef.inc""")
        Dim StdOutput = Assembler.StdOut.ReadAll()

    End Sub

    Public Shared Function Eval(ByVal Expr As String) As Integer
        If Expr Is Nothing Then
            Expr = "0"
        End If
        Dim Bytes = Assemble(".dw " & Expr)
        Return CInt(BitConverter.ToUInt16(Bytes, 0))
    End Function

    Public Shared Function Assemble(ByVal Code As String) As Byte()
        Dim Output = Assembler.Assemble(Code)

        Dim StdOutput = Assembler.StdOut.ReadAll()
        'Debug.Write(StdOutput)

        Dim Data As New List(Of Byte)

        Dim st As New tagSTATSTG
        Output.Stat(st, 0)

        Dim Result() As Byte = New Byte(st.cbSize.QuadPart - 1) {}
        If st.cbSize.QuadPart > 0 Then
            Dim BytesRead As UInteger
            Output.RemoteRead(Result(0), st.cbSize.QuadPart, BytesRead)
        End If
        Return Result
    End Function

    Public Shared Function AssembleFile(FileName As String) As Byte()
        Dim FullPath = System.IO.Path.GetFullPath(FileName)
        Dim Bytes = Assemble("#include """ & FullPath & """")
        Labels.Clear()
        For Each Label In Assembler.Labels.Keys
            Labels.Add(Label, Assembler.Labels(Label))
        Next
        Return Bytes
    End Function
End Class
