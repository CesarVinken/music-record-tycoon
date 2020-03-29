using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarTile : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    const string BASE_IMAGE_PATH = "Icons/Avatars/";
    public Image AvatarImage;
    public Character Character;

    public GameObject SelectionMarker;

    public void Awake()
    {
        Guard.CheckIsNull(SelectionMarker, "SelectedMarker");
        if (AvatarImage == null)
            Logger.Log(Logger.Initialisation, "Could not find AvatarImage image component");
    }

    public void Setup(Character character)
    {
        Character = character;

        if (Character.Gender == Gender.Male)
            AvatarImage.sprite = CharacterManager.Instance.AvatarsMale.Single(s => s.name == character.Image);
        if (Character.Gender == Gender.Female)
            AvatarImage.sprite = CharacterManager.Instance.AvatarsFemale.Single(s => s.name == character.Image);
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
