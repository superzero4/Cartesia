using System;
using System.Collections.Generic;
using System.Linq;
using Renderers;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class Element<T> : SerializedDataRenderer<T>
{
    [SerializeField] private IndexText _text;
    public IndexText Text => _text;

    public override void SetData(T data, int index)
    {
        base.SetData(data, index);
        _text.SetIndex(index);
    }

    public override void RefreshView()
    {
        // Implement any additional refresh logic here if needed
    }

    public override void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}

public abstract class Tab<T> : MonoBehaviour
{
    [SerializeField] private Element<T> _prefab;
    [SerializeField] private LayoutGroup _parent;
    private List<Element<T>> _elements;

    private void Awake()
    {
        _elements = _parent.GetComponentsInChildren<Element<T>>(true).ToList();
    }

    public void SetData(IEnumerable<T> data)
    {
        IRendererHelpers.InstantiateRenderersAndRefresh(_elements, data, _prefab, _parent.transform);
    }
}

public abstract class ListIndexedElement<T> : Element<T>
{
    [SerializeField] private IncrementalIndex _pointPrefab;
    [SerializeField] private Transform _pointContainer;
    private List<IncrementalIndex> _points;

    private void Awake()
    {
        FindSamples();
    }

    private void FindSamples()
    {
        _points = _pointContainer.GetComponentsInChildren<IncrementalIndex>(true).ToList();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void SetData(T data, int index)
    {
        if (_points == null)
            FindSamples();
        base.SetData(data, index);
        IRendererHelpers.InstantiateRenderersAndRefresh(_points, Indexes(data), _pointPrefab, _pointContainer,
            (point, i) =>
            {
                var args = new UiEvents.IndexListEventData()
                {
                    objectIndex = Index,
                    indexInObject = point.Index,
                };
                point.SetEvent(Event(), args);
            });
    }

    protected abstract UnityEvent<UiEvents.IndexListEventData> Event();


    protected abstract IEnumerable<int> Indexes(T data);

    public override void RefreshView()
    {
        IRendererHelpers.InstantiateRenderersAndRefresh(_points, Indexes(Data), _pointPrefab, _pointContainer);
    }
}