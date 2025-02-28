using System;
using UnityEngine;

[Serializable]
public class Point
{
    public float x, y, z;

    public Point(float x, float y,float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public float Distance(Point other)
    {
        return Mathf.Sqrt(Mathf.Pow(other.x - x, 2) + Mathf.Pow(other.y - y, 2) + Mathf.Pow(other.z - z, 2));
    }

}
 


