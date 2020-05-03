﻿using System.Collections;
using UnityEngine;

public class NavActor : MonoBehaviour
{
    const float MIN_PATH_UPDATE_TIME = .2f;
    const float PATH_UPDATE_MOVE_THRESHOLD = .5f;

    public Vector2 Target;
    public float TurnSpeed = 3;
    public float TurnDst = 5;
    public bool FollowingPath;
    public bool IsReevaluating;

    public NavPath Path;
    public Character Character;

    public void Start()
    {
        FollowingPath = false;
        IsReevaluating = false;
        StartCoroutine(UpdatePath());
    }

    public void SetCharacter(Character character)
    {
        Character = character;
        character.NavActor = this;

        transform.position = Character.transform.position;
        gameObject.name = "NavActor " + Character.FullName();
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            Path = new NavPath(waypoints, transform.position, TurnDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            StopCoroutine("FollowPath");
            FollowingPath = false;
        }
    }

    private IEnumerator UpdatePath()
    {
        if(Time.timeSinceLevelLoad < .3f)
        {
            yield return new WaitForSeconds(.3f);
        }

        float sqrMoveThreshold = PATH_UPDATE_MOVE_THRESHOLD * PATH_UPDATE_MOVE_THRESHOLD;
        Vector2 targetPosOld = Target;

        while (true)
        {
            yield return new WaitForSeconds(MIN_PATH_UPDATE_TIME);

            if ((Target - targetPosOld).sqrMagnitude > sqrMoveThreshold || IsReevaluating)
            {
                PathRequestManager.RequestPath(transform.position, Target, OnPathFound);
                targetPosOld = Target;
                IsReevaluating = false;
            }
        }
    }

    private IEnumerator FollowPath()
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
                if (Character.CharacterActionState == CharacterActionState.PlayerAction)
                {
                    yield return null;
                    continue;
                }

                Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
                while (Path.TurnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
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
                    transform.Translate(Vector3.forward * Time.deltaTime * Character.PlayerLocomotion.Speed, Space.Self);

                    transform.position = new Vector3(transform.position.x, transform.position.y, -1);
                    Character.PlayerLocomotion.SetPosition(transform.position);
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
