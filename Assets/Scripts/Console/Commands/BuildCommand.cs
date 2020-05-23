using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BuildCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        if(BuilderManager.Instance == null )
        {
            Console.Instance.PrintToReportText("The build command can only be used while in game");
            return;
        }
        switch (arguments[0])
        {
            case "room":
                BuildRoom(arguments);
                break;
            case "character":
                BuildCharacter(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown build command to build " + arguments[0]);
                break;
        }
    }

    public override void Help()
    {
        string printLine = "With the build command you can create assets in the level.";
        printLine += "\nUse 'room' as a 2nd argument and a name as a 3rd argument to load a building scenario for the level.";
        printLine += "\nUse 'character' as a 2nd argument to add a random character to the scene. There currently are no further specific options to create a character.";
        Console.Instance.PrintToReportText(printLine);
    }

    public void BuildRoom(List<string> arguments)
    {
        if(arguments.Count == 1)
        {
            Console.Instance.PrintToReportText("Build scenario number needed as argument in order to build rooms.");
            return;
        }
        Logger.Log("Number of rooms : {0}", RoomManager.Rooms.Count);

        //prepare
        BuilderManager.Instance.DeactivateBuildMenuMode();

        // remove all existing rooms
        RoomManager.Instance.DeleteAllRooms();
        //await CharacterManager.Instance.UpdatePathfindingGrid();
        BuilderManager.Instance.BuildingTiles.Clear();
        BuilderManager.Instance.BuildingTileLocations.Clear();

        // build room scenario
        int buildScenario;
        if (int.TryParse(arguments[1], out buildScenario))
        {
            switch (buildScenario)
            {
                case 1:
                    BuildingTileBuilder buildingTileBuilder = new BuildingTileBuilder();

                    buildingTileBuilder.SetupInitialBuildingTiles();

                    for (int i = 0; i < CharacterManager.Instance.Characters.Count; i++)
                    {
                        CharacterManager.Instance.Characters[i].CurrentRoom = null;
                    }

                    RoomBlueprint room1 = RoomBlueprint.CreateBlueprint(RoomName.Room1);
                    RoomBlueprint hallway = RoomBlueprint.CreateBlueprint(RoomName.Hallway);

                    BuilderManager.Instance.BuildRoom(room1, new Vector2(0, 0), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(room1, new Vector2(45, -37.5f), ObjectRotation.Rotation180);
                    BuilderManager.Instance.BuildRoom(room1, new Vector2(60, -75), ObjectRotation.Rotation180);
                    BuilderManager.Instance.BuildRoom(room1, new Vector2(-15, -67.5f), ObjectRotation.Rotation270);

                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, 0), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(45, -7.5f), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, -15), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(15, -22.5f), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(0, -30), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(15, -37.5f), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, -45), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(45, -52.5f), ObjectRotation.Rotation0);
                    BuilderManager.Instance.BuildRoom(hallway, new Vector2(0, -45), ObjectRotation.Rotation0);

                    BuildHallwayTrigger.DeleteAllHallwayTriggers();

                    // set current room characters?
                    break;
                default:
                    Console.Instance.PrintToReportText("Unknown build command to build " + arguments[0]);
                    break;
            }
        }
        else
        {
            Console.Instance.PrintToReportText("Argument 1 for Build [room] should be an int. Could not parse argument #" + (2) + " (" + arguments[1] + ")");
            return;
        }

        Console.Instance.PrintToReportText("Built room");
    }

    public void BuildCharacter(List<string> allArguments)
    {
        List<string> arguments = allArguments.Where((v, i) => i != 0).ToList();

        if(arguments.Count > 0)
        {
            Console.Instance.PrintToReportText("Argument '" + arguments[0] + "' was invalid. Could not create character");
            return;
        }

        CharacterStats characterStats = new CharacterStats(
                CharacterRoleGenerator.Generate(),
                CharacterAgeGenerator.Generate(),
                CharacterNameGenerator.PickGender());

        Vector2 startingPosition = new Vector2(15, 15);

        CharacterManager.Instance.GenerateCharacter(
            characterStats,
            startingPosition);

        Character character = CharacterManager.Instance.Characters[CharacterManager.Instance.Characters.Count - 1];
        string characterName = CharacterNameGenerator.GetName(character.CharacterName);
        Console.Instance.PrintToReportText(characterName + " was just born");
    }
}
