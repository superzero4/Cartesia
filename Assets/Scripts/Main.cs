using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Renderers;
using Structures_Geométriques;
using UnityEngine;
using UnityEngine.Serialization;

public class Main : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private bool _showPolygons; 
    [SerializeField] private bool _showPolyedres;
    [SerializeField,Range(0.01f,3f)] private float _scale;

    [SerializeField]
    private Point _offset;

    [FormerlySerializedAs("_geometries")] 
    [Header("References")] 
    [SerializeField,Tooltip("Used only if not overriden to define the initial state of the geometry")]
    private RelativeGeometry _initState;
    [SerializeField,Tooltip("Null means we use the serialized init state just above")] private RelativeGeometrySO _initStateOverride;
    private IGeometries _runtimeGeometry;
    
    [Header("Prefabs")] 
    [SerializeField] private PointRenderer _pointPrefab;
    [SerializeField] private SegmentRenderer _segmentPrefab;
    [SerializeField] private PolygonRenderer _polygonPrefab;
    [SerializeField] private PolyedreRenderer _polyedrePrefab;
    
    [Header("Existing")] 
    [SerializeField] private List<PointRenderer> _pointRenderers;
    [SerializeField] private List<SegmentRenderer> _segmentRenderers;
    [SerializeField] private List<PolygonRenderer> _polygonRenderers;
    [SerializeField] private List<PolyedreRenderer> _polyedreRenderers;

    public IGeometries RuntimeGeometry => _runtimeGeometry;

    private void Awake()
    {
        //We use deep copy to avoid modifying the original geometry in the scriptable object if we use it
        if(_initStateOverride != null)
            _initState = new RelativeGeometry(_initStateOverride.RelativeGeometry);
        _runtimeGeometry = new RelativeToAbsoluteGeometry(_initState, _scale, _offset);
    }

    private void Update()
    {
        IRenderer<Point>.InstantiateRenderersAndRefresh(_pointRenderers, _runtimeGeometry.Points, _pointPrefab, transform);
        IRenderer<Segment>.InstantiateRenderersAndRefresh(_segmentRenderers, _runtimeGeometry.Segments, _segmentPrefab,
            transform);
        IRenderer<Polygone>.InstantiateRenderersAndRefresh(_polygonRenderers,
            _showPolygons ? _runtimeGeometry.Polygones : Enumerable.Empty<Polygone>(), _polygonPrefab,
            transform);
        IRenderer<Polyedre>.InstantiateRenderersAndRefresh(_polyedreRenderers, _showPolyedres ? _runtimeGeometry.Polyedres : Enumerable.Empty<Polyedre>(), _polyedrePrefab,
            transform);
    }
}