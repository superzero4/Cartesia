using System.Collections.Generic;
using System.Linq;
using Structures_Geométriques;
using UnityEngine;
using Control;
using Structures_Geométriques.Extensions;

namespace Renderers
{
    public class PolyedreRenderer : SerializedDataRenderer<Polyedre>
    {
        [SerializeField] private PolygonRenderer _polygonPrefab;
        [SerializeField] private bool _reinstantiatePolygons;

        [SerializeField] List<PolygonRenderer> _polygoneRenderers = new List<PolygonRenderer>();
        [SerializeField] private List<PointRenderer> _anchors = new List<PointRenderer>();
        [SerializeField] private PointRenderer _mainAnchor;
        public Vector3 PolyedreCenter;

        public override void RefreshView()
        {
            if (_reinstantiatePolygons)
                IRendererHelpers.InstantiateRenderersAndRefresh(_polygoneRenderers, Data.Faces, _polygonPrefab,
                    transform);
            else
                IRendererHelpers.InstantiateRenderersAndRefresh(_anchors, Data.Faces.Select(f => {
                    return f.GravityCenter;
                    }),
                    _mainAnchor, transform);
            Vector3 TotalCenter = Vector3.zero;
            int TotalVertice = 0;

            //Calcul du centre en faisant moyenne de toutes les coordonnees de ses sommets
            foreach (var face in Data.Faces)
            {
                TotalCenter += face.GravityCenter.ToVector3();
                TotalVertice++;
            }

            PolyedreCenter = TotalCenter / TotalVertice;
            _mainAnchor.transform.position = PolyedreCenter;
        }

        public override void ToggleVisibility(bool visible)
        {
            foreach (var polygoneRenderer in _polygoneRenderers)
            {
                polygoneRenderer.ToggleVisibility(visible);
            }
        }

        public void Visibiliy(SelectionMode OnChangeMode)
        {
            bool val = OnChangeMode == SelectionMode.Object;
            foreach (var anchor in _anchors.Append(_mainAnchor))
                anchor.gameObject.SetActive(val);
        }
    }
}