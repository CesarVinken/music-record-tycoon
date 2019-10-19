using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadMenu : MenuScreen
{ 
    //public List<LoadGameSlot> SaveSlots = new List<LoadGameSlot>();
    public GameObject SaveSlotPrefab;
    public GameObject NoSavedGames;

    void Awake ()
    {
        Guard.CheckIsNull(SaveSlotPrefab, "MainMenuPrefab");
        Guard.CheckIsNull(MainMenuPrefab, "SaveSlotPrefab");
        Guard.CheckIsNull(NoSavedGames, "NoSavedGames");

        //MainMenuCanvas.Instance.PauseMenu = gameObject;
        FileInfo[] files = GetSaveSlotList();

        DisplaySaveSlots(files);
    }

    public FileInfo[] GetSaveSlotList()
    {
        Debug.Log("Get saves slot list");

        //if (!Directory.Exists(Application.dataPath + "/Saves"))
        //{
        //    Directory.CreateDirectory(Application.dataPath + "/Saves");
        //    Debug.Log("created Saves folder");
        //}

        //DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Saves");
        //int count = dir.GetFiles().Length;
        ////Debug.Log("number of files: " + count + " at " + dir);
        //return dir.GetFiles("*.json");
        return null;
    }

    public void DisplaySaveSlots(FileInfo[] files)
    {
        Debug.Log("Display save slots");

        //if(files.Length <= 1)   //1 file because there is a new Game.json
        //{
        //    NoSavedGames.SetActive(true);
        //    return;
        //}
        //for (int i = 0; i < files.Length; i++)
        //{
        //    string fileName = files[i].Name;
        //    if (fileName == "newGame.json")
        //        continue;

        //    GameObject slot = Instantiate(SaveSlotPrefab, transform);
        //    LoadGameSlot slotComponent = slot.GetComponent<LoadGameSlot>();
        //    SaveSlots.Add(slotComponent);
        //    string[] parts = fileName.Split('-');
        //    string locationName = parts[1];
        //    slot.name = "Save slot " + (i + 1);
        //    slotComponent.Text.text = "Save slot " + (i + 1) + " - Saved at " + locationName + " on " + parts[2] + "-" + parts[3] + "-" + parts[4] + " at " + parts[5] + ":" + parts[6];
        //    slotComponent.SaveFile = files[i];
        //}
    }
}
