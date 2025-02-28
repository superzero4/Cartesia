using Structures_Geométriques.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace Renderers
{
    public class PolygonRenderer : MonoBehaviour, IRenderer<Polygone>
    {
        [SerializeField] private MeshFilter _filter;

        private void Awake()
        {
            Assert.IsTrue(_filter.gameObject.GetComponent<MeshRenderer>() != null);
        }

        public void SetView(Polygone data)
        {
            Mesh mesh = new Mesh();
            int initialCount = data.sommets.Count;
            Vector3[] vectices = new Vector3[initialCount + 1];
            int[] triangles = new int[initialCount * 3];
            Vector3 center = Vector3.zero;
            for (int i = 0; i < initialCount; i++)
            {
                vectices[i] = data.sommets[i].ToVector3();
                center += vectices[i];
                //We draw a triangle using, center vertex (not currently calculated but we know we'll append it at the end), current vertex and next vertex
                triangles[i * 3] = i;
                triangles[i * 3+1] = (i+1)%initialCount;
                triangles[i * 3+2] = initialCount;
            }
            //Gravity center, we assume the polygon is convex
            vectices[initialCount] = center / initialCount;
            mesh.vertices = vectices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            _filter.mesh = mesh;
        }

        public void ToggleVisibility(bool visible)
        {
            _filter.gameObject.SetActive(visible);
        }
    }
}