using System.Collections;
using UnityEngine;

// This is the script that drives the collection of buttons related to the build Tab. Such as the different rooms.
public class BuildMenuContainer : MonoBehaviour
{
    public static BuildMenuContainer Instance;

    public GameObject BuildItemsContainer;
    public GameObject BuildTabContainer;
    public GameObject BuildItemPrefab;

    private Animator _animator;

    public bool PointerOnBuildMenu = false;
    public bool PanelAnimationPlaying = false; // the player should not perform build actions while the menu is still openeing or closing

    public bool IsOpen
    {
        get
        {
            return _animator.GetBool("IsOpen");
        }
        set
        {
            _animator.SetBool("IsOpen", value);
        }
    }

    public bool IsBuilding // the state in which the player is dragging an item to build. The building menu can come back up once finishing the action
    {
        get
        {
            return _animator.GetBool("IsBuilding");
        }
        set
        {
            _animator.SetBool("IsBuilding", value);
        }
    }

    public void Awake()
    {
        _animator = GetComponent<Animator>();

        if (!_animator)
            Logger.Error(Logger.Initialisation, "Cannot find animator");

        Guard.CheckIsNull(BuildItemsContainer, "BuildItemsContainer");
        Guard.CheckIsNull(BuildTabContainer, "BuildTabContainer");
        Guard.CheckIsNull(BuildItemPrefab, "BuildItemPrefab");

        Instance = this;
    }

    public void LoadBuildMenuContent(BuildMenuTabType buildMenuTabType)
    {
        // TODO: add GOs for all items for the current build menu tab
        // TODO: Should also present an updated version of which items the player can actually build (based on level/money/tech)

        // Currently just load test Room1 and Hallway
        if(buildMenuTabType == BuildMenuTabType.Rooms)
        {
            RoomBlueprint room1 = RoomBlueprint.CreateBlueprint(RoomName.Room1);
            LoadBuildMenuItem(room1);

            RoomBlueprint hallway = RoomBlueprint.CreateBlueprint(RoomName.Hallway);
            LoadBuildMenuItem(hallway);

            RoomBlueprint recordingStudio1 = RoomBlueprint.CreateBlueprint(RoomName.RecordingStudio1);
            LoadBuildMenuItem(recordingStudio1);
        }
    }

    private void LoadBuildMenuItem(IBuildItemBlueprint buildItemBlueprint)
    {
        GameObject item = Instantiate(BuildItemPrefab, BuildItemsContainer.transform);
        item.name = name;
        BuildItemTile buildItemTile = item.GetComponent<BuildItemTile>();

        buildItemTile.Setup(buildItemBlueprint);
    }

    public void CompletePanelActivation()
    {
        IEnumerator completeActivation = CompletePanelActivationRoutine();

        StartCoroutine(completeActivation);
    }

    public IEnumerator CompletePanelActivationRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        BuilderManager.InBuildMode = true;
    }

    public void RemoveBuildMenuContent(float delay)
    {
        if(delay == 0)  // when changing tabs and refreshing the content
        {
            foreach (Transform child in BuildItemsContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else // when closing the menu, wait until the menu is closed before removing content
        {
            IEnumerator removeBuildMenuContent = RemoveBuildMenuContentRoutine(delay);
            StartCoroutine(removeBuildMenuContent);
        }
    }

    public void ActivateAnimationFreeze() // prevent the player from doing buildingactions while the panel is opening or closing
    {
        IEnumerator activateAnimationFreezeRoutine = ActivateAnimationFreezeRoutine();
        StartCoroutine(activateAnimationFreezeRoutine);
    }

    public IEnumerator RemoveBuildMenuContentRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (Transform child in BuildItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        BuildMenuTabContainer.Instance.DeactivateBuildMenuTabs();
    }

    public IEnumerator ActivateAnimationFreezeRoutine()
    {
        PanelAnimationPlaying = true;
        yield return new WaitForSeconds(0.5f); // length of panel opening/closing animation
        PanelAnimationPlaying = false;
    }

    public IEnumerator WaitAndReopenPanelRoutine()
    {
        yield return new WaitForSeconds(0.5f); // length of panel opening/closing animation

        IsOpen = true;
        IsBuilding = true;
        ActivateAnimationFreeze();
        LoadBuildMenuContent(BuildMenuTabType.Rooms);

        BuildMenuTabContainer.Instance.ActivateBuildMenuTabs();

        CompletePanelActivation();
    }
}
