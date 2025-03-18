using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Indexes
{
    public List<int> indexes;
}

[Serializable]
public class RelativeGeometry
{
    public RelativeGeometry(RelativeGeometry other)
    {
        _points = other._points.Select(p => new Point(p.x, p.y, p.z)).ToList();
        _lines = other._lines.Select(l => new IndexedSegment() { Data = l.Data }).ToArray();
        _polygons = other._polygons.Select(p =>
        {
            return new IndexedPolygon()
            {
                Data = new Indexes()
                {
                    indexes = new List<int>(p.Data.indexes)
                }
            };
        }).ToList();
        _polyedres = other._polyedres.Select(p =>
        {
            return new IndexedPolyedre()
            {
                Data = new Indexes()
                {
                    indexes = new List<int>(p.Data.indexes)
                }
            };
        }).ToList();
    }

    [Header("Data")] [SerializeField] public List<Point> _points;

    [SerializeField, Tooltip("Index of points in the list")]
    public IndexedSegment[] _lines;

    [SerializeField, Tooltip("Index of points in the list")]
    public List<IndexedPolygon> _polygons;

    [SerializeField, Tooltip("Index of points in the list")]
    public List<IndexedPolyedre> _polyedres;
}