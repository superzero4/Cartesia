using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class SegmentRenderer : MonoBehaviour, IRenderer<Segment>
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private bool _showPoints = false;
        [SerializeField] private PointRenderer _start;
        [SerializeField] private PointRenderer _end;

        public void SetView(Segment data)
        {
            _lineRenderer.SetPositions(new Vector3[] { data.start.ToVector3(), data.end.ToVector3() });
            _start.SetView(data.start);
            _end.SetView(data.end);
            TogglePointVisibility(_showPoints);
        }

        private void TogglePointVisibility(bool value = true)
        {
            _start.ToggleVisibility(value);
            _end.ToggleVisibility(value);
        }

        public void ToggleVisibility(bool visible)
        {
            _lineRenderer.gameObject.SetActive(visible);
            TogglePointVisibility(visible);
        }
    }
}