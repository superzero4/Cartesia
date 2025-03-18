using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IndexedSegment : Dirty<Vector2Int, Segment>
{
    List<Point> _points;

    public void SetPoints(List<Point> points)
    {
        _points = points;
        this.MarkDirty();
    }

    public override Segment Compute(Vector2Int tSource)
    {
        return new Segment(_points[tSource.x], _points[tSource.y]);
    }
}