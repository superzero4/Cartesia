using System;
using Structures_Geométriques.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Renderers
{
    public class SegmentRenderer : SerializedDataRenderer<Segment>
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField, Range(0, .1f)] private float _width = .05f;
        [SerializeField] private BoxCollider _collider;
        [SerializeField] private bool _showPoints = false;
        [SerializeField] public PointRenderer _start;
        [SerializeField] public PointRenderer _end;

        private void Awake()
        {
            _lineRenderer.widthMultiplier = _width;
        }

        public override void RefreshView()
        {
            _lineRenderer.SetPositions(new Vector3[] { Data.start.ToVector3(), Data.end.ToVector3() });
            _start.SetData(Data.start, -1);
            _start.RefreshView();
            _end.SetData(Data.end, -1);
            _end.RefreshView();
            TogglePointVisibility(_showPoints);
            _collider.size =
                new Vector3(_width, _width, Vector3.Distance(Data.start.ToVector3(), Data.end.ToVector3()));
            _collider.transform.position = (Data.start.ToVector3() + Data.end.ToVector3()) / 2;
            _collider.transform.LookAt(Data.end.ToVector3());
        }

        private void TogglePointVisibility(bool value = true)
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