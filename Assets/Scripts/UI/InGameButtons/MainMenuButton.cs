using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
   public void OpenMainMenu()
    {
        MainMenuCanvas.Instance.ToggleMainMenu();
    }
}
