using UnityEngine;

public class ConsoleContainer : MonoBehaviour
{
    public static ConsoleContainer Instance;

    public GameObject ConsolePrefab;
    public GameObject ConsoleGO;

    private Animator _animator;

    public int AnimatorConsoleState
    {
        get
        {
            return _animator.GetInteger("ConsoleState");
        }
        set
        {
            _animator.SetInteger("ConsoleState", value);
        }
    }

    public void Awake()
    {
        Guard.CheckIsNull(ConsolePrefab, "ConsolePrefab");

        Instance = this;
    }

    public void ToggleConsole()
    {
        if (!ConsoleGO)
        {
            ConsoleGO = Instantiate(ConsolePrefab, transform, false);

            _animator = ConsoleGO.GetComponent<Animator>();

            if (!_animator)
                Logger.Error(Logger.Initialisation, "Cannot find animator");

            if (!Console.Instance)
                Logger.Error(Logger.Initialisation, "Cannot find console");
        }

        if (Console.Instance.ConsoleState == ConsoleState.Closed)
        {

            SetConsoleState(ConsoleState.Small);
        }
        else if (Console.Instance.ConsoleState == ConsoleState.Small)
        {
            SetConsoleState(ConsoleState.Large);
        }
        else
        {
            SetConsoleState(ConsoleState.Closed);
        }
    }

    public void ToggleConsole(ConsoleState forcedState)
    {
        if (!ConsoleGO)
        {
            ConsoleGO = Instantiate(ConsolePrefab, transform, false);

            _animator = ConsoleGO.GetComponent<Animator>();

            if (!_animator)
                Logger.Error(Logger.Initialisation, "Cannot find animator");

            if (!Console.Instance)
                Logger.Error(Logger.Initialisation, "Cannot find console");
        }
        SetConsoleState(forcedState);
    }

    private void SetConsoleState (ConsoleState nextState)
    {
        switch (nextState)
        {
            case ConsoleState.Closed:
                AnimatorConsoleState = 0;
                break;
            case ConsoleState.Small:
                AnimatorConsoleState = 1;
                break;
            case ConsoleState.Large:
                AnimatorConsoleState = 2;
                break;
            default:
                Logger.Error("Unknown console state {0}", nextState);
                break;
        }

        Console.Instance.SetConsoleState(nextState);   
    }
}
