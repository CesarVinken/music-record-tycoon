using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionButton : MonoBehaviour
{
    public Text InteractionOptionText;

    public List<ObjectInteractionOptionButton> ObjectInteractionOptions = new List<ObjectInteractionOptionButton>();

    public void Awake()
    {
        if (InteractionOptionText == null)
            Logger.Log(Logger.Initialisation, "Could not find InteractionOptionText component on ObjectInteractionOption");
    }

    public void Initialise(ObjectInteraction objectInteraction)
    {
        InteractionOptionText.text = objectInteraction.Name;
    }
}
