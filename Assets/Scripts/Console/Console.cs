using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public ConsoleState ConsoleState;

    public Text InputText;
    public Text ReportText;
    public InputField InputField;

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
}
