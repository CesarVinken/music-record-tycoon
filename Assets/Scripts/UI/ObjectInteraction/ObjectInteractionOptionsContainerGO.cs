
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractionOptionsContainerGO : MonoBehaviour
{
    public GameObject InteractionOptionPrefab;
    public Text Title;

    void Awake()
    {
        if (Title == null)
            Logger.Error(Logger.Initialisation, "could not find Title");

        Guard.CheckIsNull(InteractionOptionPrefab, "InteractionOptionPrefab");
    }

}
