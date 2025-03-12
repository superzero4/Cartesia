using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NUnit.Framework;

public interface IGeometries
{
    IEnumerable<Point> Points { get; }
    IEnumerable<Segment> Segments { get; }
    IEnumerable<Polygone> Polygones { get; }
}

[Serializable]
public abstract class Dirty<TSource, TResult>
{
    [SerializeField] private TSource data;

    public TSource Data
    {
        set
        {
            data = value;
            MarkDirty();
        }
        get => data;
    }

    //[SerializeField]
    private TResult result;

    //[SerializeField]
    protected bool dirty = true;

    public void MarkDirty()
    {
        dirty = true;
    }

    public void Edit(TSource newData)
    {
        data = newData;
        MarkDirty();
    }

    public TResult Value => dirty ? (result = Compute(data)) : result;

    public abstract TResult Compute(TSource tSource);
}

[Serializable]
class IndexedPolygon : Dirty<Indexes, Polygone>
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

[Serializable]
class IndexedSegment : Dirty<Vector2Int, Segment>
{
    List<Point> _points;

    public void SetPoints(List<Point> points)
    {
        _points = points;
    }

    public override Segment Compute(Vector2Int tSource)
    {
        return new Segment(_points[tSource.x], _points[tSource.y]);
    }
}

[Serializable]
class Indexes
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

    public IEnumerable<Point> Points => _points;
    public int PointsCount => _points.Count;

    public void Init()
    {
        foreach (var indexedSegment in _lines)
        {
            indexedSegment.SetPoints(_points);
        }
        foreach (var indexedPolygon in _polygons)
        {
            indexedPolygon.SetPoints(_points);
        }
    }
    public void AddPoint(Point p)
    {
        _points.Add(p);
        foreach (var indexedSegment in _lines)
        {
            indexedSegment.MarkDirty();
        }

        foreach (var indexedPolygon in _polygons)
        {
            indexedPolygon.MarkDirty();
        }
    }

    //TODO add some kind of dirty on the _lines list to ensure the segment list are updated properly
    public IEnumerable<Segment> Segments => _lines.Select(indexedSegment => indexedSegment.Value);
    public int SegmentsCount => _lines.Length;
    public IEnumerable<Polygone> Polygones => _polygons.Select(indexedPolygon => indexedPolygon.Value);
    public int PolygonesCount => _polygons.Count;

    public void AddIndex(int count, int i)
    {
        var poly = _polygons[count];
        poly.Data.indexes.Add(i);
        poly.MarkDirty();
    }
}