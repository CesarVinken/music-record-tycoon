using UnityEngine;

public class BuildModeButton : MonoBehaviour
{
    public void ToggleBuildMode()
    {
        GameManager.InBuildMode = !GameManager.InBuildMode;
        Debug.Log("Build mode is " + GameManager.InBuildMode);
    }
}
