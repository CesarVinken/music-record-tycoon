using System.Collections.Generic;

public class CloseConsoleCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        ConsoleContainer.Instance.ToggleConsole(ConsoleState.Closed);

        Console.Instance.PrintToReportText("Closed the console");
    }

    public override void Help()
    {
        string printLine = "The close command closes the console.";
        Console.Instance.PrintToReportText(printLine);
    }
}
