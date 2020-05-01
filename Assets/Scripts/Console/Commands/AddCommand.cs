using System.Collections.Generic;

public class AddCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        float total = 0f;
        string printLine = "";
        float argumentNumber;

        for (int i = 0; i < arguments.Count; i++)
        {
            if (float.TryParse(arguments[i], out argumentNumber))
            {
                total += argumentNumber;
                if(i == 0)
                {
                    printLine += arguments[i];
                } else
                {
                    printLine = printLine + " + " + arguments[i];
                }
            }
            else
            {
                Console.Instance.PrintToReportText("All arguments for the Add command should be numbers. Could not parse argument #" + (i + 1) + " (" + arguments[i] + ")");
                return;
            }

        }
        printLine = printLine + " = " + total;
        Console.Instance.PrintToReportText(printLine);
    }

    public override void Help()
    {
        string printLine = "Add up numbers with an unlimited number of arguments. For example, the command 'add 1 2 3' will result in '1 + 2 + 3 = 6'";
        Console.Instance.PrintToReportText(printLine);
    }
}
