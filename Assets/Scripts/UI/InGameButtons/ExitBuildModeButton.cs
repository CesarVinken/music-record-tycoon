using UnityEngine;

public class ExitBuildModeButton : MonoBehaviour
{
    public void ExitBuildMode()
    {
        GameManager.Instance.BuilderManager.ExitBuildMode();
    }
}
