using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BuildCommand : CommandProcedure
{
    public async override void Run(List<string> arguments)
    {
        if(BuilderManager.Instance == null )
        {
            Console.Instance.PrintToReportText("The build command can only be used while in game");
            return;
        }
        switch (arguments[0])
        {
            case "room":
                await BuildRoom(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown build command to build " + arguments[0]);
                break;
        }
    }

    public async Task BuildRoom(List<string> arguments)
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
        await CharacterManager.Instance.UpdatePathfindingGrid();
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

                    await BuilderManager.Instance.BuildRoom(room1, new Vector2(0, 0), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(room1, new Vector2(45, -37.5f), RoomRotation.Rotation180);
                    await BuilderManager.Instance.BuildRoom(room1, new Vector2(60, -75), RoomRotation.Rotation180);
                    await BuilderManager.Instance.BuildRoom(room1, new Vector2(-15, -67.5f), RoomRotation.Rotation270);

                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, 0), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(45, -7.5f), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, -15), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(15, -22.5f), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(0, -30), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(15, -37.5f), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(30, -45), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(45, -52.5f), RoomRotation.Rotation0);
                    await BuilderManager.Instance.BuildRoom(hallway, new Vector2(0, -45), RoomRotation.Rotation0);

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
}
