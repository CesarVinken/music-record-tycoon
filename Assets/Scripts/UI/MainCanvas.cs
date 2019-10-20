using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance;

    public GameObject ConfirmationModalPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(ConfirmationModalPrefab, "ConfirmationModalPrefab");
    }
}
