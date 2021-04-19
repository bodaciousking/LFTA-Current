//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MapGenerator : MonoBehaviour
//{
//    public Init_Placement IP;
//    MapFunctions mF;
//    public Transform tilePrefab;
//    public Transform obstaclePrefab;
//    public Vector2 mapSize;
//    [Range(0,1)]
//    public float outlinePercent;
//    [Range(0, 1)]
//    public float obstaclePercent;
//    [Range(0,0.5f)]
//    public float heightDeviation;
//    float[] heightMap;
//    public static MapGenerator instance;
//    bool[,] mapFlags;
//    public Coord[,] coordMap;
//    public Tile[,] tileGrid;

//    public bool[,] testArray;

//    public bool newMap, genMapOnStart, randomFighterSpawns;
//    public int seed = 10;
//    Coord mapCenter;

//    Grid grid;

//    List<Coord> allTileCoords;
//    public static Queue<Coord> shuffledTileCoords;

//    private void Awake()
//    {
//        if(instance != null)
//        {
//            Debug.Log("Too man mapGen instances!");
//            return;
//        }
//        instance = this;
//    }
    

//    public void GenerateMap()
//    {
//        if(newMap)
//            seed = Random.Range(0, 1000);

//        tileGrid = new Tile[(int)mapSize.x, (int)mapSize.y];

//        allTileCoords = new List<Coord>();
//        for (int x = 0; x < mapSize.x; x++)
//        {
//            for (int y = 0; y < mapSize.y; y++)
//            {
//                for (int z = 0; z < mapSize.z; z++)
//                { allTileCoords.Add(new Coord(x, y)); }
//            }
//        }
//        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
//        mapCenter = new Coord((int)mapSize.x / 2, (int)mapSize.y / 2);

//        coordMap = new Coord[(int)mapSize.x, (int)mapSize.y];
//        coordMap[mapCenter.x, mapCenter.y] = mapCenter;

//        string holderName = "Generated Map";
//        if (transform.Find(holderName))
//        {
//            DestroyImmediate(transform.Find(holderName).gameObject);
//        }
//        Transform mapHolder = new GameObject(holderName).transform;
//        mapHolder.parent = transform;
//        string obstacleHolderName = "Obstacles";
//        Transform obstacleHolder = new GameObject(obstacleHolderName).transform;
//        obstacleHolder.parent = mapHolder;

//        List<Coord> openTiles = new List<Coord>();

//        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

//        int obstaclecount = (int)(mapSize.x * mapSize.y * obstaclePercent);
//        int currentObstacleCount = 0;

//        for (int x = 0; x < mapSize.x; x++)
//        {
//            for (int y = 0; y < mapSize.y; y++)
//            {
//                Vector3 tilePosition = mF.CoordToPosition(x, y, mapSize);
//                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
//                newTile.localScale = Vector3.one * outlinePercent;
//                newTile.parent = mapHolder;
//                Tile tileScript = newTile.GetComponent<Tile>();
//                tileScript.tilePos = mF.CoordToVector2(x, y);
//                tileGrid[x, y] = tileScript;
//                Coord openTileCoord = new Coord(x,y);
//                openTiles.Add(openTileCoord);
//                coordMap[openTileCoord.x, openTileCoord.y] = openTileCoord;
//            }
//        }


//        for (int i = 0; i < obstaclecount; i++)
//        {
//            Coord randomCoord = mF.GetRandomCoord(shuffledTileCoords);
//            obstacleMap[randomCoord.x, randomCoord.y] = true;
//            currentObstacleCount++;
//            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
//            {
//                Vector3 obstaclePosition = mF.CoordToPosition(randomCoord.x, randomCoord.y, mapSize);
//                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
//                newObstacle.parent = obstacleHolder;

//                Coord obstacledCoord = new Coord(randomCoord.x, randomCoord.y);
//                openTiles.Remove(obstacledCoord);
//            }
//            else
//            {
//                obstacleMap[randomCoord.x, randomCoord.y] = false;

//                currentObstacleCount--;
//            }
//        }

//        Coord[] openTileCoords = openTiles.ToArray();
//        Coord[] shuffledOpenTiles = Utility.ShuffleArray(openTileCoords, seed);

//        if(randomFighterSpawns)
//        for (int i = 0; i < IP.randomPlaceFighters.Count; i++)
//        {
//            if(i > shuffledOpenTiles.Length)
//            {
//                Debug.Log("Too many fighters!");
//                break;
//            }
//            Vector3 pos = mF.CoordToPosition(shuffledOpenTiles[i].x, shuffledOpenTiles[i].y, mapSize);
//            IP.randomPlaceFighters[i].transform.position = pos;
//            Unit currentUnit = IP.randomPlaceFighters[i].GetComponent<Unit>();
//            currentUnit.location = mF.CoordToVector2(shuffledOpenTiles[i].x, shuffledOpenTiles[i].y);
//            Vector2 vector = new Vector2(shuffledOpenTiles[i].x, shuffledOpenTiles[i].y);
//            currentUnit.currentTile = grid.TileFromVector2(vector);
//            currentUnit.currentTile.occupied = true;
//            currentUnit.currentTile.occupyingUnit = currentUnit;
//        }

//        grid.CreateGrid();

//    }
       

//    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
//    {
//        mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
//        Queue<Coord> queue = new Queue<Coord>();
//        queue.Enqueue(mapCenter);
//        mapFlags[mapCenter.x, mapCenter.y] = true;

//        int accessibleTileCount = 1;

//        while (queue.Count > 0)
//        {
//            Coord tile = queue.Dequeue();

//            for (int x = -1; x <= 1; x++){
//                for (int y = -1; y<=1; y++)
//                {
//                    int neighbourX = tile.x + x;
//                    int neighbourY = tile.y + y;
//                    if(x==0 || y == 0)
//                    {
//                        if(neighbourX >=0 && neighbourX < obstacleMap.GetLength(0)&& neighbourY>=0 && neighbourY < obstacleMap.GetLength(1))
//                        {
//                            if(!mapFlags[neighbourX,neighbourY]&& !obstacleMap[neighbourX, neighbourY])
//                            {
//                                mapFlags[neighbourX, neighbourY] = true;
//                                queue.Enqueue(new Coord(neighbourX, neighbourY));
//                                accessibleTileCount++;
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
//        return targetAccessibleTileCount == accessibleTileCount;
//    }

//    public void Start()
//    {
//        mF = MapFunctions.instance;
//        IP = Init_Placement.instance;
//        grid = GameObject.Find("A*").GetComponent<Grid>();
//        if (genMapOnStart)
//        GenerateMap();

//    }
//}




