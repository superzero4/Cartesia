using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Indexes
{
    public List<int> indexes;
    public Indexes()
    {
        indexes = new();
    }
}

[Serializable]
public class RelativeGeometry
{
    public RelativeGeometry(RelativeGeometry other)
    {
        //Deep copying floats
        _points = other._points.Select(p => new Point(p.x, p.y, p.z)).ToList();
        //Deep copying components of a Vector2Int
        _lines = other._lines.Select(l => new IndexedSegment(l.Data.x, l.Data.y)).ToArray();
        //Deep copying in a new list, indexes=ints of the other list
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
        //Deep copying in a new list, indexes=ints of the other list
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