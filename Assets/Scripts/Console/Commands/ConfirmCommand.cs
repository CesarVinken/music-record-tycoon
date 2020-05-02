using System.Collections.Generic;
using UnityEngine;

// The confirm command for checking and confirming setup is correct, ..
public class ConfirmCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        if (BuilderManager.Instance == null)
        {
            Console.Instance.PrintToReportText("The confirm command can only be used while in game");
            return;
        }
        switch (arguments[0])
        {
            case "wallpieces":
                ConfirmWallPiecesAlign();
                break;
            default:
                Console.Instance.PrintToReportText("Unknown delete command to confirm " + arguments[0]);
                break;
        }
    }
    public override void Help()
    {
        string printLine = "Use the confirm command to run certain tests and calculations in the game.";
        printLine += "\nUse 'wallpieces' as a 2nd argument to check if all the wallpieces are positioned corerctly. If they are not lining up, this can break wall visibility.";
        Console.Instance.PrintToReportText(printLine);
    }

    // Make sure that wall pieces of all room prefabs are on the correct location. Because if they are not lining up, it can break the wall visibility.
    public void ConfirmWallPiecesAlign()
    {
        Console.Instance.PrintToReportText("\n-----------------------------------------\n");
        Console.Instance.PrintToReportText("Starting alignment analysis of wallpieces\n");

        GameObject testRoomContainer = GameObject.Instantiate(new GameObject());
        int problemCount = 0;
        foreach (KeyValuePair<RoomName, Dictionary<ObjectRotation, GameObject>> prefabGroup in BuilderManager.Instance.RegisteredRoomPrefabs)
        {
            RoomName roomName = prefabGroup.Key;
            GameObject prefabGO = prefabGroup.Value[ObjectRotation.Rotation0];
            GameObject roomGO = GameObject.Instantiate(prefabGO, testRoomContainer.transform);

            Room room = roomGO.GetComponent<Room>();
            for (int i = 0; i < room.WallPieces.Count; i++)
            {
                WallPiece wallPiece = room.WallPieces[i];

                float xPosition = wallPiece.transform.position.x;
                float yPosition = wallPiece.transform.position.y;
                // check position
                if (xPosition % 5 != 0 || yPosition % 2.5f != 0)
                {
                    string printLine = "The wall piece <b>" + wallPiece.name + "</b> in room <b>" + room.name + "</b> is not aligned with the grid. Its coordinates are " + xPosition + "," + yPosition + "\n";

                    if(xPosition % 5 != 0)
                        printLine += "<color=red>  - wallPiece.transform.position.x % 5 = " + (xPosition % 5 + "</color>\n");
                    else
                        printLine += "  - wallPiece.transform.position.x % 5 = " + (xPosition % 5 + "\n");

                    if (yPosition % 5 != 0)
                        printLine += "<color=red>  - wallPiece.transform.position.y % 2.5f = " + (yPosition % 2.5f + "</color>\n");
                    else
                        printLine += "  - wallPiece.transform.position.y % 2.5f = " + (yPosition % 2.5f + "\n");

                    Console.Instance.PrintToReportText(printLine);
                }
            }
        }

        Console.Instance.PrintToReportText("-----------------------------------------\n");
        
        if (problemCount > 0)
            Console.Instance.PrintToReportText("\nFinished analysis. <color=red>There were " + problemCount + " problems.</color>\n");
        else
            Console.Instance.PrintToReportText("\nFinished analysis. <color=green>All wall pieces are aligned correctly!</color>\n");

        GameObject.Destroy(testRoomContainer);
    }
}
