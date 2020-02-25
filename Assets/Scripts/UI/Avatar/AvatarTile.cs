using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarTile : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public Image AvatarImage;
    public PlayableCharacter Character;

    public GameObject SelectionMarker;

    public void Awake()
    {
        Guard.CheckIsNull(SelectionMarker, "SelectedMarker");
        if (AvatarImage == null)
            Logger.Log(Logger.Initialisation, "Could not find AvatarImage image component");
    }

    public void Setup(PlayableCharacter character)
    {
        Character = character;
        // set sprite for character
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CharacterManager.Instance.SelectedCharacter.Id == Character.Id) return;

        CharacterManager.Instance.SelectCharacter(Character);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TODO delete funtion?
    }

    public void EnableSelectionMarker()
    {
        SelectionMarker.SetActive(true);
    }

    public void DisableSelectionMarker()
    {
        SelectionMarker.SetActive(false);
    }
}
