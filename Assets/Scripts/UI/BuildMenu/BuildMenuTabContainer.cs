using System.Collections.Generic;
using UnityEngine;

public class BuildMenuTabContainer : MonoBehaviour
{
    public static BuildMenuTabContainer Instance;

    public BuildMenuTab CurrentBuildMenuTab = null;
    public List<BuildMenuTab> BuildMenuTabs;

    public void Awake()
    {
        Instance = this;

        if(BuildMenuTabs.Count == 0)
        {
            Logger.Error(Logger.Initialisation, "Could not find build menu tabs");
        }
    }

    public void ActivateBuildMenuTabs()
    {
        for (int i = 0; i < BuildMenuTabs.Count; i++)
        {
            BuildMenuTabs[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateBuildMenuTabs()
    {
        for (int i = 0; i < BuildMenuTabs.Count; i++)
        {
            BuildMenuTabs[i].gameObject.SetActive(false);
        }
    }

    public void SetCurrentBuildMenuTab(BuildMenuTab buildMenuTab) 
    {
        if (CurrentBuildMenuTab != null && buildMenuTab.TabType == CurrentBuildMenuTab.TabType) return;

        CurrentBuildMenuTab = buildMenuTab;
        BuildMenuContainer.Instance.RemoveBuildMenuContent(0);
        BuildMenuContainer.Instance.LoadBuildMenuContent(buildMenuTab.TabType);
    }

    public void ResetCurrentBuildMenuTab()
    {
        CurrentBuildMenuTab = null;
    }
}
