using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Renderers;
using Structures_Geométriques;
using UnityEngine;

public class Main : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private bool _showPolygons;

    [Header("References")] [SerializeField]
    private Geometries _geometries;

    [Header("Prefabs")] [SerializeField] private PointRenderer _pointPrefab;
    [SerializeField] private SegmentRenderer _segmentPrefab;
    [SerializeField] private PolygonRenderer _polygonPrefab;
    [SerializeField] private PolyedreRenderer _polyedrePrefab;
    [Header("Existing")] [SerializeField] private List<PointRenderer> _pointRenderers;
    [SerializeField] private List<SegmentRenderer> _segmentRenderers;
    [SerializeField] private List<PolygonRenderer> _polygonRenderers;
    [SerializeField] private List<PolyedreRenderer> _polyedreRenderers;

    private void Start()
    {
        _geometries.Init();
    }

    private void Update()
    {
        IRenderer<Point>.InstantiateRenderersAndRefresh(_pointRenderers, _geometries.Points, _pointPrefab, transform);
        IRenderer<Segment>.InstantiateRenderersAndRefresh(_segmentRenderers, _geometries.Segments, _segmentPrefab,
            transform);
        IRenderer<Polygone>.InstantiateRenderersAndRefresh(_polygonRenderers,
            _showPolygons ? _geometries.Polygones : Enumerable.Empty<Polygone>(), _polygonPrefab,
            transform);
        IRenderer<Polyedre>.InstantiateRenderersAndRefresh(_polyedreRenderers, _geometries.Polyedres, _polyedrePrefab,
            transform);
    }
}