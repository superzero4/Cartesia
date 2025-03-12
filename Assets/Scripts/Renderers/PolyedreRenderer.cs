using System.Collections.Generic;
using Structures_Geométriques;
using UnityEngine;

namespace Renderers
{
    public class PolyedreRenderer : SerializedDataRenderer<Polyedre>
    {
        [SerializeField] private PolygonRenderer _polygonPrefab;
        [SerializeField] List<PolygonRenderer> _polygoneRenderers = new List<PolygonRenderer>();
        public override void RefreshView()
        {
            IRenderer<Polygone>.InstantiateRenderersAndRefresh(_polygoneRenderers, Data.Faces, _polygonPrefab, transform);
        }

        public override void ToggleVisibility(bool visible)
        {
            foreach (var polygoneRenderer in _polygoneRenderers)
            {
                polygoneRenderer.ToggleVisibility(visible);
            }
        }
    }
}