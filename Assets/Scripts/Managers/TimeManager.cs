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

    public void Awake()
    {
        Instance = this;

        SetYear(1952);
        SetTime(12);
    }

    public void Start()
    {
        StartTime();
    }

    public void StartTime()
    {
        InvokeRepeating("TriggerTimeIncrease", .0001f, 1.0f);
    }

    public void SetYear(int currentYear)
    {
        CurrentYear = currentYear;
        CurrentDayInYear = 0;
    }

    public void SetTime(int timeInHours)
    {
        if(timeInHours > 23)
        {
            Logger.Error("Set time can only between 0 and 24");
        }

        CurrentTimeInHours = timeInHours;

        SetDayPart();
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

    }

    public void IncreaseYear(int years)
    {
        CurrentYear = CurrentYear + years;
        CurrentDayInYear = 0;
        Logger.Log(Logger.Time, "The year is now {0}", CurrentYear);
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
