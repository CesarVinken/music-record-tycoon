using UnityEngine;

// This is the script for the button to enter build mode.
public class BuildTabButton : MonoBehaviour
{
    public void OpenBuildTab()
    {
        GameManager.Instance.BuilderManager.ActivateBuildTabMode();
    }
}
