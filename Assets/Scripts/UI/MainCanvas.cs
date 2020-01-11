using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public GameObject ConfirmationModalPrefab;
    public GameObject PointerImageGO;
    public Image PointerImage;
    public bool IsDraggingIcon;
    //public float IconFrameHeight = 800;
    //public float IconFrameWidth = 800;

    private float _imageHeight;
    private float _imageWidth;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConfirmationModalPrefab, "ConfirmationModalPrefab");
        Guard.CheckIsNull(PointerImageGO, "PointerImageGO");

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
            if (EventSystem.current.IsPointerOverGameObject() && BuildMenuContainer.Instance.IsOpen)
            {
                PointerImageGO.transform.position = new Vector2(mousePosition.x, mousePosition.y);
            }
            else
            {
                if(BuildMenuContainer.Instance.IsOpen)
                {
                    Logger.Log(Logger.UI, "Close build menu");
                    BuildMenuContainer.Instance.IsOpen = false;
                    BuildMenuContainer.Instance.RemoveBuildMenuContent();
                }
                Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector2 tileSizeInUnits = new Vector2(30f, 15f);

                float xx = Mathf.Round(mouseWorldPosition.y / tileSizeInUnits.y + mouseWorldPosition.x / tileSizeInUnits.x);
                float yy = Mathf.Round(mouseWorldPosition.y / tileSizeInUnits.y - mouseWorldPosition.x / tileSizeInUnits.x);

                // Calculate grid aligned position from current position
                float snappedX = (xx - yy) * 0.5f * tileSizeInUnits.x;
                float snappedY = (xx + yy) * 0.5f * tileSizeInUnits.y;

                PointerImageGO.transform.position = Camera.main.WorldToScreenPoint(new Vector2(snappedX - 8f, snappedY + .6f));
            }

            if (Input.GetMouseButtonDown(1))
            {
                UnsetPointerImage();
            }
        }
    }

    public void SetPointerImage(Sprite sprite)
    {
        SetPointerImageSize(PointerImage);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;
    }

    public void SetPointerImage(Sprite sprite, Vector2 pointerImageProportions)
    {
        SetPointerImageSize(PointerImage, pointerImageProportions);

        PointerImage.sprite = sprite;
        PointerImage.enabled = true;
        PointerImage.preserveAspect = true;
    }

    public void UnsetPointerImage()
    {
        PointerImage.enabled = false;
        PointerImage.sprite = null;
    }

    public Vector2 GetPointerImageSize(Image pointerImage)
    {
        float TileUnitWidth = 5f;   // the x width of one side (such as right up)
        float unitPixelWidth = (Camera.main.WorldToScreenPoint(new Vector2(TileUnitWidth * 0.66f, 0)) - Camera.main.WorldToScreenPoint(new Vector2(0, 0))).x;
        float fullIconWidth = unitPixelWidth * (RoomBlueprint.LeftUpAxisLength + RoomBlueprint.RightUpAxisLength); // the width on the screen of the blue print. The hover image should have this width as well. 
                                                                                                                   // BuilderManager.Instance.SelectedRoom.RightUpAxisLength // <-- To get length of room should later on generic like this!
        return new Vector2(fullIconWidth, fullIconWidth);
    }

    public void SetPointerImageSize(Image pointerImage)
    {
        pointerImage.rectTransform.sizeDelta = GetPointerImageSize(pointerImage);
    }

    private void SetPointerImageSize(Image pointerImage, Vector2 pointerImageProportions)
    {
        pointerImage.rectTransform.sizeDelta = pointerImageProportions;
    }
}
