
using System;
using System.Threading.Tasks;

public class CharacterRoutineTask
{
    public Character Character;
    public CharacterRoutineType CharacterRoutineType;
    public int Duration;

    public CharacterRoutineTask(CharacterRoutineType routineType, Character character)
    {
        CharacterRoutineType = routineType;
        Character = character;
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(CharacterRoutineType routineType, Character character)
    {
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineType, character)
            .WithDuration();
        return characterRoutineTask;
    }

    public static CharacterRoutineTask CreateCharacterRoutineTask(Character character)
    {
        CharacterRoutineType routineType = GetRandomRoutineType();
        CharacterRoutineTask characterRoutineTask = new CharacterRoutineTask(routineType, character)
            .WithDuration();
        return characterRoutineTask;
    }

    private CharacterRoutineTask WithDuration(int duration = 1000)
    {
        Duration = duration;
        return this;
    }

    public async Task Execute()
    {
        Character.SetCharacterActionState(CharacterActionState.RoutineAction);
        //Logger.Log("{0} executes {1}", Character.FullName(), CharacterRoutineType);
        await Task.Delay(Duration);
        return;
    }

    private static CharacterRoutineType GetRandomRoutineType()
    {
        Array values = Enum.GetValues(typeof(CharacterRoutineType));
        int randomValue = Util.InitRandomNumber().Next(values.Length);
        Logger.Log("{0} randomValue {1}", randomValue, values.Length);

        CharacterRoutineType randomCharacterRoutineType = (CharacterRoutineType)values.GetValue(randomValue);

        return randomCharacterRoutineType;
    }
}
