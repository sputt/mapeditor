Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Partial Class Scenario
    Private Sub LoadDefs(FileName As String, Dictionary As Dictionary(Of String, ZDef), InstanceType As Type)
        Dim Rx As New Regex(
            "^;(?<Name>[a-zA-Z][a-zA-Z ]+) - (?<Description>.+)\s*" &
            "(^; (?<ArgName>\w+) - (?<ArgDesc>.+)\s*)*" &
            "(^;Properties\s*)?" &
            "(^; (?<PropertyName>[a-zA-Z_]+) = (?<PropertyValue>.+)\s*)*" &
            "#macro (?<MacroName>[a-z0-9_]+).*\s*$", RegexOptions.Multiline Or RegexOptions.Compiled)

        Dim Stream = New StreamReader(FileName)
        Dim Matches = Rx.Matches(Stream.ReadToEnd())
        Stream.Close()

        For Each Match As Match In Matches
            Dim Groups = Match.Groups

            Dim Properties As New Dictionary(Of String, Object)
            For i = 0 To Groups("PropertyName").Captures.Count - 1
                Dim PropVal As Object = Groups("PropertyValue").Captures(i).Value
                If PropVal.Contains(",") Then
                    Dim List As IEnumerable(Of String) = PropVal.Split(",")
                    PropVal = List.Select(Of String)(Function(s) s.Trim())
                End If
                Properties.Add(Groups("PropertyName").Captures(i).Value.ToUpper(),
                               PropVal)
            Next

            Dim Def As New ZDef(Groups("Name").Value,
                                Groups("MacroName").Value.ToUpper(),
                                Groups("Description").Value.Trim(), InstanceType, Properties)

            For i = 0 To Groups("ArgName").Captures.Count - 1
                Def.AddArg(Groups("ArgName").Captures(i).Value.ToUpper(), Groups("ArgDesc").Captures(i).Value.Trim(), Me)
            Next

            Dictionary.Add(Def.Macro, Def)
        Next
    End Sub
End Class
