using UnityEngine;

public class ConsoleButton : MonoBehaviour
{
    public void OpenConsole()
    {
        ConsoleContainer.Instance.ToggleConsole(ConsoleState.Large);

        MainMenuCanvas.Instance.CloseMainMenu();
    }
}
