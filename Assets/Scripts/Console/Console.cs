using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public ConsoleState ConsoleState;

    public Text InputText;
    public Text ReportText;
    public InputField InputField;
    public int inputHistoryIndex = 99;

    void Awake()
    {
        if (InputText == null)
            Logger.Log(Logger.Initialisation, "Could not find InputText component on console");

        if (ReportText == null)
            Logger.Log(Logger.Initialisation, "Could not find ReportText component on console");

        if (InputField == null)
            Logger.Log(Logger.Initialisation, "Could not find InputField component on console");

        ConsoleState = ConsoleState.Closed;
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

        string reportText = ReportText.text;
        reportText += "\n";
        reportText += InputText.text;

        InputField.text = "";
        ReportText.text = reportText;

        InputField.ActivateInputField();
        InputField.Select();
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
}
