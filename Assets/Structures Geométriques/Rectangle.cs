using UnityEngine;

public struct Rectangle
{
    public Point2D topLeft;
    public float width, height;

    public Rectangle(Point2D topLeft, float width, float height)
    {
        this.topLeft = topLeft;
        this.width = width;
        this.height = height;
    }

    public float Area() => width * height;
}