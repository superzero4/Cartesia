using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class IndexedPolygon : Dirty<Indexes, Polygone>
{
    List<Point> _points;

    public void SetPoints(List<Point> points)
    {
        _points = points;
    }

    public override Polygone Compute(Indexes tSource)
    {
        return new Polygone(
            tSource.indexes
                .Where(index => index >= 0 && index < _points.Count)
                .Select(index => _points[index]));
    }
}