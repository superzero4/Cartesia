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
        
        private Segment _data;

        public Segment Data
        {
            get => _data;
            set => _data = value;
        }

        public void SetView()
        {
            _lineRenderer.SetPositions(new Vector3[] { Data.start.ToVector3(), Data.end.ToVector3() });
            _start.Data = Data.start;
            _start.SetView();
            _end.Data = Data.end;
            _end.SetView();
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