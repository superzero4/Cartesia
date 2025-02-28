using System;
using UnityEngine;

[Serializable]
public struct Cercle
{
    public Point center;
    public float radius;

    public Cercle(Point center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public float Area() => Mathf.PI * radius * radius;
}
