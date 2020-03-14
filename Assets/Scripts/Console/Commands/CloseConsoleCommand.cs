using System.Collections.Generic;

public class CloseConsoleCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        ConsoleContainer.Instance.ToggleConsole(ConsoleState.Closed);

        Console.Instance.PrintToReportText("Closed the console");
    }
}
