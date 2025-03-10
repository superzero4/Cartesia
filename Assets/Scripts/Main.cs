using System;
using System.Collections.Generic;
using Renderers;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField] private Geometries _geometries;

    [SerializeField] private PointRenderer _pointPrefab;
    [SerializeField] private SegmentRenderer _segmentPrefab;
    [SerializeField] private PolygonRenderer _polygonPrefab;
    [SerializeField] private List<PointRenderer> _pointRenderers;
    [SerializeField] private List<SegmentRenderer> _segmentRenderers;
    [SerializeField] private List<PolygonRenderer> _polygonRenderers;

    private void Start()
    {
        _geometries.ResetShapes();
    }

    private void Update()
    {
        int i = 0;
        for (; i < _geometries.Points.Count; i++)
        {
            if (i >= _pointRenderers.Count)
            {
                var rend = Instantiate(_pointPrefab, transform);
                _pointRenderers.Add(rend);
            }
            _pointRenderers[i].ToggleVisibility(true);
            _pointRenderers[i].Data = _geometries.Points[i];
            _pointRenderers[i].RefreshView();
        }
        for(; i < _pointRenderers.Count; i++)
        {
            _pointRenderers[i].ToggleVisibility(false);
        }
        var segments = _geometries.Segments;
        i = 0;
        for (; i < segments.Count; i++)
        {
            if (i >= _segmentRenderers.Count)
            {
                var rend = Instantiate(_segmentPrefab, transform);
                _segmentRenderers.Add(rend);
            }
            _segmentRenderers[i].ToggleVisibility(true);
            _segmentRenderers[i].Data = segments[i];
            _segmentRenderers[i].RefreshView();
        }
        for(; i < _segmentRenderers.Count; i++)
        {
            _segmentRenderers[i].ToggleVisibility(false);
        }
        var polygons = _geometries.Polygones;
        i = 0;
        for (; i < polygons.Count; i++)
        {
            if (i >= _polygonRenderers.Count)
            {
                var rend = Instantiate(_polygonPrefab, transform);
                _polygonRenderers.Add(rend);
            }
            _polygonRenderers[i].ToggleVisibility(true);
            _polygonRenderers[i].Data = polygons[i];
            _polygonRenderers[i].RefreshView();
        }
    }
}