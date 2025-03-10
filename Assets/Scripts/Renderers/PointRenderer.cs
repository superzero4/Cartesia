using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class PointRenderer : MonoBehaviour,IRenderer<Point>
    {
        [SerializeField] private GameObject _root;
        private Point _data;

        public void SetView()
        {
            _root.transform.position = Data.ToVector3();
        }

        public Point Data
        {
            get => _data;
            set => _data = value;
        }

        public void ToggleVisibility(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }
    }
}