
using System.Collections.Generic;

public class ConsoleCommand
{
    public string Name;
    public int ArgumentCountMin;
    public int ArgumentCountMax;
    public CommandProcedure CommandProcedure;

    private ConsoleCommand(string name, int argumentCountMin, int argumentCountMax, CommandProcedure commandProcedure)
    {
        Guard.CheckIsEmptyString("name", name);

        if(argumentCountMin > argumentCountMax)
        {
            Logger.Error("Cannot add command {0} because argumentCountMin cannot be larger than argumentCountMax", name);
        }

        Name = name.ToLower();
        ArgumentCountMin = argumentCountMin;
        ArgumentCountMax = argumentCountMax;

        CommandProcedure = commandProcedure;
    }

    public static ConsoleCommand AddCommand(string name, int argumentCountMin, int argumentCountMax, CommandProcedure commandProcedure)
    {
        return new ConsoleCommand(name, argumentCountMin, argumentCountMax, commandProcedure);
    }

    public static ConsoleCommand AddCommand(string name, CommandProcedure commandProcedure)
    {
        return new ConsoleCommand(name, 0, 0, commandProcedure);
    }

    public static ConsoleCommand AddCommand(string name, int argumentCountMin, CommandProcedure commandProcedure)
    {
        return new ConsoleCommand(name, argumentCountMin, 9999, commandProcedure);
    }

    public void Execute(List<string> arguments)
    {
        bool hasValidArgumentCount = CheckArgumentCount(arguments);
      
        if (!hasValidArgumentCount) return;

        arguments = arguments.ConvertAll(argument => argument.ToLower());

        CommandProcedure.Run(arguments);
        Logger.Log("Successfuly executed command {0}", Name);
    }

    private bool CheckArgumentCount(List<string> arguments)
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
            }
            else
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs at least " + ArgumentCountMin + " argument" + argumentCountMinPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
            }
            return false;
        }
        else if (arguments.Count > ArgumentCountMax && ArgumentCountMin != -1)
        {
            if (ArgumentCountMin == ArgumentCountMax)
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs exactly " + ArgumentCountMin + " argument" + argumentCountMinPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
            }
            else
            {
                Console.Instance.PrintToReportText("Command " + Name + " needs at most " + ArgumentCountMax + " argument" + argumentCountMaxPluralSuffix + ". Received " + arguments.Count + " argument" + argumentsPluralSuffix);
            }
            return false;
        }
        return true;
    }
}

public abstract class CommandProcedure
{
    public abstract void Run(List<string> arguments);
    public abstract void Help();
}

