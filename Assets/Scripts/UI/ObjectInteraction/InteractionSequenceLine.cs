using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSequenceLine : MonoBehaviour
{
    public Text Text;

    public void Awake()
    {
        if (Text == null)
            Logger.Error(Logger.Initialisation, "Cannot find text component on InteractionSequenceLine");
    }
    public void Initialise(string reaction)
    {
        Text.text = reaction;
    }
}
