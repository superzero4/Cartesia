using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;

[CreateAssetMenu(fileName = "Geometries", menuName = "Geometries")]
public class Geometries : ScriptableObject
{
    [Header("Data")] [SerializeField] private List<Point> _points;

    [SerializeField, Tooltip("Index of points in the list")]
    private Vector2Int[] _lines;

    [SerializeField] private List<Indexes> _polygons;

    [Serializable]
    private class Indexes
    {
        public List<int> indexes;
    }

    private List<Segment> _segments = null;
    private List<Polygone> _polygones = null;

    public List<Point> Points => _points;

    public void ResetShapes()
    {
        _segments = null;
        _polygones = null;
    }

    //TODO add some kind of dirty on the _lines list to ensure the segment list are updated properly
    public List<Segment> Segments =>  GenerateSegments();
    public List<Polygone> Polygones =>  GeneratePolygones();
    public void AddIndex(int polygon, int newIndex)
    {
        _polygons[polygon].indexes.Add(newIndex);
    }

    private List<Polygone> GeneratePolygones()
    {
        return _polygones = _polygons
            .Select(poly => new Polygone(
                poly.indexes
                    .Where(index => isIndexValid(index))
                    .Select(index => _points[index]).ToList()))
            .ToList();
    }

    private bool isIndexValid(int index) => index >= 0 && index < _points.Count;

    private List<Segment> GenerateSegments()
    {
        return _segments = _lines
            .Where(point => point.x != point.y && isIndexValid(point.x) && isIndexValid(point.y))
            .Select(point => new Segment(_points[point.x], _points[point.y]))
            .ToList();
    }
}