using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public static Console Instance;
    public ConsoleState ConsoleState;

    public Text InputText;
    public Text ReportText;
    public InputField InputField;
    public int inputHistoryIndex = 0;

    public List<ConsoleLine> ConsoleLines = new List<ConsoleLine>();
    public List<ConsoleCommand> Commands = new List<ConsoleCommand>();
    void Awake()
    {
        if (InputText == null)
            Logger.Log(Logger.Initialisation, "Could not find InputText component on console");

        if (ReportText == null)
            Logger.Log(Logger.Initialisation, "Could not find ReportText component on console");

        if (InputField == null)
            Logger.Log(Logger.Initialisation, "Could not find InputField component on console");

        Instance = this;
        ConsoleState = ConsoleState.Closed;

        RegisterCommands();
    }

    public void Update()
    {
        if (InputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GetPreviousInput();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GetNextInput();
            }
        }
    }

    public void SetConsoleState(ConsoleState newConsoleState)
    {
        ConsoleState = newConsoleState;
        InputField.ActivateInputField();
        InputField.Select();
    }

    public void PublishInputText()
    {
        if (InputField.text == "") return;

        string inputText = InputText.text;

        PrintToReportText(inputText, true);

        InputField.text = "";

        InputField.ActivateInputField();
        InputField.Select();

        if (inputText[0] == '$')
        {
            RunCommand(inputText);
        }
    }

    public void PrintToReportText(string text, bool isPlayerInput = false)
    {
        string consoleLine = "\n" + text;

        if (ConsoleLines.Count > 30)
        {
            ConsoleLines.RemoveAt(0);
        }

        ConsoleLines.Add(new ConsoleLine(consoleLine, isPlayerInput));
        inputHistoryIndex = ConsoleLines.Count;

        string reportText = "";

        for (int i = 0; i < ConsoleLines.Count; i++)
        {
            reportText = reportText + ConsoleLines[i].Text;
        }

        ReportText.text = reportText;
    }

    public void GetPreviousInput()
    {
        if (ConsoleLines.Count == 0) return;

        int localInputHistoryIndex = inputHistoryIndex;

        for (int i = localInputHistoryIndex - 1; i >= 0; i--)
        {
            if(ConsoleLines[i].IsPlayerInput)
            {
                inputHistoryIndex = i;
                break;
            }
        }

        if(inputHistoryIndex == localInputHistoryIndex)
        {
            for (int j = ConsoleLines.Count - 1; j >= 0; j--)
            {
                if (ConsoleLines[j].IsPlayerInput)
                {
                    inputHistoryIndex = j;
                    break;
                }
            }
        }

        if (inputHistoryIndex == localInputHistoryIndex)
        {
            InputField.text = "";
        }
        else
        {
            string previousReportText = ConsoleLines[inputHistoryIndex].Text;
            InputField.text = previousReportText;
        }

    }

    public void GetNextInput()
    {
        if (ConsoleLines.Count == 0) return;

        int localInputHistoryIndex = inputHistoryIndex;

        for (int i = inputHistoryIndex + 1; i < ConsoleLines.Count; i++)
        {
            if (ConsoleLines[i].IsPlayerInput)
            {
                inputHistoryIndex = i;
                break;
            }
        }
        if (inputHistoryIndex == localInputHistoryIndex)
        {
            for (int j = 0; j < ConsoleLines.Count; j++)
            {
                if (ConsoleLines[j].IsPlayerInput)
                {
                    inputHistoryIndex = j;
                    break;
                }
            }
        }

        if (inputHistoryIndex == localInputHistoryIndex)
        {
            InputField.text = "";
        }
        else
        {
            string nextReportText = ConsoleLines[inputHistoryIndex].Text;
            InputField.text = nextReportText;
        }
    }

    public void RunCommand(string line)
    {
        string sanatisedLine = line.Substring(1);
        sanatisedLine.Trim();
        Logger.Log("Command input {0}", sanatisedLine);
        string[] arguments = sanatisedLine.Split(' ');
        List<string> sanatisedArguments = new List<string>();
        for (int i = 0; i < arguments.Length; i++)
        {
            if (arguments[i] == "") continue;
            sanatisedArguments.Add(arguments[i].ToLower());
        }

        string commandName = sanatisedArguments[0];
        sanatisedArguments.RemoveAt(0); // remove title from arguments

        ConsoleCommand Command = Commands.Find(j => j.Name == commandName);

        if(Command == null)
        {
            Logger.Warning("Could not find a command with the name {0}", commandName);
            PrintToReportText("Could not find a command with the name {0}");
            return;
        }

        Command.Execute(sanatisedArguments);
    }

    public void RegisterCommands()
    {
        Commands.Add(ConsoleCommand.AddCommand("add", 1, new AddCommand()));
        Commands.Add(ConsoleCommand.AddCommand("build", 1, 2, new BuildCommand()));
        Commands.Add(ConsoleCommand.AddCommand("close", new CloseConsoleCommand()));
        Commands.Add(ConsoleCommand.AddCommand("confirm", 1, 1, new ConfirmCommand()));
        Commands.Add(ConsoleCommand.AddCommand("delete", 2, 3, new DeleteCommand()));
        Commands.Add(ConsoleCommand.AddCommand("help", 0, 1, new HelpCommand()));
        Commands.Add(ConsoleCommand.AddCommand("show", 1, 3, new ShowCommand()));
        Commands.Add(ConsoleCommand.AddCommand("time", 1, 3, new TimeCommand()));
    }
}
