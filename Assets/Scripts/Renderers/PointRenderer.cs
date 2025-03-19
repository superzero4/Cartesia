using Structures_Geométriques.Extensions;
using UnityEngine;

namespace Renderers
{
    public class PointRenderer : SerializedDataRenderer<Point>
    {
        public new int Index
        {
            get
            {
                Debug.LogWarning("PointRenderer indexes are not necessary, the reference is enough you don't need the index to acces back it's position in the list (all the index stuff is basically juste to avoid going through the list to find the index of the shapes but for points we don't use indirect indexed/relative geometries therefore reference is enough for all modifications)");
                return -1;
            }
        }
        public new void SetData(Point data, int index)
        {
            base.SetData(data, -1);
        }
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