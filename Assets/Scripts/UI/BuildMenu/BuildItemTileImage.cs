using UnityEngine;
using UnityEngine.EventSystems;

public class BuildItemTileImage : MonoBehaviour, IPointerClickHandler
{
    public BuildItemTile BuildItemTile;

    public void Awake()
    {
        Guard.CheckIsNull(BuildItemTile, "BuildItemTile");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BuilderManager.Instance.SetSelectedRoom(new RoomBlueprint());     // TODO Will later NOT always be the same room

        Sprite roomIcon = Resources.Load<Sprite>("Icons/Room1");
        RectTransform rectTransform = (RectTransform)transform;
        MainCanvas.Instance.SetPointerImage(roomIcon, new Vector2(rectTransform.rect.width, rectTransform.rect.height));

        BuilderManager.Instance.DrawAvailablePlots();
    }
}
