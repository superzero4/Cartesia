using UnityEngine;

namespace Geometry
{
    [CreateAssetMenu(fileName = "RelativeGeometry", menuName = "Geometry/RelativeGeometry", order = 0)]
    public class RelativeGeometrySO : ScriptableObject
    {
        [SerializeField] private RelativeGeometry _relativeGeometry;

        public RelativeGeometry RelativeGeometry => _relativeGeometry;
    }
}