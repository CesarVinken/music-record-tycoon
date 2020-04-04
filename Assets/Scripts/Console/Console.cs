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
    public int inputHistoryIndex = 99;

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

        PrintToReportText(inputText);

        InputField.text = "";

        InputField.ActivateInputField();
        InputField.Select();

        if (inputText[0] == '$')
        {
            RunCommand(inputText);
        }
    }

    public void PrintToReportText(string text)
    {
        string reportText = ReportText.text;
        reportText += "\n";
        reportText += text;

        ReportText.text = reportText;
    }

    public void GetPreviousInput()
    {
        if (ReportText.text == "") return;

        string[] newLine = { "\n" };
        string[] reportTextArray = ReportText.text.Split('\n');

        if (inputHistoryIndex == 99)
        {
            inputHistoryIndex = reportTextArray.Length - 1;
        } else if(inputHistoryIndex > 1)
        {
            inputHistoryIndex--;
        } else
        {
            inputHistoryIndex = reportTextArray.Length - 1;
        }

        string previousReportText = reportTextArray[inputHistoryIndex];
        InputField.text = previousReportText;
    }

    public void GetNextInput()
    {
        if (ReportText.text == "") return;

        string[] newLine = { "\n" };
        string[] reportTextArray = ReportText.text.Split('\n');

        if (inputHistoryIndex == 99)
        {
            inputHistoryIndex = 1;
        }
        else if (inputHistoryIndex < reportTextArray.Length - 1)
        {
            inputHistoryIndex++;
        }
        else
        {
            inputHistoryIndex = 1;
        }

        string nextReportText = reportTextArray[inputHistoryIndex];
        InputField.text = nextReportText;
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
        Commands.Add(ConsoleCommand.AddCommand("close", new CloseConsoleCommand()));
        Commands.Add(ConsoleCommand.AddCommand("add", 1, new AddCommand()));
        Commands.Add(ConsoleCommand.AddCommand("build", 1, 2, new BuildCommand()));
        Commands.Add(ConsoleCommand.AddCommand("delete", 2, 3, new DeleteCommand()));
    }
}
