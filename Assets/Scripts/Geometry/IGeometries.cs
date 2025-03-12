using System.Collections.Generic;

public interface IGeometries
{
    IEnumerable<Point> Points { get; }
    IEnumerable<Segment> Segments { get; }
    IEnumerable<Polygone> Polygones { get; }
}