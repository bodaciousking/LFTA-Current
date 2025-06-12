using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public static Grid instance;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask;
    public Node[,,] nodeGrid;

    float nodeDiameter;
    public int gridSizeX, gridSizeY, gridSizeZ;

    MapInfo mapInfo;

    public Tile TileFromGrid(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0 || y < 0 || z < 0 ||
            x >= nodeGrid.GetLength(0) ||
            y >= nodeGrid.GetLength(1) ||
            z >= nodeGrid.GetLength(2))
        {
            return null;
        }

        return TileFromNode(nodeGrid[x, y, z]);
    }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of Grid found!");
            return;
        }
        instance = this;


        nodeDiameter = nodeRadius * 2;
    }

    private void Start()
    {
        mapInfo = MapInfo.instance;
    }

    public int MaxSize
    {
        get {
            return gridSizeX * gridSizeY * gridSizeZ;
        }
    }
    public void CreateGrid()
    {
        mapInfo = MapInfo.instance;
        gridWorldSize = mapInfo.mapSize;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
        nodeGrid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2 - Vector3.up * gridWorldSize.y / 2;
        unwalkableMask = 1 << 11;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = true;
                    RaycastHit hit;
                    if (Physics.Raycast(worldPoint + Vector3.down * 0.1f, Vector3.up, out hit, 3.5f))
                    {
                        walkable = false;
                    }
                    nodeGrid[x, y, z] = new Node(walkable, worldPoint, x, y, z);

                    if (TileFromNode(nodeGrid[x, y, z]))
                    {
                        Tile tileScript = TileFromNode(nodeGrid[x, y, z]);
                        tileScript.blocked = !walkable;
                        tileScript.nodeWorldPoint = worldPoint;
                    }
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node, int range, int vRange)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -vRange; y <= vRange; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    if (x == 0 && y == 0 && z == 0 || y != 0 && x == 0 && z == 0 || x != 0 && z != 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(nodeGrid[checkX, checkY, checkZ]);
                    }
                }
            }
        }
        return neighbours;
    }
    public List<Tile> NodesToTiles(List<Node> nodes)
    {
        List<Tile> tiles = new List<Tile>();

        foreach (Node node in nodes)
        {
            tiles.Add(TileFromNode(node));
        }

        return tiles;
    }
    public List<Node> Get8Neighbours(Node node, int range)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int distance = Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);
                    if (distance > range)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbours.Add(nodeGrid[checkX, checkY, checkZ]);
                    }
                }

            }
        }
        return neighbours;
    } 
    public bool IsWithinBounds(Vector3Int check)
    {
        if (check.x >= 0 && check.x < gridSizeX && check.y >= 0 && check.y < gridSizeY && check.z >= 0 && check.z < gridSizeZ)
        {
            return true;
        }
        else return false;
    }

    public List<Vector3> GetSurroundingEnemyLocations(Vector3 currentLocation, int team)
    {
        List<Vector3> locations = new List<Vector3>();

        Node currentNode = NodeFromWorldPoint(currentLocation);
        List<Node> surroundingNodes = GetNeighbours(currentNode, 1, 1);
        for (int i = 0; i < surroundingNodes.Count; i++)
        {
            if (TileFromNode(surroundingNodes[i]))
            {
                Tile nodeTile = TileFromNode(surroundingNodes[i]);
                if (nodeTile.occupyingUnit)
                {
                    if (nodeTile.occupyingUnit.alive)
                        if (nodeTile.occupyingUnit.team != team)
                        {
                            locations.Add(nodeTile.occupyingUnit.transform.position);
                        }
                }
            }
        }

        return locations;
    }

    public Tile TileFromNode(Node node)
    {
        int x = node.gridX;
        int y = node.gridY;
        int z = node.gridZ;

        return mapInfo.tileGrid[x, y, z];
        
    }

    public void ResetGrid()
    {
        //Debug.Log ( "Called resetgrid.");
        foreach (Node item in nodeGrid)
        {
            item.gCost = 0;
            item.parent = null;
            item.hCost = 0;
            item.aCost = 0;
        }
    }

    public void ResetTileGrid()
    {
        //Debug.Log("Called reset tile grid.");
        foreach (Tile item in mapInfo.tileGrid)
        {
            if (item)
            {
                item.inMoveRange = false;
                item.inAttackRange = false;
                item.attackable = false;
                item.validAbilityTarget = false;
            }
        }
    }

    public Node NodeFromTile(Tile tile)
    {
        return nodeGrid[(int)tile.tilePos.x, (int)tile.tilePos.y, (int)tile.tilePos.z];
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); 
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
        return nodeGrid[x, y, z];
    }


    public Tile TileFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        //Debug.Log("X:" + x + " Y:" + y + " Z:" + z);
        return mapInfo.tileGrid[x, y, z];
    }

    private void Update()
    {
    }
}
