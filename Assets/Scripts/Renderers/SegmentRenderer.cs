using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class SegmentRenderer : SerializedDataRenderer<Segment>
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private bool _showPoints = false;
        [SerializeField] public PointRenderer _start;
        [SerializeField] public PointRenderer _end;

        public override void RefreshView()
        {
            _lineRenderer.SetPositions(new Vector3[] { Data.start.ToVector3(), Data.end.ToVector3() });
            _start.SetData(Data.start,-1);
            _start.RefreshView();
            _end.SetData(Data.end,-1);
            _end.RefreshView();
            TogglePointVisibility(_showPoints);
        }

        private  void TogglePointVisibility(bool value = true)
        {
            _start.ToggleVisibility(value);
            _end.ToggleVisibility(value);
        }

        public override void ToggleVisibility(bool visible)
        {
            _lineRenderer.gameObject.SetActive(visible);
            TogglePointVisibility(visible);
        }
    }
}