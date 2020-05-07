using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public int CurrentYear { get { return _currentYear; } private set { _currentYear = value; } }
    [SerializeField]
    private int _currentYear = 0;

    public int CurrentTimeInHours { get { return _currentTimeInHours; } private set { _currentTimeInHours = value; } }
    [SerializeField]
    private int _currentTimeInHours = 0;

    public DayPart CurrentDayPart { get { return _currentDayPart; } private set { _currentDayPart = value; } }
    [SerializeField]
    private DayPart _currentDayPart = 0;

    public int CurrentDayInYear { get { return _currentDayInYear; } private set { _currentDayInYear = value; } }
    [SerializeField]
    private int _currentDayInYear = 0;

    private int _daysPerYearThreshold = 1;

    public const int MinYear = 1950;
    public const int MaxYear = 2020;

    private float _timeIncreaseInterval = 1.0f;
    private float _timeUntilNextTimeIncrease = 0;
    public void Awake()
    {
        Instance = this;

        SetYear(MinYear);
        SetTime(12);

        _timeUntilNextTimeIncrease = _timeIncreaseInterval;
    }

    public void Start()
    {
        StartCoroutine(IncreaseTime());
    }

    private IEnumerator IncreaseTime()
    {
        Logger.Log(Logger.Time, "Start the time");
        while (!GameManager.GamePaused)
        {
            yield return new WaitForSeconds(0.5f);
            _timeUntilNextTimeIncrease = _timeUntilNextTimeIncrease - 0.5f;

            if(_timeUntilNextTimeIncrease <= 0)
            {
                TriggerTimeIncrease();
                _timeUntilNextTimeIncrease = _timeIncreaseInterval;
            }
        }
        Logger.Log(Logger.Time, "Stop the time");
        PauseTime();
    }

    public void SetYear(int currentYear)
    {
        CurrentYear = currentYear;
        CurrentDayInYear = 0;

        TimeDisplayContainer.Instance.UpdateYearUI(CurrentYear);
    }

    public void SetTime(int timeInHours)
    {
        if(timeInHours > 23)
        {
            Logger.Error("Set time can only between 0 and 24");
        }

        CurrentTimeInHours = timeInHours;

        SetDayPart();

        TimeDisplayContainer.Instance.UpdateTimeUI(CurrentTimeInHours);
    }

    public void TriggerTimeIncrease()
    {
        IncreaseTime(1);
    }

    public void TriggerYearIncrease()
    {
        IncreaseYear(1);
    }

    public void IncreaseTime(int hours)
    {
        CurrentTimeInHours = CurrentTimeInHours + hours;
        while (CurrentTimeInHours > 23)
        {
            CurrentTimeInHours = CurrentTimeInHours - 24;
        }
        SetDayPart();

        if(CurrentTimeInHours == 0)
        {
            CurrentDayInYear++;

            if(CurrentDayInYear >= _daysPerYearThreshold)
            {
                TriggerYearIncrease();
            }
        }
        Logger.Log(Logger.Time, "The time is now {0}", CurrentTimeInHours);
        TimeDisplayContainer.Instance.UpdateTimeUI(CurrentTimeInHours);
    }

    public void PauseTime()
    {
        StopCoroutine(IncreaseTime());
    }

    public void IncreaseYear(int years)
    {
        CurrentYear = CurrentYear + years;
        CurrentDayInYear = 0;
        Logger.Log(Logger.Time, "The year is now {0}", CurrentYear);
        TimeDisplayContainer.Instance.UpdateYearUI(CurrentYear);
    }

    private void SetDayPart()
    {
        if (CurrentTimeInHours >= 18)
            CurrentDayPart = DayPart.Evening;
        else if (CurrentTimeInHours >= 12)
            CurrentDayPart = DayPart.Afternoon;
        else if (CurrentTimeInHours >= 6)
            CurrentDayPart = DayPart.Morning;
        else
            CurrentDayPart = DayPart.Night;
    }
}
