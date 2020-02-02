using UnityEngine;
using UnityEngine.EventSystems;

public class BuildItemTileImage : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public BuildItemTile BuildItemTile;

    public void Awake()
    {
        Guard.CheckIsNull(BuildItemTile, "BuildItemTile");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
            RoomBlueprint blueprint = BuildItemTile.BuildItemBlueprint as RoomBlueprint;
            BuilderManager.Instance.SetSelectedRoom(blueprint);

            Sprite roomIcon = Resources.Load<Sprite>("Icons/" + blueprint.Name);
            if(roomIcon == null)
            {
                Logger.Error(Logger.Building, "Could not find or load icon for {0}", blueprint.Name);
            }
            RectTransform rectTransform = (RectTransform)transform;
            MainCanvas.Instance.SetPointerImage(roomIcon, new Vector2(rectTransform.rect.width, rectTransform.rect.height));

            BuilderManager.Instance.DrawAvailablePlots();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentPlatform == Platform.Android)
        {
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                RoomBlueprint blueprint = BuildItemTile.BuildItemBlueprint as RoomBlueprint;
                BuilderManager.Instance.SetSelectedRoom(blueprint);

                Sprite roomIcon = Resources.Load<Sprite>("Icons/" + blueprint.Name);
                if (roomIcon == null)
                {
                    Logger.Error(Logger.Building, "Could not find or load icon for {0}", blueprint.Name);
                }
                RectTransform rectTransform = (RectTransform)transform;
                MainCanvas.Instance.SetPointerImage(roomIcon, new Vector2(rectTransform.rect.width, rectTransform.rect.height));

                BuilderManager.Instance.DrawAvailablePlots();
            }
        }
    }
}

