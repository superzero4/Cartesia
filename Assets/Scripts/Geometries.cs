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

public abstract class Dirty<TSource,TResult>
{
    private TSource data;
    private TResult result;
    private bool dirty = true;
    public void Edit(TSource newData)
    {
        data = newData;
        dirty = true;
    }
    public TResult Value => dirty ? (result = Compute(data)) : result;

    public abstract TResult Compute(TSource tSource);
}
[Serializable]
class IndexedPolygon : Dirty<Indexes, Polygone>
{
    List<Point> _points;
    public IndexedPolygon(List<Point> points)
    {
        _points = points;
    })
    public override Polygone Compute(Indexes tSource)
    {
        return new Polygone(
                tSource.indexes
                    .Where(index => index>=0 && index < _points.Count)
                    .Select(index => _points[index]).ToList()))
    }
}
[Serializable]
class IndexedSegment : Dirty<Vector2Int, Segment>
{
    List<Point> _points;
    public IndexedSegment(List<Point> points)
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

    [SerializeField] private List<IndexedPolygon> _polygons;
    public IEnumerable<Point> Points => _points;

    public void ResetShapes()
    {
        _segments = null;
        _polygones = null;
    }

    //TODO add some kind of dirty on the _lines list to ensure the segment list are updated properly
    public IEnumerable<Segment> Segments =>  _lines.Select(indexedSegment => indexedSegment.Value);
    public IEnumerable<Polygone> Polygones =>  _polygons.Select(indexedPolygon => indexedPolygon.Value);
}