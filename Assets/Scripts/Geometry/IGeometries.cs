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
    //void AddPointToPolygon(int polygoneIndex, int pointIndex);
    void AddSegment(int p1, int p2);
    void AddPolygon();
    public void AddPolyedre();
    public void Refresh();
}