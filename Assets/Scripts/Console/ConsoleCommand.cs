
using System.Collections.Generic;

public class ConsoleCommand
{
    public string Name;
    public int ArgumentCountMin;
    public int ArgumentCountMax;

    private ConsoleCommand(string name, int argumentCountMin, int argumentCountMax)
    {
        Guard.CheckIsEmptyString("name", name);

        if(argumentCountMin > argumentCountMax)
        {
            Logger.Error("Cannot add command {0} because argumentCountMin cannot be larger than argumentCountMax", name);
        }

        Name = name.ToLower();
        ArgumentCountMin = argumentCountMin;
        ArgumentCountMax = argumentCountMax;
    }

    public static ConsoleCommand AddCommand(string name, int argumentCountMin, int argumentCountMax)
    {
        return new ConsoleCommand(name, argumentCountMin, argumentCountMax);
    }

    public void Execute(List<string> arguments)
    {
        CheckArgumentCount(arguments);
        Logger.Log("Run command {0}", Name);
    }

    private void CheckArgumentCount(List<string> arguments)
    {
        string argumentCountMinPluralSuffix = "";
        string argumentCountMaxPluralSuffix = "";
        string argumentsPluralSuffix = "";
        if (ArgumentCountMin > 1)
            argumentCountMinPluralSuffix = "s";
        if (ArgumentCountMax > 1)
            argumentCountMaxPluralSuffix = "s";
        if (arguments.Count > 1)
            argumentsPluralSuffix = "s";

        if (arguments.Count < ArgumentCountMin && ArgumentCountMin != -1)
        {
            if (ArgumentCountMin == ArgumentCountMax)
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs exactly " + ArgumentCountMin + " argument" + argumentCountMinPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
                return;
            }
            else
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs at least " + ArgumentCountMin + " argument" + argumentCountMinPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
                return;
            }
        }
        else if (arguments.Count > ArgumentCountMax && ArgumentCountMin != -1)
        {
            if (ArgumentCountMin == ArgumentCountMax)
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs exactly " + ArgumentCountMin + " argument" + argumentCountMinPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
                return;
            }
            else
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs at most " + ArgumentCountMax + " argument" + argumentCountMaxPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
                return;
            }
        }
    }
}
