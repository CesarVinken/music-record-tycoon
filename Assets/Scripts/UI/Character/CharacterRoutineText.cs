using UnityEngine;
using UnityEngine.UI;

public class CharacterRoutineText : MonoBehaviour
{
    public Text Text;

    [SerializeField]
    private Character _character;

    private Transform _characterTransform;

    public void Awake()
    {
        if (!Text)
            Logger.Error(Logger.Initialisation, "Cannot find text component");
    }

    public void Update()
    {
        Vector2 interactingCharacterPosition = _characterTransform.position;
        Vector2 textPosition = Camera.main.WorldToScreenPoint(new Vector2(interactingCharacterPosition.x, interactingCharacterPosition.y + 8.5f));
        transform.position = textPosition;
    }

    public void Initialise(Character character)
    {
        _character = character;
        _characterTransform = _character.transform;
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
