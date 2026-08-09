Imports WPFZ80MapEditor
Imports Common.Logging

Module Module1

    Sub Main()
        Dim Logger = LogManager.GetLogger("Editor.Runner")

        SPASMHelper.Initialize("C:\Users\Clzdg\Documents\Projects\zelda")

        Logger.Info("HELLO!")
        Dim Scenario As New Scenario
        Scenario.LoadImages("C:\Users\Clzdg\Documents\Projects\zelda\graphics.asm").Wait()
        Scenario.LoadScenario("C:\Users\Clzdg\Documents\Projects\zelda", "maps\hill.zmap").Wait()

        Debug.WriteLine("Done loading")
        Console.ReadLine()

        UndoManager.PushUndoState(Scenario.Maps(0), UndoManager.TypeFlags.Anims)
    End Sub

End Module
