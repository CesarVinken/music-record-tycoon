﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRoomTrigger : MonoBehaviour
{
    private GameObject _confirmationModal;
    private Vector2 _midpoint = new Vector2(0, 0);
    private Room _room;

    public static List<DeleteRoomTrigger> DeleteRoomTriggers = new List<DeleteRoomTrigger>();

    public void Awake()
    {
        _confirmationModal = BuilderManager.Instance.ConfirmationModalGO;
        DeleteRoomTriggers.Add(this);
    }

    public void Setup(Room room)
    {
        _room = room;
        _midpoint = _room.RoomCorners[Direction.Down] + (_room.RoomCorners[Direction.Up] - _room.RoomCorners[Direction.Down]) / 2;
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);
    }

    public void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        if (_confirmationModal)
        {
            _confirmationModal.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        }
    }

    public void CreateDeleteRoomConfirmation()
    {
        if (_confirmationModal)
            return;

        GameObject modal = Instantiate(MainCanvas.Instance.ConfirmationModalPrefab);
        modal.transform.position = Camera.main.WorldToScreenPoint(_midpoint);
        modal.transform.SetParent(MainCanvas.Instance.transform);
        _confirmationModal = modal;
        _confirmationModal.GetComponent<ConfirmationModal>().Setup(this);
    }

    public void DeleteRoom()
    {
        Room tempRoomCopy = _room;
        _room.RemoveDoorConnectionFromAdjacentRooms();
        _room.RemoveThisRoomFromAdjacentRooms();
        _room.DeleteRoom();
        Debug.LogWarning("I AM BEING DELETED: " + tempRoomCopy.Id);
        tempRoomCopy.CleanUpDeletedRoomTiles();

        BuilderManager.Instance.UpdatePathfindingGrid();

        for (int l = 0; l < RoomManager.Rooms.Count; l++)
        {
            Debug.Log(RoomManager.Rooms[l].Id + " has " + RoomManager.Rooms[l].AdjacentRooms.Count + " adjacent rooms");
        }
    }

    public static void DeleteDeleteRoomTriggers()
    {
        for (int i = 0; i < DeleteRoomTriggers.Count; i++)
        {
            DeleteDeleteRoomTrigger(DeleteRoomTriggers[i]);
        }
        DeleteRoomTriggers.Clear();
    }

    public static void DeleteDeleteRoomTrigger(DeleteRoomTrigger deleteRoomTrigger)
    {
        Destroy(deleteRoomTrigger.gameObject);
    }
}
