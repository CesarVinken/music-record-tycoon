using System.Collections.Generic;
using System.Linq;

public class HelpCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        if (arguments.Count == 0 || arguments.Count > 1)
        {
            Console.Instance.PrintToReportText(defaultResponse());
            return;
        }

        if(arguments.Count == 1)
        {
            ConsoleCommand consoleCommand = Console.Instance.Commands.FirstOrDefault(command => command.Name == arguments[0]);
            if(consoleCommand == null)
            {
                Console.Instance.PrintToReportText(arguments[0] + " is not a known command.\n");
                Console.Instance.PrintToReportText(defaultResponse());
                return;
            }
            consoleCommand.CommandProcedure.Help();
        }

    }

    public string defaultResponse()
    {
        string helpString = "The possible commands to run are:";
        for (int i = 0; i < Console.Instance.Commands.Count; i++)
        {
            ConsoleCommand command = Console.Instance.Commands[i];
            helpString += "\n   -" + command.Name;
        }
        helpString += "\n";
        helpString += "\nUse 'close' or F1 to close the console.";
        return helpString;
    }

    public override void Help()
    {
        Console.Instance.PrintToReportText(defaultResponse());
    }
}
