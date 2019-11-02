using System.Collections;
using UnityEngine;

public class NavActor : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = .2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = .5f;

    public Vector3 Target;
    public float TurnSpeed = 3;
    public float TurnDst = 5;
    public bool FollowingPath;

    public NavPath Path;

    public void Start()
    {
        FollowingPath = false;
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            Path = new NavPath(waypoints, transform.position, TurnDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator UpdatePath()
    {
        if(Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
        Vector3 targetPosOld = Target;

        while (true)
        {
            yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);

            if ((Target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(transform.position, Target, OnPathFound);
                targetPosOld = Target;
            }
        }
    }

    IEnumerator FollowPath()
    {
        if (Path.LookPoints.Length == 0)
        {
            FollowingPath = false;
            yield return null;
        }
        else
        {
            FollowingPath = true;
            int pathIndex = 0;
            transform.LookAt(Path.LookPoints[0]);

            while (FollowingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
                while (Path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    //Debug.LogWarning(pathIndex + " out of " + Path.FinishLineIndex);

                    if (pathIndex == Path.FinishLineIndex)
                    {
                        FollowingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }
                if (FollowingPath)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(Path.LookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * TurnSpeed);
                    transform.Translate(Vector3.forward * Time.deltaTime * PlayerCharacter.Instance.PlayerLocomotion.Speed, Space.Self);

                    transform.position = new Vector3(transform.position.x, transform.position.y, -1);
                    PlayerCharacter.Instance.PlayerLocomotion.SetPosition(transform.position);
                }
                yield return null;
            }
        }
    }

    public void DrawPathGizmo()
    {
        if(Path != null)
        {
            Path.DrawWithGizmos();
        }
    }
}
