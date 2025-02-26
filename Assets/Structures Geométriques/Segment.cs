using UnityEngine;

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
        return Vector2.Distance(new Vector2(start.x, start.y), new Vector2(end.x, end.y));
    }
}
