using UnityEngine;

// This is the script for the button to enter the build menu.
public class BuildMenuButton : MonoBehaviour
{
    public void ToggleBuildMenu()
    {
        if (BuilderManager.BuildMenuActivated) CloseBuildMenu();
        else OpenBuildMenu();
    }

    public void OpenBuildMenu()
    {
        Logger.Log(Logger.UI, "Open build menu");
        BuilderManager.Instance.ActivateBuildMenuMode();
    }

    public void CloseBuildMenu()
    {
        Logger.Log(Logger.UI, "Close build menu");
        BuilderManager.Instance.DeactivateBuildMenuMode();
    }

}
