using System.Collections.Generic;
using Structures_Geométriques;

public interface IGeometries
{
    IEnumerable<Point> Points { get; }
    IEnumerable<Segment> Segments { get; }
    IEnumerable<Polygone> Polygones { get; }
    IEnumerable<Polyedre> Polyedres { get; }
    int PolygonesCount { get; }
    int PointsCount { get; }
    void AddPoint(Point point);
    void AddIndex(int geomPolygonesCount, int geomPointsCount);
}