using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
   public void MainMenu()
    {
        MainMenuCanvas.Instance.ToggleMainMenu();
    }

    public void DestroyMainMenuButton()
    {
        Destroy(gameObject);
        Destroy(this);
    }
}
