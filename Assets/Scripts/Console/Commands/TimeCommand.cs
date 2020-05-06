using System;
using System.Collections.Generic;

public class TimeCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        if (TimeManager.Instance == null)
        {
            Console.Instance.PrintToReportText("The time command can only be used while in game");
            return;
        }
        switch (arguments[0])
        {
            case "time":
                SetTime(arguments);
                break;
            case "year":
                SetYear(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown time command argument " + arguments[0]);
                break;
        }
    }

    public override void Help()
    {
        string printLine = "With the time command you can change the time of the game.";
        printLine += "\nUse 'time' as a 2nd argument to set the time of the game with the desired time (0-23) as 3rd argument.";
        printLine += "\nUse 'year' as a 2nd argument to set the year of the game with the desired year (" + TimeManager.MinYear + "-" + TimeManager.MaxYear + ") as 3rd argument.";
        Console.Instance.PrintToReportText(printLine);
    }

    private void SetTime(List<string> arguments)
    {
        if(arguments.Count < 2)
        {
            Console.Instance.PrintToReportText("To set the time please, please give a 3rd argument with the desired time (0-23)");
            return;
        }

        if(Int32.TryParse(arguments[1], out int result))
        {
            if(result < 0 || result > 23)
            {
                Console.Instance.PrintToReportText("Given argument for time " + arguments[0] + " is not a valid hour. Please give a number between 0 and 23 to set the time");
                return;
            }
            TimeManager.Instance.SetTime(result);
        }
        else
        {
            Console.Instance.PrintToReportText("Given argument for time " + arguments[0] + " is not a number. Please give a number between 0 and 23 to set the time");
        }
    }

    private void SetYear(List<string> arguments)
    {
        if (arguments.Count < 2)
        {
            Console.Instance.PrintToReportText("To set the year please, please give a 3rd argument with the desired year (" + TimeManager.MinYear + "-" + TimeManager.MaxYear + ")");
            return;
        }

        if (Int32.TryParse(arguments[1], out int result))
        {
            if (result < TimeManager.MinYear || result > TimeManager.MaxYear)
            {
                Console.Instance.PrintToReportText("Given argument for year " + arguments[0] + " is not a valid year. Please give a number between " + TimeManager.MinYear + " and "+ TimeManager.MaxYear + " to set the time");
                return;
            }
            TimeManager.Instance.SetYear(result);
        }
        else
        {
            Console.Instance.PrintToReportText("Given argument for time " + arguments[0] + " is not a number. Please give a number between " + TimeManager.MinYear + " and " + TimeManager.MaxYear + " to set the time");
        }
    }
}
