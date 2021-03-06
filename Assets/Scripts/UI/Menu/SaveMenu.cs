﻿using System.IO;
using UnityEngine;

public class SaveMenu : MenuScreen
{
    //public List<SaveGameSlot> SaveSlots = new List<SaveGameSlot>();
    public GameObject SaveSlotPrefab;
    public GameObject EmptySlotPrefab;

    public void Awake()
    {
        Guard.CheckIsNull(MainMenuPrefab, "MainMenuPrefab");
        Guard.CheckIsNull(SaveSlotPrefab, "SaveSlotPrefab");
        Guard.CheckIsNull(EmptySlotPrefab, "EmptySlotPrefab");

        FileInfo[] files = GetSaveSlotList();

        DisplaySaveSlots(files);
    }

    public FileInfo[] GetSaveSlotList()
    {
        Logger.Log(Logger.Datareading, "Get saves slot list");

        //if (!Directory.Exists(Application.dataPath + "/Saves"))
        //{
        //    Directory.CreateDirectory(Application.dataPath + "/Saves");
        //    Logger.Log("created Saves folder");
        //}

        //DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Saves");
        //int count = dir.GetFiles().Length;
        ////Logger.Log("number of files: " + count + " at " + dir);
        //return dir.GetFiles("*.json");
        return null;
    }

    public void DisplaySaveSlots(FileInfo[] files)
    {
        Logger.Log(Logger.UI, "Display save slots");
        //for (int i = 0; i < files.Length; i++)
        //{
        //    string fileName = files[i].Name;
        //    if (fileName == "newGame.json")
        //        continue;
        //    GameObject slot = Instantiate(SaveSlotPrefab, transform);
        //    SaveGameSlot slotComponent = slot.GetComponent<SaveGameSlot>();
        //    SaveSlots.Add(slotComponent);

        //    string[] parts = fileName.Split('-');
        //    string locationName = parts[1];
        //    slot.name = "Save slot " + (i + 1);
        //    slotComponent.Text.text = "Save slot " + (i + 1) + " - Saved at " + locationName + " on " + parts[2] + "-" + parts[3] + "-" + parts[4] + " at " + parts[5] + ":" + parts[6];

        //}

        //GameObject emptySlot = Instantiate(EmptySlotPrefab, transform);
    }
}
