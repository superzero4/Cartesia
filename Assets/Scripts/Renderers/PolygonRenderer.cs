using Control;
using Structures_Geométriques.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;


namespace Renderers
{
    public class PolygonRenderer : SerializedDataRenderer<Polygone>
    {
        [SerializeField] private MeshFilter _filter;
        [SerializeField] private SphereCollider _collider;
        public Vector3 GravityCenter;


        Mesh mesh;
        private void Awake()
        {
            mesh = new Mesh();
            mesh.MarkDynamic();
            _filter.mesh = mesh;
            Assert.IsTrue(_filter.gameObject.GetComponent<MeshRenderer>() != null);
        }
        public override void RefreshView()
        {
            int initialCount = Data.sommets.Count;
            Vector3[] vectices = new Vector3[initialCount + 1];
            int[] triangles = new int[initialCount * 3];
            Vector3 center = Vector3.zero;
            for (int i = 0; i < initialCount; i++)
            {
                vectices[i] = Data.sommets[i].ToVector3();
                center += vectices[i];
                //We draw a triangle using, center vertex (not currently calculated but we know we'll append it at the end), current vertex and next vertex
                var v1 = i;
                var v2 = (i + 1) % initialCount;
                var v3 = initialCount;
                triangles[i * 3 + 0] = v1;
                triangles[i * 3 + 1] = v2;
                triangles[i * 3 + 2] = v3;
                
                
                
            }
            //Gravity center, we assume the polygon is convex
            GravityCenter= center / initialCount;
            vectices[initialCount] = GravityCenter;
            _collider.transform.position = GravityCenter;
            mesh.vertices = vectices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.MarkModified();
        }

        public override void ToggleVisibility(bool visible)
        {
            _filter.gameObject.SetActive(visible);
        }

        public void Visibiliy(SelectionMode OnChangeMode)
        {
            if (OnChangeMode==SelectionMode.Face)
            {
                _collider.gameObject.SetActive(true);
            }
      
            else
            {
                _collider.gameObject.SetActive(true);
            }
        }
    }
}