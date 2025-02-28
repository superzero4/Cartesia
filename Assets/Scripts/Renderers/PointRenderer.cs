using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class PointRenderer : MonoBehaviour,IRenderer<Point>
    {
        [SerializeField] private GameObject _root;

        public void SetView(Point data)
        {
            _root.transform.position = data.ToVector3();
        }

        public void ToggleVisibility(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }
    }
}