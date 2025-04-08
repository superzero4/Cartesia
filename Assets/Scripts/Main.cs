using System;
using System.Collections.Generic;
using System.Linq;
using Control;
using Geometry;
using Renderers;
using Structures_Geométriques;
using UnityEngine;
using UnityEngine.Serialization;

public class Main : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private bool _showPolygons;
    [SerializeField] private bool _showPolyedres;
    [SerializeField, Range(0.01f, 3f)] private float _scale;

    [SerializeField] private Point _offset;

    [FormerlySerializedAs("_geometries")] [Header("References")] [SerializeField]
    private UIManager _uiManager;

    [SerializeField, Tooltip("Used only if not overriden to define the initial state of the geometry")]
    private RelativeGeometry _initState;

    [SerializeField, Tooltip("Null means we use the serialized init state just above")]
    private RelativeGeometrySO _initStateOverride;

    private IGeometries _runtimeGeometry;

    [Header("Prefabs")] [SerializeField] private PointRenderer _pointPrefab;
    [SerializeField] private SegmentRenderer _segmentPrefab;
    [SerializeField] private PolygonRenderer _polygonPrefab;
    [SerializeField] private PolyedreRenderer _polyedrePrefab;

    [Header("Existing")] [SerializeField] private List<PointRenderer> _pointRenderers;
    [SerializeField] private List<SegmentRenderer> _segmentRenderers;
    [SerializeField] private List<PolygonRenderer> _polygonRenderers;
    [SerializeField] private List<PolyedreRenderer> _polyedreRenderers;

    public IGeometries RuntimeGeometry => _runtimeGeometry;

    private void Awake()
    {
        //We use deep copy to avoid modifying the original geometry in the scriptable object if we use it
        if (_initStateOverride != null)
            _initState = new RelativeGeometry(_initStateOverride.RelativeGeometry);
        _uiManager.SetGeometryToDisplay(_initState);
        var _tools = new ToolsUI(_initState);
        UiEvents.lineEvent.AddListener(_tools.UpdatePointInLine);
        UiEvents.polygonEvent.AddListener(_tools.UpdatePointInPolygon);
        UiEvents.polyedreEvent.AddListener(_tools.UpdatePointInPolyedre);
        _runtimeGeometry = new RelativeToAbsoluteGeometry(_initState, _scale, _offset);
    }

    private void Update()
    {
        //TODO link with changes to the geometry
        Refresh();
    }

    private void Refresh()
    {
        _uiManager.RefreshAll();
        _runtimeGeometry.Refresh();
        IRendererHelpers.InstantiateRenderersAndRefresh(_pointRenderers, _runtimeGeometry.Points, _pointPrefab,
            transform);
        IRendererHelpers.InstantiateRenderersAndRefresh(_segmentRenderers, _runtimeGeometry.Segments, _segmentPrefab,
            transform);
        IRendererHelpers.InstantiateRenderersAndRefresh(_polygonRenderers,
            _showPolygons ? _runtimeGeometry.Polygones : Enumerable.Empty<Polygone>(), _polygonPrefab,
            transform);
        IRendererHelpers.InstantiateRenderersAndRefresh(_polyedreRenderers,
            _showPolyedres ? _runtimeGeometry.Polyedres : Enumerable.Empty<Polyedre>(), _polyedrePrefab,
            transform);
    }
}