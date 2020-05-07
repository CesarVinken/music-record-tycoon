using UnityEngine;
using UnityEngine.UI;

public class TimeDisplayContainer : MonoBehaviour
{
    public static TimeDisplayContainer Instance;

    [SerializeField]
    private Text _yearText;
    [SerializeField]
    private Text _timeText;

    public void Awake()
    {
        Instance = this;

        if (_yearText == null)
            Logger.Error(Logger.Initialisation, "Cannot find text component for YearText");

        if (_timeText == null)
            Logger.Error(Logger.Initialisation, "Cannot find text component for TimeText");
    }

    public void UpdateTimeUI(int newTime)
    {
        _timeText.text = newTime.ToString();
    }

    public void UpdateYearUI(int newYear)
    {
        _yearText.text = newYear.ToString();
    }
}
