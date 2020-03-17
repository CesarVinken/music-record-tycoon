using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionButton : MonoBehaviour
{
    public Text InteractionOptionText;

    public ObjectInteraction ObjectInteraction;

    public void Awake()
    {
        if (InteractionOptionText == null)
            Logger.Log(Logger.Initialisation, "Could not find InteractionOptionText component on ObjectInteractionOption");
    }

    public void Initialise(ObjectInteraction objectInteraction)
    {
        ObjectInteraction = objectInteraction;

        InteractionOptionText.text = objectInteraction.Name;
    }

    public async void Run()
    {
        OnScreenTextContainer.Instance.DeleteObjectInteractionTextContainer();
        Logger.Log("Spawn text {0}", ObjectInteraction.Reaction);
        GameObject interactionSequenceLine = OnScreenTextContainer.Instance.CreateInteractionSequenceLine(ObjectInteraction);
        await Task.Delay(3000);
        if(interactionSequenceLine != null)
            OnScreenTextContainer.Instance.DeleteInteractionSequenceLine(interactionSequenceLine);
        return;
    }
}
