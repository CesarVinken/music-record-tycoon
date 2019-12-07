using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameSlot : MonoBehaviour
{
    //public Text Text;

    public void SaveGame()
    {
        Logger.Log(Logger.Datawriting, "Save game. To be implemented");
        //{
        //    ScenePersistentData.StoreScenePersistentData();

        //    Logger.Log("Click save button");
        //    if (!Directory.Exists(Application.dataPath + "/Saves"))
        //    {
        //        Directory.CreateDirectory(Application.dataPath + "/Saves");
        //        Logger.Log("created Saves folder");
        //    }

        //    SaveFileData saveFile = new SaveFileData
        //    {
        //        CurrentScene = SceneNavigationManager.GetSceneName(SceneNavigationManager.CurrentLocation),
        //        PlayerLocation = new Vector2(PlayerCharacter.Instance.transform.position.x, PlayerCharacter.Instance.transform.position.y),
        //        InventoryItems = ScenePersistentData.InventoryItems
        //    };

        //    foreach (var item in ScenePersistentData.GameEventStatuses)
        //    {
        //      //  Logger.Log("add to Save event " + item.Key);
        //        saveFile.GameEventStatuses.Add(new DictionaryToJsonItem(item.Key, item.Value));
        //    }

        //    foreach (var item in ScenePersistentData.DialogueSeriesPassed)
        //    {
        //       // Logger.Log("add to Save dialogue series " + item.Key);
        //        saveFile.DialogueSeriesPassed.Add(new DictionaryToJsonItem(item.Key, item.Value));
        //    }

        //    foreach (var item in ScenePersistentData.InteractableSceneObjects)
        //    {
        //      //  Logger.Log("add to Save object " + item.Key);
        //        saveFile.InteractableObjectsInScene.Add(new DictionaryToJsonItem(item.Key, item.Value));
        //    }

        //    string date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute;
        //    string filePath = Application.dataPath + "/Saves/save-" + saveFile.CurrentScene + "-" + date + ".json";

        //    string dataAsJson = JsonUtility.ToJson(saveFile);
        //    File.WriteAllText(filePath, dataAsJson);
        //    Logger.Log("Save complete.");

        //    if (MainCanvas.Instance.PauseMenu != null)
        //        MainCanvas.Instance.TogglePauseMenu();
    }
}
