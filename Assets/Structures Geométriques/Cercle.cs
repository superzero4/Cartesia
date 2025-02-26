using UnityEngine;

public struct Cercle
{
    public Point2D center;
    public float radius;

    public Cercle(Point2D center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public float Area() => Mathf.PI * radius * radius;
}
