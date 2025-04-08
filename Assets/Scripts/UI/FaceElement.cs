using System.Collections.Generic;
using System.Linq;
using Renderers;
using UnityEngine;

namespace UI
{
    public class FaceElement : Element<IndexedPolygon>
    {
        [SerializeField] private IncrementalIndex _pointPrefab;
        [SerializeField] private Transform _pointContainer;
        private List<IncrementalIndex> _points = new();
        public void Awake()
        {
            _points = _pointContainer.GetComponentsInChildren<IncrementalIndex>(true).ToList();
        }
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