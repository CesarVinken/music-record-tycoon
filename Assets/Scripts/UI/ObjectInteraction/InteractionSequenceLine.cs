using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSequenceLine : MonoBehaviour
{
    public Text Text;
    private Character _interactingCharacter;

    public void Awake()
    {
        if (Text == null)
            Logger.Error(Logger.Initialisation, "Cannot find text component on InteractionSequenceLine");
    }

    public void Update()
    {
        Vector2 interactingCharacterPosition = _interactingCharacter ? _interactingCharacter.transform.position : transform.position;
        Vector2 textPosition = Camera.main.WorldToScreenPoint(interactingCharacterPosition);
        transform.position = textPosition;
    }

    public void Initialise(string reaction, Character interactingCharacter)
    {
        Text.text = reaction;
        _interactingCharacter = interactingCharacter;
    }
}
