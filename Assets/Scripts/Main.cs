using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
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
    }

    private void Refresh<T, Rend>(List<Rend> renderers, IEnumerable<T> data, Rend prefab)
        where Rend : MonoBehaviour, IRenderer<T>
    {
        int i = 0;
        foreach (var d in data)
        {
            Rend renderer;
            if (i >= renderers.Count)
            {
                Assert.IsTrue(i-renderers.Count == 0);
                renderer = Instantiate(prefab, transform);
                renderers.Add(renderer);
            }
            else
            {
                renderer = renderers[i];
            }

            renderer.ToggleVisibility(true);
            renderer.Data = d;
            renderer.RefreshView();
            i++;
        }

        for (; i < _pointRenderers.Count; i++)
        {
            _pointRenderers[i].ToggleVisibility(false);
        }
    }

    private void Update()
    {
        Refresh(_pointRenderers, _geometries.Points, _pointPrefab);
        Refresh(_segmentRenderers, _geometries.Segments, _segmentPrefab);
        Refresh(_polygonRenderers, _geometries.Polygones, _polygonPrefab);
    }
}