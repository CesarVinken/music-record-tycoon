using System.Collections.Generic;
using UnityEngine;

public class BuildMenuTabContainer : MonoBehaviour
{
    public static BuildMenuTabContainer Instance;

    public BuildMenuTab CurrentBuildMenuTab = null;
    public DeleteObjectMode DeleteObjectModeButton;
    public List<BuildMenuTab> BuildMenuTabs;

    public void Awake()
    {
        Instance = this;

        if (DeleteObjectModeButton == null) 
            Logger.Error(Logger.Initialisation, "Could not find DeleteObjectModeButton");

        if (BuildMenuTabs.Count == 0)
        {
            Logger.Error(Logger.Initialisation, "Could not find build menu tabs");
        }
    }

    public void ActivateBuildMenuTabs()
    {
        DeleteObjectModeButton.gameObject.SetActive(true);

        for (int i = 0; i < BuildMenuTabs.Count; i++)
        {
            BuildMenuTabs[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateBuildMenuTabs()
    {
        DeleteObjectModeButton.gameObject.SetActive(false);

        for (int i = 0; i < BuildMenuTabs.Count; i++)
        {
            BuildMenuTabs[i].gameObject.SetActive(false);
        }
    }

    public void SetCurrentBuildMenuTab(BuildMenuTab buildMenuTab) 
    {
        if (CurrentBuildMenuTab != null && buildMenuTab.TabType == CurrentBuildMenuTab.TabType) return;

        BuilderManager.Instance.DeleteAllTriggers();

        CurrentBuildMenuTab = buildMenuTab;
        BuildMenuContainer.Instance.RemoveBuildMenuContent(0);
        BuildMenuContainer.Instance.LoadBuildMenuContent(buildMenuTab.TabType);

        if(BuilderManager.InDeleteObjectMode)
            BuilderManager.Instance.DeactivateDeleteRoomMode();
    }

    public void ResetCurrentBuildMenuTab()
    {
        CurrentBuildMenuTab = null;
    }
}
