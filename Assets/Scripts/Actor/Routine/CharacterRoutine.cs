using System.Collections.Generic;
using UnityEngine;

public class CharacterRoutine
{
    public List<CharacterRoutineTask> RoutineTasks = new List<CharacterRoutineTask>();

    public void TryGetNewRoutineTask(Character character) // updated when the character has no routine tasks. Later: when the character has needs (eg hungry), new routine tasks will be added automatically (eg eat)
    {
        if (Random.Range(0, 2) == 0)
        {
            RoutineTasks.Add(PickRandomRoutineTask(character));
        }
    }

    public CharacterRoutineTask PickRandomRoutineTask(Character character)
    {
        return CharacterRoutineTask.CreateCharacterRoutineTask(character);
    }
}
