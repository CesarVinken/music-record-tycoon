using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

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
    //public GameObject BuildTabPrefab;

    private Animator _animator;
    //private CanvasGroup _canvasGroup;

    public bool PointerOnBuildMenu = false;

    public bool IsOpen
    {
        get { return _animator.GetBool("IsOpen"); }
        set { _animator.SetBool("IsOpen", value); }
    }

    public void Awake()
    {
        _animator = GetComponent<Animator>();
        //_canvasGroup = GetComponent<CanvasGroup>();

        if (!_animator)
            Logger.Error(Logger.Initialisation, "Cannot find animator");

        //if (!_canvasGroup)
        //    Logger.Error(Logger.Initialisation, "Cannot find canvasGroup");

        //Guard.CheckIsNull(ExitBuildMenuButtonPrefab, "ExitBuildMenuButtonPrefab");
        //Guard.CheckIsNull(RoomButtonPrefab, "RoomButtonPrefab");
        //Guard.CheckIsNull(DeleteRoomButtonPrefab, "DeleeRoomButtonPrefab");
        Guard.CheckIsNull(BuildItemsContainer, "BuildItemsContainer");
        Guard.CheckIsNull(BuildTabContainer, "BuildTabContainer");
        Guard.CheckIsNull(BuildItemPrefab, "BuildItemPrefab");

        Instance = this;
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    //Debug.Log("Mouse Over: " + eventData.pointerCurrentRaycast.gameObject.name);
    //    if (eventData.pointerCurrentRaycast.gameObject != null)
    //    {
    //        if (eventData.pointerCurrentRaycast.gameObject.name == "BuildText") return;

    //        PointerOnBuildMenu = true;
    //    }
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    PointerOnBuildMenu = false;
    //}

    //public void Update()
    //{
    //    if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
    //    {
    //        _canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
    //    }
    //    else
    //    {
    //        // if not in "open" state, make the menu not interactable
    //        _canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
    //    }
    //}

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

    public void LoadBuildMenuContent(BuildMenuTab buildMenuTab)
    {
        // TODO: add GOs for all items for the current build menu tab

        // Currently just load test Room1 and Hallway
        GameObject item = Instantiate(BuildItemPrefab, BuildItemsContainer.transform);
        item.name = "Room1";
        BuildItemTile buildItemTile = item.GetComponent<BuildItemTile>();
        
        buildItemTile.Setup("Room1", "This room is just for testing");
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
    public void RemoveBuildMenuContent()
    {
        IEnumerator updateGrid = RemoveBuildMenuContentRoutine();
        StartCoroutine(updateGrid);
    }

    public IEnumerator RemoveBuildMenuContentRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (Transform child in BuildItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
