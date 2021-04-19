using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MapFunctions : MonoBehaviour
{
    public static MapFunctions instance;
    public GameObject sectModel;

    private void Awake()
    {
        instance = this;
    }


    public List<Coord> GetNeighbours(Coord sourceTile, int range, Vector2 mapSize, Coord[,] coordMap)
    {
        List<Coord> neighbours;
        Coord tile = sourceTile;
        neighbours = new List<Coord>();

        float halfRange = range * 0.5f;

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                int neighbourX = tile.x + x;
                int neighbourY = tile.y + y;
                if (x == 0 || y == 0 || x <= halfRange && y <= halfRange && y >= -halfRange && x >= -halfRange)
                {
                    if (neighbourX >= 0 && neighbourX < (int)mapSize.x && neighbourY >= 0 && neighbourY < (int)mapSize.y)
                    {
                        neighbours.Add(coordMap[neighbourX, neighbourY]);
                    }
                }
            }
        }
        return neighbours;
    }
    public Vector3 CoordToPosition(int x, int y, int z, Vector3 mapSize)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, -mapSize.y /2 + 0.5f + y, -mapSize.z / 2 + 0.5f + z);
    }

    public Vector2 CoordToVector2(int x, int y)
    {
        return new Vector2(x, y);
    }

    public Coord GetRandomCoord(Queue<Coord> shuffledTileCoords)
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public GameObject CreateSectGraphic(Sect sect)
    {
        GameObject obj = Instantiate(sectModel, sect.actualLocation, Quaternion.Euler(Vector3.right * 90));
        return obj;
    }

    public Vector3 CoordToVector3(Coord coord)
    {
        return new Vector3(coord.x, coord.y, coord.z);
    }


}

public class Sect
{
    public Vector3 actualLocation;
    public Vector3 simpleLocation;
    public BasicSect basicSect;
    public int graphicsReference;
    public GameObject graphics;
    public Coord coord;
    public Node node;

    public Sect(Vector3 _actualLocation, Vector3 _simpleLocation, GameObject _graphics, Coord _coord, BasicSect _basicSect)
    {
        actualLocation = _actualLocation;
        simpleLocation = _simpleLocation;
        graphics = _graphics;
        coord = _coord;
        basicSect = _basicSect;
    }

    public void ChangeColor(Color newColor)
    {
        graphics.GetComponent<Renderer>().material.color = newColor;
        string colorString = JsonUtility.ToJson(newColor);
        basicSect.colorString = colorString;
    }
}
[System.Serializable]
public class BasicSect
{
    public int x, y, z;
    public int gR;
    public string colorString;

    public BasicSect(int _x, int _y, int _z, int _gR, string _sectColor)
    {
        x = _x;
        y = _y;
        z = _z;
        gR = _gR;
        colorString = _sectColor;
    }
}