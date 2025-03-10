using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class PointRenderer : MonoRenderer<Point>
    {
        [SerializeField] private GameObject _root;

        public override void RefreshView()
        {
            _root.transform.position = Data.ToVector3();
        }

        public override void ToggleVisibility(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }
    }
}