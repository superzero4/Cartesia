using UnityEngine;

public struct Point2D
{
    public float x, y;

    public Point2D(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString() => $"({x}, {y})";
}
 


