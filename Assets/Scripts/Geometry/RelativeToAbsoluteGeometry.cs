using System;
using System.Collections.Generic;
using System.Linq;
using Structures_Geométriques;
using UnityEngine.Serialization;



public class RelativeToAbsoluteGeometry : IGeometries
{
    RelativeGeometry _relativeGeometry = null;
    private List<Point> _points;
    public IEnumerable<Point> Points => _points;
    public int PointsCount => _points.Count;

    public RelativeToAbsoluteGeometry(RelativeGeometry geom, float scale, Point offset)
    {
        _relativeGeometry = geom;
        _points = _relativeGeometry._points;
        _points.ForEach(p =>
        {
            p.x *= scale;
            p.x += offset.x;
            p.y *= scale;
            p.y += offset.y;
            p.z *= scale;
            p.z += offset.z;
        });
        foreach (var indexedSegment in _relativeGeometry._lines)
            indexedSegment.SetPoints(_points);
        foreach (var indexedPolygon in _relativeGeometry._polygons)
            indexedPolygon.SetPoints(_points);
        foreach (var indexedPolyedre in _relativeGeometry._polyedres)
            indexedPolyedre.SetPolygons(_relativeGeometry._polygons);
    }

    public void AddPoint(Point p)
    {
        foreach (var indexedSegment in _lines)
            indexedSegment.MarkDirty();
        foreach (var indexedPolygon in _polygons)
            indexedPolygon.MarkDirty();
        foreach (var indexedPolyedre in _polyedres)
            indexedPolyedre.MarkDirty();
    }

    private IndexedSegment[] _lines => _relativeGeometry._lines;
    private List<IndexedPolygon> _polygons => _relativeGeometry._polygons;
    private List<IndexedPolyedre> _polyedres => _relativeGeometry._polyedres;
    //TODO add some kind of dirty on the _lines list to ensure the segment list are updated properly
    public IEnumerable<Segment> Segments => _lines.Select(indexedSegment => indexedSegment.Value);
    public int SegmentsCount => _lines.Length;
    public IEnumerable<Polygone> Polygones => _polygons.Select(indexedPolygon => indexedPolygon.Value);
    public int PolygonesCount => _polygons.Count;
    public IEnumerable<Polyedre> Polyedres => _polyedres.Select(indexedPolyedre => indexedPolyedre.Value);
    public int PolyedresCount => _polyedres.Count;

    public void AddIndex(int count, int i)
    {
        var poly = _polygons[count];
        poly.Data.indexes.Add(i);
        poly.MarkDirty();
        //We dirty all polygedre that could depends on this one
        foreach (var indexedPolyedre in _polyedres)
            indexedPolyedre.MarkDirty();
    }
}