using System.Collections.Generic;
using Renderers;
using UnityEngine;

namespace UI
{
    public class FaceElement : Element<IndexedPolygon>
    {
        [SerializeField] private IncrementalIndex _pointPrefab;
        [SerializeField] private Transform _pointContainer;
        [SerializeField] private List<IncrementalIndex> _points;
        public override void SetData(IndexedPolygon data, int index)
        {
            base.SetData(data, index);
            IRendererHelpers.InstantiateRenderersAndRefresh(_points, Data.Data.indexes, _pointPrefab, _pointContainer);
        }

        public override void RefreshView()
        {
            
        }
    }
}