using System;
using System.Linq;
using Control;
using Structures_Geométriques;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    public class IndexListEventData : EventDataBase
    {
        public int indexInObject;
    }

    public class LineEvent : UnityEvent<LineEventData>
    {
    }

    public class IndexListEvent : UnityEvent<IndexListEventData>
    {
    }

    public static LineEvent lineEvent = new();
    public static IndexListEvent polygonEvent = new();
    public static IndexListEvent polyedreEvent = new();
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
    private ColorBlock _active;
    private ColorBlock _inactive;

    private void Awake()
    {
        _selection.OnChangeMode.AddListener(OnSelectionUpdated);
        values = Enum.GetValues(typeof(SelectionMode)).Cast<SelectionMode>().Skip(1).ToArray();
        for (int i = 0; i < _tabs.Length; i++)
            _tabs[i].Init(values[i], _selection);
        var source = _tabs[0].Interactable.colors;
        _active = source;
        _active.normalColor = _active.highlightedColor;

        _inactive = source;
        _inactive.normalColor = source.disabledColor;
        _inactive.highlightedColor = source.normalColor;
        _inactive.pressedColor = source.normalColor;
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
            _tabs[i].Interactable.colors = active ? _active : _inactive;
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