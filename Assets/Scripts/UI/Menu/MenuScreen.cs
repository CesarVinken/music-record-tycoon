
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    public GameObject MainMenuPrefab;

    public void Update()
    {
        if(GameManager.Instance != null)
        {
            if (!PointerHelper.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0) ||
                    Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    CloseMenu();
                }
            }
        }
    }

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

    public void CloseMenu()
    {
        if (MainMenuCanvas.Instance == null)
            Logger.Error(Logger.UI, "Cannot find MainMenuCanvas");

        MainMenuCanvas.Instance.CloseMainMenu();
    }
}
