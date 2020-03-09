using UnityEngine;

public class ConsoleContainer : MonoBehaviour
{
    public static ConsoleContainer Instance;

    public GameObject ConsolePrefab;
    public GameObject ConsoleGO;

    private Console _console;
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

            _console = ConsoleGO.GetComponent<Console>();

            if (!_console)
                Logger.Error(Logger.Initialisation, "Cannot find console");
        }

        if (_console.ConsoleState == ConsoleState.Closed)
        {

            SetConsoleState(ConsoleState.Small);
        }
        else if (_console.ConsoleState == ConsoleState.Small)
        {
            SetConsoleState(ConsoleState.Large);
        }
        else
        {
            SetConsoleState(ConsoleState.Closed);
        }
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

        _console.SetConsoleState(nextState);   
    }
}
