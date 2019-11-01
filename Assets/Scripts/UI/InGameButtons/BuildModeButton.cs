using UnityEngine;

// This is the script for the button to enter build mode.
public class BuildModeButton : MonoBehaviour
{
    public void EnterBuildMode()
    {
        GameManager.Instance.BuilderManager.EnterBuildMode();
    }
}
