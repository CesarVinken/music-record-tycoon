using System;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest CurrentPathRequest;

    public static PathRequestManager Instance;
    Pathfinding Pathfinding;

    bool IsProcessingPath;

    public void Awake()
    {
        Instance = this;
        Pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        Instance.PathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    public void TryProcessNext()
    {
        if(!IsProcessingPath && PathRequestQueue.Count > 0)
        {
            CurrentPathRequest = PathRequestQueue.Dequeue();
            IsProcessingPath = true;

            Pathfinding.StartFindPath(CurrentPathRequest.PathStart, CurrentPathRequest.PathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        CurrentPathRequest.Callback(path, success);
        IsProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 PathStart;
        public Vector3 PathEnd;
        public Action<Vector3[], bool> Callback;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
        {
            PathStart = start;
            PathEnd = end;
            Callback = callback;
        }
    }
}
