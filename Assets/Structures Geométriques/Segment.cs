using System;
using UnityEngine;

[Serializable]
public struct Segment
{
    public Point start, end;

    public Segment(Point start, Point end)
    {
        this.start = start;
        this.end = end;
    }

    public float Length()
    {
        return start.Distance(end);
    }
}
