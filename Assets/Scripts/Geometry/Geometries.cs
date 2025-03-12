using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Structures_Geométriques;

[Serializable]
public class Indexes
{
    public List<int> indexes;
}

[CreateAssetMenu(fileName = "Geometries", menuName = "Geometries")]
public class Geometries : ScriptableObject, IGeometries
{
    [Header("Data")] [SerializeField] private List<Point> _points;

    [SerializeField, Tooltip("Index of points in the list")]
    private IndexedSegment[] _lines;

    [SerializeField, Tooltip("Index of points in the list")]
    private List<IndexedPolygon> _polygons;

    [SerializeField, Tooltip("Index of points in the list")]
    private List<IndexedPolyedre> _polyedres;

    public IEnumerable<Point> Points => _points;
    public int PointsCount => _points.Count;

    public void Init()
    {
        foreach (var indexedSegment in _lines)
            indexedSegment.SetPoints(_points);
        foreach (var indexedPolygon in _polygons)
            indexedPolygon.SetPoints(_points);
        foreach (var indexedPolyedre in _polyedres)
            indexedPolyedre.SetPolygons(_polygons);
    }

    public void AddPoint(Point p)
    {
        _points.Add(p);
        foreach (var indexedSegment in _lines)
            indexedSegment.MarkDirty();

        foreach (var indexedPolygon in _polygons)
            indexedPolygon.MarkDirty();
        foreach (var indexedPolyedre in _polyedres)
            indexedPolyedre.MarkDirty();
    }

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