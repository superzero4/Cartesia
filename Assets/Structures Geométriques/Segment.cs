using UnityEngine;

public struct Segment
{
    public Point2D start, end;

    public Segment(Point2D start, Point2D end)
    {
        this.start = start;
        this.end = end;
    }

    public float Length()
    {
        return Vector2.Distance(new Vector2(start.x, start.y), new Vector2(end.x, end.y));
    }
}
