
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    public GameObject MainMenuPrefab;

    public void Back()
    {
        InstantiatePanel(MainMenuPrefab);
    }

    public void InstantiatePanel(GameObject panelPrefab)
    {
        GameObject LoadPanel = Instantiate(panelPrefab, MainMenuCanvas.Instance.transform);
        MainMenuCanvas.Instance.SetCurrentMenuPanel(LoadPanel);
        Destroy(gameObject);
    }
}
