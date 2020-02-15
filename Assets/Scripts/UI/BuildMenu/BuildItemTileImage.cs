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
            BuilderManager.Instance.DeleteAllTriggers();

            RoomBlueprint blueprint = BuildItemTile.BuildItemBlueprint as RoomBlueprint;
            BuilderManager.Instance.SetSelectedRoom(blueprint);
            Logger.Log("Selected: {0}", blueprint.Name);
            Sprite roomIcon = MainCanvas.GetRoomIcon(blueprint.Name, RoomRotation.Rotation0);

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
                BuilderManager.Instance.DeleteAllTriggers();

                RoomBlueprint blueprint = BuildItemTile.BuildItemBlueprint as RoomBlueprint;
                BuilderManager.Instance.SetSelectedRoom(blueprint);

                Sprite roomIcon = MainCanvas.GetRoomIcon(blueprint.Name, RoomRotation.Rotation0);

                RectTransform rectTransform = (RectTransform)transform;
                MainCanvas.Instance.SetPointerImage(roomIcon, new Vector2(rectTransform.rect.width, rectTransform.rect.height));

                BuilderManager.Instance.DrawAvailablePlots();
            }
        }
    }
}

