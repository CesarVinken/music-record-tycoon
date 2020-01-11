using UnityEngine;

// This is the script for the button to enter the build menu.
public class BuildMenuButton : MonoBehaviour
{
    public void ToggleBuildMenu()
    {
        if (BuildMenuContainer.Instance.IsOpen) CloseBuildMenu();
        else OpenBuildMenu();
    }

    public void OpenBuildMenu()
    {
        Logger.Log(Logger.UI, "Open and activate build menu");
        BuilderManager.Instance.ActivateBuildMenuMode();
    }

    public void CloseBuildMenu()
    {
        Logger.Log(Logger.UI, "Close and deactivate build menu");
        BuilderManager.Instance.DeactivateBuildMenuMode();
    }
}
