using System.Collections.Generic;
using UnityEngine;

public class CharacterRoutine
{
    public List<CharacterRoutineTask> RoutineTasks = new List<CharacterRoutineTask>();
    public Character Character;
    public bool InRoutine = false;

    public CharacterRoutine(Character character)
    {
        Character = character;
    }

    public void TryGetNewRoutineTask() // updated when the character has no routine tasks. Later: when the character has needs (eg hungry), new routine tasks will be added automatically (eg eat)
    {
        if (Random.Range(0, 2) == 0)
        {
            RoutineTasks.Add(PickRandomRoutineTask());
        }
    }

    public CharacterRoutineTask PickRandomRoutineTask()
    {
        return CharacterRoutineTask.CreateCharacterRoutineTask(Character);
    }

    public void InterruptRoutine()
    {
        Logger.Log("Interupted routine of {0}", Character.FullName());
        InRoutine = false;
        RoutineTasks.Clear();
    }
}
