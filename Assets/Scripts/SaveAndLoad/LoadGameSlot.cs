using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameSlot : MonoBehaviour
{
    //public Text Text;
    //public FileInfo SaveFile;

    public void LoadGame()
    {
        Debug.Log("Load game. To be implemented.");
        //    GameDataManager.LoadedSaveFile = LoadSaveFile();
        //    GameManager.GameStarted = false;
        //    SceneManager.LoadScene(GameDataManager.LoadedSaveFile.CurrentScene);
        //}

        //private SaveFileData LoadSaveFile()
        //{
        //    string filePath = Application.dataPath + "/Saves/" + SaveFile.Name;

        //    if (File.Exists(filePath))
        //    {
        //        string dataAsJson = File.ReadAllText(filePath);
        //        return JsonUtility.FromJson<SaveFileData>(dataAsJson);
        //    }
        //    else
        //    {
        //        Debug.LogError("Could not find the save file");
        //        return null;
        //    }
    }
}