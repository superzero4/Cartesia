using System.Collections.Generic;
using Structures_Geométriques;
using UnityEngine;
using Control;

namespace Renderers
{
    public class PolyedreRenderer : SerializedDataRenderer<Polyedre>
    {
        [SerializeField] private PolygonRenderer _polygonPrefab;
        [SerializeField] List<PolygonRenderer> _polygoneRenderers = new List<PolygonRenderer>();
        [SerializeField] private SphereCollider _collider;
        public Vector3 PolyedreCenter;
        public override void RefreshView()
        {
            IRendererHelpers.InstantiateRenderersAndRefresh(_polygoneRenderers, Data.Faces, _polygonPrefab, transform);
            Vector3 TotalCenter = Vector3.zero;
            int TotalVertice = 0;
            
            //Calcul du centre en faisant moyenne de toutes les coordonnees de ses sommets
            foreach (var polygonRenderer in _polygoneRenderers)
            {
                TotalCenter += polygonRenderer.GravityCenter;
                TotalVertice++;
            }
            PolyedreCenter = TotalCenter / TotalVertice;
            _collider.transform.position = PolyedreCenter;

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
            if (OnChangeMode == SelectionMode.Object)
            {
                _collider.gameObject.SetActive(true);
            }
            
            else
            {
                _collider.gameObject.SetActive(false);
            }
        }
    }
}