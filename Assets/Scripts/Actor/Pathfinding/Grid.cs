using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public bool DisplayRouteGizmos;
    //public Transform Player;
    public LayerMask UnwalkableMask;
    public Vector2 GridWorldSize;   //this should be based on the size of the background image
    public float NodeRadius;
    public TerrainType[] WalkableRegions;
    public int ObstacleProximityPenalty = 40;
    private LayerMask _walkableMask;
    private Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();
    private Node[,] _myGrid;

    private float _nodeDiameter;
    private int _gridSizeX;
    private int _gridSizeY;

    int _penaltyMin = int.MaxValue;
    int _penaltyMax = int.MinValue;

    public void Awake()
    {
        _nodeDiameter = NodeRadius * 2;
        if(GridWorldSize.x <= 0 || GridWorldSize.y <= 0)
        {
            Debug.LogError("Invalid world grid values");
        }
        if(NodeRadius <= 0)
        {
            Debug.LogError("Node radius must be larger than 0");
        }

        _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
        _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);

        foreach (TerrainType region in WalkableRegions)
        {
            _walkableMask.value |= region.TerrainMask.value;
            _walkableRegionsDictionary.Add((int)Mathf.Log(region.TerrainMask.value, 2), region.TerrainPenalty);
        }

        CreateGrid();
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
        _myGrid = new Node[_gridSizeX, _gridSizeY];

        Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.up * (y * _nodeDiameter + NodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, NodeRadius, UnwalkableMask));

                int movementPenalty = 0;

                Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100, _walkableMask))    //get terrain layer of node
                {
                    _walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if(!walkable)
                {
                    movementPenalty += ObstacleProximityPenalty;
                }

                _myGrid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }
        Debug.Log("grid size " + _myGrid.Length);
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


                if(blurredPenalty > _penaltyMax)
                {
                    _penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < _penaltyMin)
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

                if(checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                {
                    neighbours.Add(_myGrid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float percentY = (worldPosition.y + GridWorldSize.y / 2) / GridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

        return _myGrid[x, y];
    }


    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 1));

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
