using UnityEngine;

namespace Structures_Geométriques.Extensions
{
    public static class PointExtensions
    {
        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3(point.x, point.y, point.z);
        }
    }
}