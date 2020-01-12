using UnityEngine;

public class BuildMenuTab : MonoBehaviour
{
    public BuildMenuTabType TabType;

    public void SelectTab()
    {
        BuildMenuTabContainer.Instance.SetCurrentBuildMenuTab(this);
    }
}
