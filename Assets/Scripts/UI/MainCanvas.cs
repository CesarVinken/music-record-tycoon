using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public GameObject PointerImageGO;
    public GameObject TriggersContainer;
    public Image PointerImage;
    public bool IsDraggingIcon;

    public GameObject NotificationPrefab;
    public GameObject ConfirmationModalPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(PointerImageGO, "PointerImageGO");
        Guard.CheckIsNull(TriggersContainer, "TriggersContainer");

        Guard.CheckIsNull(NotificationPrefab, "NotificationPrefab");
        Guard.CheckIsNull(ConfirmationModalPrefab, "ConfirmationModalPrefab");

        PointerImage.sprite = null;
        PointerImage.enabled = false;
        PointerImage.raycastTarget = false;
        IsDraggingIcon = false;
    }

    public void Update()
    {
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
                Vector2 tileSizeInUnits = new Vector2(30f, 15f);

                float xx = Mathf.Round(mouseWorldPosition.y / tileSizeInUnits.y + mouseWorldPosition.x / tileSizeInUnits.x);
                float yy = Mathf.Round(mouseWorldPosition.y / tileSizeInUnits.y - mouseWorldPosition.x / tileSizeInUnits.x);

                // Calculate grid aligned position from current position
                float snappedX = (xx - yy) * 0.5f * tileSizeInUnits.x;
                float snappedY = (xx + yy) * 0.5f * tileSizeInUnits.y;

                PointerImageGO.transform.position = Camera.main.WorldToScreenPoint(new Vector2(snappedX - 8f, snappedY + .6f));

                if (BuilderManager.Instance.BuildingPlotLocations.ContainsKey(new Vector2(snappedX, snappedY)))
                {
                    Vector2 availablePlotVectorPosition = BuilderManager.Instance.BuildingPlotLocations[new Vector2(snappedX, snappedY)];
                    if (BuildingPlot.AvailablePlotVectorPosition == availablePlotVectorPosition && BuilderManager.PointerIsOnAvailablePlot) return;

                    BuildingPlot.AvailablePlotVectorPosition = availablePlotVectorPosition;

                    SetPointerImageOverlayColor(new Color(1, 1, 1, 1));
                    BuilderManager.PointerIsOnAvailablePlot = true;
                    BuildingPlot buildingPlot = BuilderManager.Instance.BuildingPlots[BuildingPlot.AvailablePlotVectorPosition];
                    Logger.Log("Rotation of building plot::" + buildingPlot.PlotRotation);

                    Sprite roomIcon = GetRoomIcon(buildingPlot.RoomBlueprint.Name, buildingPlot.PlotRotation);
                    SetPointerImage(roomIcon, buildingPlot.PlotRotation);
                }
                else
                {
                    SetPointerImageOverlayColor(new Color(1, .2f, .2f, .7f));
                    BuilderManager.PointerIsOnAvailablePlot = false;
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


    public void SetPointerImage(Sprite sprite, RoomRotation rotation)
    {
        SetPointerImageScale(rotation);
        SetPointerImageSize(PointerImage);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;

        IsDraggingIcon = true;
    }

    public void SetPointerImage(Sprite sprite, Vector2 pointerImageProportions)
    {
        SetPointerImageSize(PointerImage, pointerImageProportions);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;

        IsDraggingIcon = true;
    }

    public void UnsetPointerImage()
    {
        PointerImage.enabled = false;
        PointerImage.sprite = null;

        IsDraggingIcon = false;
    }

    public void SetPointerImageOverlayColor(Color color)
    {
        // TODO currently colour change is requested every frame!
        PointerImage.color = color;
    }

    public Vector2 GetPointerImageSize(RoomBlueprint blueprint, Image pointerImage)
    {
        float TileUnitWidth = 5f;   // the x width of one side (such as right up)
        float unitPixelWidth = (Camera.main.WorldToScreenPoint(new Vector2(TileUnitWidth * 0.66f, 0)) - Camera.main.WorldToScreenPoint(new Vector2(0, 0))).x;
        float fullIconWidth = unitPixelWidth * (blueprint.LeftUpAxisLength + blueprint.RightUpAxisLength); // the width on the screen of the blue print. The hover image should have this width as well. 
                                                                                                                   // BuilderManager.Instance.SelectedRoom.RightUpAxisLength // <-- To get length of room should later on generic like this!
        return new Vector2(fullIconWidth, fullIconWidth);
    }

    public void SetPointerImageSize(Image pointerImage)
    {
        pointerImage.rectTransform.sizeDelta = GetPointerImageSize(BuilderManager.Instance.SelectedRoom, pointerImage);
    }

    private void SetPointerImageSize(Image pointerImage, Vector2 pointerImageProportions)
    {
        pointerImage.rectTransform.sizeDelta = pointerImageProportions;
    }

    private void SetPointerImageScale(RoomRotation rotation)
    {
        Logger.Log(":::: ROTIAON {0}", rotation);
        if (rotation == RoomRotation.Rotation90 || rotation == RoomRotation.Rotation270)
        {
            PointerImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        } else
        {
            PointerImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public static Sprite GetRoomIcon(string name, RoomRotation rotation)
    {
        if (rotation == RoomRotation.Rotation0 || rotation == RoomRotation.Rotation90)
        {
            Sprite roomIcon = Resources.Load<Sprite>("Icons/Rooms/" + name + "Rotation0");
            if (roomIcon == null)
            {
                Logger.Error(Logger.Building, "Could not find or load icon for {0}-{1}", name, rotation);
                return Resources.Load<Sprite>("Icons/Rooms/Room1Rotation0");
            }
            return roomIcon;
        } else
        {
            Sprite roomIcon = Resources.Load<Sprite>("Icons/Rooms/" + name + "Rotation180");
            if (roomIcon == null)
            {
                Logger.Error(Logger.Building, "Could not find or load icon for {0}-{1}", name, rotation);
                return Resources.Load<Sprite>("Icons/Rooms/Room1Rotation0");
            }
            return roomIcon;
        }
    }
}
