using UnityEngine;

public class ExitBuildTabButton : MonoBehaviour
{
    public void ExitBuildMode()
    {
        GameManager.Instance.BuilderManager.DeactivateBuildTabMode();
    }
}
