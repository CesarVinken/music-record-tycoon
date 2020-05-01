using System.Collections.Generic;
using System.Linq;

public class ShowCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        switch (arguments[0])
        {
            case "gizmo":
                ShowGizmo(arguments);
                break;
            default:
                Console.Instance.PrintToReportText("Unknown show command to show " + arguments[0]);
                break;
        }
    }

    public void ShowGizmo(List<string> allArguments)
    {
        if (GameManager.Instance.CurrentPlatform == Platform.Android)
        {
            Console.Instance.PrintToReportText("Toggling the gizmo display settings is only possible on pc");
            return;
        }

        List<string> arguments = allArguments.Where((v, i) => i != 0).ToList();

        switch (arguments[0])
        {
            case "pathfinding-grid":
                if (arguments.Count > 1 && arguments[1] == "off")
                {
                    GameManager.ShowPathfindingGridGizmos = false;
                    return;
                }
                GameManager.ShowPathfindingGridGizmos = true;
                return;
            case "character-path":
                if (arguments.Count > 1 && arguments[1] == "off")
                {
                    GameManager.DrawCharacterPathGizmo = false;
                    return;
                }
                GameManager.DrawCharacterPathGizmo = true;
                return;
            case "building-tiles":
                if (arguments.Count > 1 && arguments[1] == "off")
                {
                    GameManager.DrawBuildingTilesGizmos = false;
                    return;
                }
                GameManager.DrawBuildingTilesGizmos = true;
                return;
            case "door-location":
                if (arguments.Count > 1 && arguments[1] == "off")
                {
                    GameManager.DrawDoorLocationGizmos = false;
                    return;
                }
                GameManager.DrawDoorLocationGizmos = true;
                return;
            default:
                Console.Instance.PrintToReportText("Unknown argument " + arguments[0] + ".");
                return;
        }
    }

}
