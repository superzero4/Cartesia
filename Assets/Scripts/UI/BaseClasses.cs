using System;
using System.Collections.Generic;
using System.Linq;
using Renderers;
using UnityEngine;

public abstract class Element<T> : SerializedDataRenderer<T>
{
    [SerializeField] private IndexText _text;
    public IndexText Text => _text;
    public override void SetData(T data, int index)
    {
        base.SetData(data, index);
        _text.SetIndex(index);
    }
    public override void ToggleVisibility(bool visible)
    {
        gameObject.SetActive(visible);
    }
}

public abstract class Tab<T> : MonoBehaviour
{
    [SerializeField] private Element<T> _prefab;
    [SerializeField] private Transform _parent;
    private List<Element<T>> _elements;

    private void Awake()
    {
        _elements = _parent.GetComponentsInChildren<Element<T>>(true).ToList();
    }

    public void SetData(IEnumerable<T> data)
    {
        IRendererHelpers.InstantiateRenderersAndRefresh(_elements, data, _prefab, _parent);
    }
}