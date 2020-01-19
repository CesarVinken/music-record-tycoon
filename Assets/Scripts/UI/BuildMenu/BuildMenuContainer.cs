using System.Collections;
using UnityEngine;

// This is the script that drives the collection of buttons related to the build Tab. Such as the different rooms.
public class BuildMenuContainer : MonoBehaviour
{
    public static BuildMenuContainer Instance;

    //public GameObject ExitBuildMenuButton;
    //public GameObject RoomButton; // later replace with dynamic array
    //public GameObject DeleteRoomButton; // later replace with dynamic array
    public GameObject BuildItemsContainer;
    public GameObject BuildTabContainer;
    //public GameObject ExitBuildMenuButtonPrefab;
    //public GameObject RoomButtonPrefab;
    //public GameObject DeleteRoomButtonPrefab;
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

    //public void CreateAllButtons()
    //{
    //    ExitBuildMenuButton = Instantiate(ExitBuildMenuButtonPrefab, transform);
    //    RoomButton = Instantiate(RoomButtonPrefab, transform);
    //    DeleteRoomButton = Instantiate(DeleteRoomButtonPrefab, transform);
    //}

    //public void DeleteAllButtons()
    //{
    //    if (ExitBuildMenuButton != null)
    //        Destroy(ExitBuildMenuButton);
    //    if (RoomButton != null)
    //        Destroy(RoomButton); 
    //    if (DeleteRoomButton != null)
    //        Destroy(DeleteRoomButton);
    //}

    public void LoadBuildMenuContent(BuildMenuTabType buildMenuTabType)
    {
        // TODO: add GOs for all items for the current build menu tab
        // TODO: Should also present an updated version of which items the player can actually build (based on level/money/tech)

        // Currently just load test Room1 and Hallway
        if(buildMenuTabType == BuildMenuTabType.Rooms)
        {
            GameObject item = Instantiate(BuildItemPrefab, BuildItemsContainer.transform);
            item.name = "Room1";
            BuildItemTile buildItemTile = item.GetComponent<BuildItemTile>();

            buildItemTile.Setup("Room1", "This room is just for testing");
        }
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

        BuildMenuContainer.Instance.IsOpen = true;
        BuildMenuContainer.Instance.IsBuilding = true;
        BuildMenuContainer.Instance.ActivateAnimationFreeze();
        BuildMenuContainer.Instance.LoadBuildMenuContent(BuildMenuTabType.Rooms);

        BuildMenuTabContainer.Instance.ActivateBuildMenuTabs();

        BuildMenuContainer.Instance.CompletePanelActivation();
    }
}
