using UnityEngine;
using UnityEngine.UI;

public enum PointerImageOverlayState
{
    Normal,
    Red
}
public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public GameObject PointerImageGO;
    public GameObject TriggersContainer;
    public Image PointerImage;
    public bool IsDraggingIcon;

    public GameObject NotificationPrefab;
    public GameObject ConfirmationModalPrefab;
    public GameObject AvatarContainer;
    public GameObject ConsoleContainer;
    public GameObject OnScreenTextContainer;
    public GameObject TimeDisplayContainer;

    private float _currentSnappedX;
    private float _currentSnappedY;
    private ObjectRotation _rotationRoomOnLastHover = ObjectRotation.Rotation0;
    private PointerImageOverlayState _currentPointerImageOverlayState = PointerImageOverlayState.Normal;

    public static Vector2 TileSizeInUnits = new Vector2(30f, 15f);

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(PointerImageGO, "PointerImageGO");
        Guard.CheckIsNull(TriggersContainer, "TriggersContainer");
        Guard.CheckIsNull(AvatarContainer, "AvatarContainer");
        Guard.CheckIsNull(ConsoleContainer, "ConsoleContainer");
        Guard.CheckIsNull(OnScreenTextContainer, "OnScreenTextContainer");
        Guard.CheckIsNull(TimeDisplayContainer, "TimeDisplayContainer");

        Guard.CheckIsNull(NotificationPrefab, "NotificationPrefab");
        Guard.CheckIsNull(ConfirmationModalPrefab, "ConfirmationModalPrefab");

        PointerImage.sprite = null;
        PointerImage.enabled = false;
        PointerImage.raycastTarget = false;
        IsDraggingIcon = false;
    }

    public void Update()
    {
        if (Console.Instance && Console.Instance.ConsoleState == ConsoleState.Large)
            return;

        if (PointerImage.sprite != null)
        {
            Vector2 mousePosition = Input.mousePosition;
            bool isPointerOverGameObject = PointerHelper.IsPointerOverGameObject();
            if (isPointerOverGameObject && BuildMenuContainer.Instance.IsOpen)
            {
                PointerImageGO.transform.position = new Vector2(mousePosition.x, mousePosition.y);
            }
            else
            {
                if(BuildMenuContainer.Instance.IsOpen)
                {
                    Logger.Log(Logger.UI, "Close build menu");
                    BuildMenuContainer.Instance.ActivateAnimationFreeze();

                    BuildMenuContainer.Instance.IsOpen = false;
                    BuildMenuContainer.Instance.RemoveBuildMenuContent(0.5f);
                }
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                mouseWorldPosition = new Vector2(mouseWorldPosition.x, mouseWorldPosition.y + 7.5f);

                float xx = Mathf.Round((mouseWorldPosition.y) / TileSizeInUnits.y + (mouseWorldPosition.x) / TileSizeInUnits.x);
                float yy = Mathf.Round((mouseWorldPosition.y) / TileSizeInUnits.y - (mouseWorldPosition.x) / TileSizeInUnits.x) - 1;

                // Calculate grid aligned position from current position
                float snappedX = (xx - yy) * 0.5f * TileSizeInUnits.x;
                float snappedY = (xx + yy) * 0.5f * TileSizeInUnits.y;

                if (_currentSnappedX != snappedX || _currentSnappedY != snappedY)
                {
                    _currentSnappedX = snappedX;
                    _currentSnappedY = snappedY;

                    if (BuilderManager.Instance.BuildingPlotLocations.ContainsKey(new Vector2(_currentSnappedX, _currentSnappedY)))
                    {
                        Vector2 availablePlotVectorPosition = BuilderManager.Instance.BuildingPlotLocations[new Vector2(_currentSnappedX, _currentSnappedY)];
                        if (BuildingPlot.AvailablePlotVectorPosition == availablePlotVectorPosition && BuilderManager.PointerIsOnAvailablePlot) return;

                        BuildingPlot.AvailablePlotVectorPosition = availablePlotVectorPosition;

                        SetPointerImageOverlayColor(PointerImageOverlayState.Normal);
                        BuilderManager.PointerIsOnAvailablePlot = true;
                        BuildingPlot buildingPlot = BuilderManager.Instance.BuildingPlots[BuildingPlot.AvailablePlotVectorPosition];

                        Sprite roomIcon = GetRoomIcon(buildingPlot.RoomBlueprint.Name, buildingPlot.PlotRotation);
                        SetPointerImage(roomIcon, buildingPlot.PlotRotation);

                        RepositionImage();
                    }
                    else
                    {
                        if (BuilderManager.PointerIsOnAvailablePlot)
                        {
                            BuilderManager.PointerIsOnAvailablePlot = false;
                        }
                        if(_currentPointerImageOverlayState == PointerImageOverlayState.Normal)
                        {
                            SetPointerImageOverlayColor(PointerImageOverlayState.Red);
                        }

                        RepositionImage();
                    }
                }      
            }

            if(GameManager.Instance.CurrentPlatform == Platform.PC)
            {
                if (BuildMenuContainer.Instance.PanelAnimationPlaying) return;

                if (Input.GetMouseButtonDown(1))
                {
                    UnsetPointerImage();

                    if (!BuildMenuContainer.Instance.IsOpen)
                    {
                        BuildMenuContainer.Instance.ActivateAnimationFreeze();
                        BuilderManager.Instance.ActivateBuildMenuMode();
                    }

                }
            }
        }
    }


    public void SetPointerImage(Sprite sprite, ObjectRotation rotation)
    {
        SetPointerImageScale(rotation);
        SetPointerImageSize(PointerImage);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;

        IsDraggingIcon = true;
        _rotationRoomOnLastHover = rotation;
    }

    public void SetNewPointerImage(Sprite sprite, Vector2 pointerImageProportions)
    {
        SetPointerImageSize(PointerImage, pointerImageProportions);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;

        IsDraggingIcon = true;

        SetPointerImageOverlayColor(PointerImageOverlayState.Normal);
    }

    public void UnsetPointerImage()
    {
        PointerImage.enabled = false;
        PointerImage.sprite = null;

        IsDraggingIcon = false;
    }

    public void SetPointerImageOverlayColor(PointerImageOverlayState overlayState)
    {
        _currentPointerImageOverlayState = overlayState;
        switch (overlayState)
        {
            case PointerImageOverlayState.Normal:
                PointerImage.color = new Color(1, 1, 1, 1);
                break;
            case PointerImageOverlayState.Red:
                PointerImage.color = new Color(1, .2f, .2f, .7f);
                break;
            default:
                Logger.Error(Logger.UI, "State {0} was not yet implemented", overlayState);
                break;
        }
    }

    // NOTE: Make sure icon is rectengular and has the room starting at the bottom
    public Vector2 GetPointerImageSize(RoomBlueprint blueprint, Image pointerImage)
    {
        float tileLength = 5f;   // distance to go horizontal or vertical to get to next tile row or column
        float roomWidth = tileLength * (blueprint.LeftUpAxisLength + blueprint.RightUpAxisLength); // the width in tiles on the map of a room

        float defaultScreenFactor = 1280;
        float pixelMultiplicationFactor = defaultScreenFactor / Screen.width;
        float roomScreenWidth = (Camera.main.WorldToScreenPoint(new Vector2(roomWidth * pixelMultiplicationFactor, 0)) - Camera.main.WorldToScreenPoint(new Vector2(0, 0))).x; // the width on the screen of the blueprint. The hover image should have this width as well. 

        return new Vector2(roomScreenWidth, roomScreenWidth);
    }

    public void SetPointerImageSize(Image pointerImage)
    {
        pointerImage.rectTransform.sizeDelta = GetPointerImageSize(BuilderManager.Instance.SelectedRoom, pointerImage);
    }

    private void SetPointerImageSize(Image pointerImage, Vector2 pointerImageProportions)
    {
        pointerImage.rectTransform.sizeDelta = pointerImageProportions;
    }

    private void SetPointerImageScale(ObjectRotation rotation)
    {
        if (rotation == ObjectRotation.Rotation90 || rotation == ObjectRotation.Rotation270)
            PointerImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        else
            PointerImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    public static Sprite GetRoomIcon(string name, ObjectRotation rotation)
    {
        string roomName = name.Replace(" ", "");

        if (rotation == ObjectRotation.Rotation0 || rotation == ObjectRotation.Rotation90)
        {
            Sprite roomIcon = Resources.Load<Sprite>("Icons/Rooms/" + roomName + "Rotation0");
            if (roomIcon == null)
            {
                Logger.Error(Logger.Building, "Could not find or load icon for {0}-{1}", name, rotation);
                return Resources.Load<Sprite>("Icons/Rooms/Room1Rotation0");
            }
            return roomIcon;
        } else
        {
            Sprite roomIcon = Resources.Load<Sprite>("Icons/Rooms/" + roomName + "Rotation180");
            if (roomIcon == null)
            {
                Logger.Error(Logger.Building, "Could not find or load icon for {0}-{1}", name, rotation);
                return Resources.Load<Sprite>("Icons/Rooms/Room1Rotation0");
            }
            return roomIcon;
        }
    }

    public void RepositionImage()
    {
        Vector2 imageOffset = getPointerImageOffset(BuilderManager.Instance.SelectedRoom);
        PointerImageGO.transform.position = Camera.main.WorldToScreenPoint(new Vector2(
            _currentSnappedX - imageOffset.x, _currentSnappedY + imageOffset.y));
    }

    private Vector2 getPointerImageOffset(RoomBlueprint blueprint)
    {
        Vector2 offset = new Vector2(0, 0);

        switch (blueprint.RoomName)
        {
            case RoomName.Hallway:
                offset.x = TileSizeInUnits.x / 2f;
                offset.y = TileSizeInUnits.y / 2f;
                break;
            case RoomName.Room1:
                offset.x = TileSizeInUnits.x / 4f;
                if(_rotationRoomOnLastHover == ObjectRotation.Rotation0 || _rotationRoomOnLastHover == ObjectRotation.Rotation180) {
                    offset.y = TileSizeInUnits.y;
                } else
                {
                    offset.y = TileSizeInUnits.y * 1.5f;
                }
                break;
            case RoomName.RecordingStudio1:
                offset.x = TileSizeInUnits.x / 4f;
                if (_rotationRoomOnLastHover == ObjectRotation.Rotation0 || _rotationRoomOnLastHover == ObjectRotation.Rotation180)
                {
                    offset.y = TileSizeInUnits.y * 0.5f;
                }
                else
                {
                    offset.y = TileSizeInUnits.y * 1f;
                }
                break;
            default:
                Logger.Warning("Image offset not implemented for {0}", blueprint.RoomName);
                break;
        }
        return offset;
    }
}
