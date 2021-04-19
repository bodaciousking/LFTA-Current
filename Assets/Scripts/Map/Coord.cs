using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Coord
{
    public int x;
    public int y;
    public int z;

    public Coord(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }

    public static bool operator ==(Coord c1, Coord c2)
    {
        return c1.x == c2.x && c1.y == c2.y && c1.z == c2.z;
    }

    public static bool operator !=(Coord c1, Coord c2)
    {
        return !(c1 == c2);
    }
}
