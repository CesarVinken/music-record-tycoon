using UnityEngine;
using UnityEngine.UI;

public class InteractionSequenceLineGO : MonoBehaviour
{
    public Text Text;
    private Character _interactingCharacter = null;
    private Vector2 _fallbackPosition;

    public void Awake()
    {
        if (Text == null)
            Logger.Error(Logger.Initialisation, "Cannot find text component on InteractionSequenceLine");
    }

    public void Update()
    {
        Vector2 interactingCharacterPosition = _interactingCharacter != null ? _interactingCharacter.transform.position : new Vector3(_fallbackPosition.x, _fallbackPosition.y, 0);
        Vector2 textPosition = Camera.main.WorldToScreenPoint(interactingCharacterPosition);
        transform.position = textPosition;
    }

    public void Initialise(InteractionSequenceLine sequenceLine, Vector2 linePosition, Character interactingCharacter)
    {
        Text.text = sequenceLine.Line;
        _fallbackPosition = linePosition;

        if (interactingCharacter != null)
        {
            _interactingCharacter = interactingCharacter;
            transform.position = interactingCharacter.transform.position;
        }
        else
        {
            transform.position = linePosition;
        }
    }
}
