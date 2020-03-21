using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

public class Grid : MonoBehaviour
{
    public bool DisplayRouteGizmos;
    public LayerMask UnwalkableMask;
    public int GridWorldSizeX;   //this should be based on the size of the background image
    public int GridWorldSizeY;   //this should be based on the size of the background image
    public float NodeRadius;
    public TerrainType[] WalkableRegions;
    public int ObstacleProximityPenalty = 40;
    private LayerMask _walkableMask;
    private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
    private Node[,] _myGrid;

    private float _nodeDiameter;
    private int _gridSizeX;
    private int _gridSizeY;
    private float _normalisedWorldOffsetX;  // Used if the map is more to the right than to the left
    private float _normalisedWorldOffsetY;  // User if the map is more up than down
    private Vector2 _minimumNodeLocation;

    int _penaltyMin = int.MaxValue;
    int _penaltyMax = int.MinValue;

    public void Awake()
    {
        _nodeDiameter = NodeRadius * 2;

        if (NodeRadius <= 0)
        {
            Logger.Error(Logger.Pathfinding, "Node radius must be larger than 0");
        }

        foreach (TerrainType region in WalkableRegions)
        {
            _walkableMask.value |= region.TerrainMask.value;
            _walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
        }

        //CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return _gridSizeX * _gridSizeY;
        }
    }

    public void CreateGrid()
    {
        Logger.Log(Logger.Pathfinding, "Create grid");
        GridWorldSizeX = Mathf.RoundToInt(-CameraController.PanLimits[Direction.Left] + CameraController.PanLimits[Direction.Right]);
        GridWorldSizeY = Mathf.RoundToInt(-CameraController.PanLimits[Direction.Down] + CameraController.PanLimits[Direction.Up]);
        _gridSizeX = Mathf.RoundToInt(GridWorldSizeX / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSizeY / _nodeDiameter);

        _myGrid = new Node[_gridSizeX, _gridSizeY];
        Vector3 worldBottomLeft = new Vector3(CameraController.PanLimits[Direction.Left], CameraController.PanLimits[Direction.Down], 0);
        Vector2 firstNodeLocation = worldBottomLeft + Vector3.right * (0 * _nodeDiameter + NodeRadius) + Vector3.up * (0 * _nodeDiameter + NodeRadius);
        Vector2 lastNodeLocation = worldBottomLeft + Vector3.right * (_gridSizeX - 1 * _nodeDiameter + NodeRadius) + Vector3.up * (_gridSizeY - 1 * _nodeDiameter + NodeRadius);
        _normalisedWorldOffsetY = Math.Abs(lastNodeLocation.y) - Math.Abs(firstNodeLocation.y);
        if (Math.Abs(firstNodeLocation.x) > Math.Abs(lastNodeLocation.x))
        {
            _minimumNodeLocation.x = Math.Abs(firstNodeLocation.x);
            _normalisedWorldOffsetX = 0;
        }
        else
        {
            _minimumNodeLocation.x = Math.Abs(lastNodeLocation.x);
            _normalisedWorldOffsetX = Math.Abs(lastNodeLocation.x) - Math.Abs(firstNodeLocation.x);
        }

        if (Math.Abs(firstNodeLocation.y) > Math.Abs(lastNodeLocation.y))
        {
            _minimumNodeLocation.y = Math.Abs(firstNodeLocation.y);
            _normalisedWorldOffsetY = 0;
        }
        else
        {
            _minimumNodeLocation.y = Math.Abs(lastNodeLocation.y);
            _normalisedWorldOffsetY = Math.Abs(lastNodeLocation.y) - Math.Abs(firstNodeLocation.y);
        }

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.up * (y * _nodeDiameter + NodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, NodeRadius, UnwalkableMask));

                int movementPenalty = 0;

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, _walkableMask))    //get terrain layer of node
                {
                    _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if (!walkable)
                {
                    movementPenalty += ObstacleProximityPenalty;
                }

                _myGrid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        BlurPenaltyMap(3);
    }

    public void BlurPenaltyMap(int blurSize)
    {
        int kernelSize = blurSize * 2 + 1;
        int kernelExtents = (kernelSize - 1) / 2;

        int[,] penaltiesHorizontalPass = new int[_gridSizeX, _gridSizeY];
        int[,] penaltiesVerticalPass = new int[_gridSizeX, _gridSizeY];

        for (int y = 0; y < _gridSizeY; y++)
        {
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += _myGrid[sampleX, y].MovementPenalty;
            }

            for (int x = 1; x < _gridSizeX; x++)
            {
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, _gridSizeX);
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, _gridSizeX - 1);

                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - _myGrid[removeIndex, y].MovementPenalty + _myGrid[addIndex, y].MovementPenalty;
            }
        }

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
            }

            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            _myGrid[x, 0].MovementPenalty = blurredPenalty;

            for (int y = 1; y < _gridSizeY; y++)
            {
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, _gridSizeY);
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, _gridSizeY - 1);

                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                _myGrid[x, y].MovementPenalty = blurredPenalty;


                if (blurredPenalty > _penaltyMax)
                {
                    _penaltyMax = blurredPenalty;
                }
                if (blurredPenalty < _penaltyMin)
                {
                    _penaltyMin = blurredPenalty;
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_myGrid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector2 NormalisedWorldSize = new Vector2(_minimumNodeLocation.x * 2, _minimumNodeLocation.y * 2); // take the minumum point on the grid and mirror it in order to get a world size in which the negative range is as large as the positive range. Then we can take the percentage of that worldSize
        float percentX = (worldPosition.x + NormalisedWorldSize.x / 2) / NormalisedWorldSize.x;
        float percentY = (worldPosition.y + NormalisedWorldSize.y / 2) / NormalisedWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        float _gridSizeFoNormalisedWorldSizeX = Mathf.RoundToInt(NormalisedWorldSize.x / _nodeDiameter);
        float _gridSizeForNormalisedWorldSizeY = Mathf.RoundToInt(NormalisedWorldSize.y / _nodeDiameter);

        int nodeX = Mathf.RoundToInt((_gridSizeFoNormalisedWorldSizeX - 1) * percentX) - Mathf.RoundToInt(_normalisedWorldOffsetX / _nodeDiameter);
        int nodeY = Mathf.RoundToInt((_gridSizeForNormalisedWorldSizeY - 1) * percentY) - Mathf.RoundToInt(_normalisedWorldOffsetY / _nodeDiameter);

        return _myGrid[nodeX, nodeY];
    }

    public void DrawPathfindingGridGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSizeX, GridWorldSizeY, 1));

        if (_myGrid != null && DisplayRouteGizmos)
        {
            foreach (Node n in _myGrid)
            {
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(_penaltyMin, _penaltyMax, n.MovementPenalty));
                Gizmos.color = (n.Walkable) ? Gizmos.color : Color.red;

                Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter));
            }
        }
    }

    [System.Serializable]
    public class TerrainType
    {
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}