using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NewGameSlot : MonoBehaviour
{
    //private Vector2 _initialPlayerPos = new Vector2(-16.5f, -20.3f);

    //public void NewGame()
    //{
    //    Logger.Log("Click button");
    //    GameDataManager.LoadedSaveFile = LoadNewGameFile();
    //    Logger.Log(GameDataManager.LoadedSaveFile.PlayerLocation);
    //    GameManager.GameStarted = false;
    //    SceneManager.LoadScene("Up");
    //}

    //private SaveFileData LoadNewGameFile()
    //{
    //    if (!Directory.Exists(Application.dataPath + "/Saves"))
    //    {
    //        Directory.CreateDirectory(Application.dataPath + "/Saves");
    //        Logger.Log("created Saves folder");
    //    }

    //    string filePath = Application.dataPath + "/Saves/newGame.json";

    //    if (File.Exists(filePath))
    //    {
    //        string dataAsJson = File.ReadAllText(filePath);
    //        return JsonUtility.FromJson<SaveFileData>(dataAsJson);
    //    }
    //    else
    //    {
    //        Logger.LogError("Could not find newGame.json. We need to generate the New Game file.");
    //        SaveFileData saveFileData = GenerateNewGameFile();
    //        return saveFileData;
    //    }
    //}

    //private SaveFileData GenerateNewGameFile()
    //{
    //    GameDataManager.LoadGameDataFromFile();

    //    SaveFileData saveFile = new SaveFileData
    //    {
    //        CurrentScene = "Up",
    //        InventoryItems = new List<int>(),
    //        PlayerLocation = _initialPlayerPos
    //    };

    //    for (int i = 0; i < GameDataManager.GameEventsRegister.Length; i++)
    //    {
    //        saveFile.GameEventStatuses.Add(new DictionaryToJsonItem(GameDataManager.GameEventsRegister[i].Id, GameDataManager.GameEventsRegister[i].DefaultState));
    //    }

    //    for (int j = 0; j < GameDataManager.DialogueSeriesRegister.Length; j++)
    //    {
    //        saveFile.DialogueSeriesPassed.Add(new DictionaryToJsonItem(GameDataManager.DialogueSeriesRegister[j].Id, GameDataManager.DialogueSeriesRegister[j].Passed));
    //    }

    //    for (int k = 0; k < GameDataManager.InteractableObjectsRegister.Length; k++)
    //    {
    //        saveFile.InteractableObjectsInScene.Add(new DictionaryToJsonItem(GameDataManager.InteractableObjectsRegister[k].Id, GameDataManager.InteractableObjectsRegister[k].InScene));
    //    }

    //    string filePath = Application.dataPath + "/Saves/newGame.json";

    //    string dataAsJson = JsonUtility.ToJson(saveFile);
    //    File.WriteAllText(filePath, dataAsJson);
    //    Logger.Log("Created newGame file.");

    //    return saveFile;
    //}
}
