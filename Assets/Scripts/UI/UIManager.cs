using System;
using System.Linq;
using Control;
using Structures_Geométriques;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public static class UiEvents
{
    public class EventDataBase
    {
        public int objectIndex;
        public bool up;
    }

    public class LineEventData : EventDataBase
    {
        public bool isFirst;
    }

    public class PolygonEventData : EventDataBase
    {
        public int indexInObject;
    }

    public class PolyedreEventData : PolygonEventData
    {
        public int indexInFace;
    }

    public class LineEvent : UnityEvent<LineEventData>
    {
    }

    public class PolygonEvent : UnityEvent<PolygonEventData>
    {
    }

    public class PolyedreEvent : UnityEvent<PolyedreEventData>
    {
    }

    public static LineEvent lineEvent = new();
    public static PolygonEvent polygonEvent = new();
    public static PolyedreEvent polyedreEvent = new();
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private Tab[] _tabs;
    [SerializeField] private Tab<Point> _pointTab;
    [SerializeField] private Tab<IndexedSegment> _segmentTab;
    [SerializeField] private Tab<IndexedPolygon> _polygonTab;
    [SerializeField] private Tab<IndexedPolyedre> _polygonalChainTab;
    [SerializeField] private GameObject[] _tabContents;
    [SerializeField] private SelectionModeControl _selection;

    [FormerlySerializedAs("geometry")] [SerializeField]
    private RelativeGeometry _geometry;

    private SelectionMode[] values;

    private void Awake()
    {
        _selection.OnChangeMode.AddListener(OnSelectionUpdated);
        values = Enum.GetValues(typeof(SelectionMode)).Cast<SelectionMode>().Skip(1).ToArray();
        for (int i = 0; i < _tabs.Length; i++)
            _tabs[i].Init(values[i].ToString());
    }

    public void SetGeometryToDisplay(RelativeGeometry geometry)
    {
        this._geometry = geometry;
    }

    public void OnSelectionUpdated(SelectionMode mode)
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            bool active = i == ((int)mode) - 1;
            _tabs[i].interactable = active;
            _tabContents[i].gameObject.SetActive(active);
        }

        Refresh(mode);
    }

    public void RefreshAll()
    {
        foreach (var val in values)
            Refresh(val);
    }

    public void Refresh(SelectionMode mode)
    {
        switch (mode)
        {
            case SelectionMode.Point:
                _pointTab.SetData(_geometry._points);
                break;
            case SelectionMode.Line:
                _segmentTab.SetData(_geometry._lines);
                break;
            case SelectionMode.Face:
                _polygonTab?.SetData(_geometry._polygons);
                break;
            case SelectionMode.Object:
                _polygonalChainTab?.SetData(_geometry._polyedres);
                break;
        }
    }
}