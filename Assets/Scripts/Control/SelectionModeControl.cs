using System;
using UnityEngine;
using UnityEngine.Events;

namespace Control
{
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Line = 2,
        Face = 3,
        Object = 4
    }

    public class SelectionModeControl : MonoBehaviour
    {
        [SerializeField]
        private SelectionMode _selectionMode = SelectionMode.Line;
        public UnityEvent<SelectionMode> OnChangeMode;
        public SelectionMode SelectionMode => _selectionMode;

        private void Start()
        {
            OnChangeMode.Invoke(_selectionMode);
        }

        private void OnValidate()
        {
            OnChangeMode.Invoke(_selectionMode);
        }

        private void Update()
        {
            SelectionMode newMode = SelectionMode.None;
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Z))
            {
                newMode = SelectionMode.Point;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.X))
            {
                newMode = SelectionMode.Line;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.C))
            {
                newMode = SelectionMode.Face;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.V))
            {
                newMode = SelectionMode.Object;
            }

            if (newMode != SelectionMode.None && newMode != _selectionMode)
            {
                _selectionMode = newMode;
                OnChangeMode.Invoke(_selectionMode);
            }
        }
    }
}