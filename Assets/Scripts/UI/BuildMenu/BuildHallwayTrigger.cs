using System.Collections.Generic;
using UnityEngine;

public class BuildHallwayTrigger : MonoBehaviour
{
    private Vector2 _startingPoint = new Vector2();
    private Vector2 _midpoint = new Vector2();
    public static List<BuildHallwayTrigger> BuildHallwayTriggers = new List<BuildHallwayTrigger>();

    public void Setup(Vector2 startingPoint, ObjectDirection direction)
    {
        _startingPoint = startingPoint;
        BuildHallwayTriggers.Add(this);

        _midpoint = new Vector2(_startingPoint.x, _startingPoint.y + 7.5f);
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);

        RectTransform rect = GetComponent<RectTransform>();
        switch (direction)
        {
            case ObjectDirection.LeftDown:
                rect.Rotate(new Vector3(0, 0, 180));
                break;
            case ObjectDirection.LeftUp:
                rect.Rotate(new Vector3(0, 0, 90));
                break;
            case ObjectDirection.RightUp:
                break;
            case ObjectDirection.RightDown:
                rect.Rotate(new Vector3(0, 0, -90));
                break;
            default:
                Logger.Error("Not implemented direction {0}", direction);
                break;
        }
    }

    public void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(_midpoint);

        if (GameManager.Instance.CurrentPlatform == Platform.PC)
        {
            if (Input.GetMouseButtonDown(1))
            {
                BuilderManager.Instance.DeleteAllTriggers();
            }
        }
    }

    public void BuildHallway()
    {
        DeleteAllHallwayTriggers();
        BuilderManager.Instance.BuildRoom(BuilderManager.Instance.SelectedRoom, _startingPoint, RoomRotation.Rotation0);
    }

    public static void DeleteAllHallwayTriggers()
    {
        for (int i = 0; i < BuildHallwayTriggers.Count; i++)
        {
            Destroy(BuildHallwayTriggers[i].gameObject);
        }
        BuildHallwayTriggers.Clear();
    }
}
