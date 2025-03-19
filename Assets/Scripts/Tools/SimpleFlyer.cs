using UnityEngine;

namespace Tools
{
    public class SimpleFlyer : MonoBehaviour
    {
        [SerializeField] private Manipulator _manipulator;
        [SerializeField,Range(0.001f, 10f)] private float _speed = 1f;
        [SerializeField,Range(0.001f, 100f)] private float _rotationSpeed = 1f;
        public void Update()
        {
            var translation = Input.GetAxis("Vertical") * _speed;
            var rotation = Input.GetAxis("Horizontal") * _rotationSpeed;
            _manipulator.externalFakeInput = Input.GetKey(KeyCode.Space);
            translation *= Time.deltaTime;
            rotation *= Time.deltaTime;
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);
        }
    }
}