using System;
using System.Collections.Generic;
using Renderers;
using UnityEngine;

public enum SelectionMode
{
    Point,
    Line,
    Face,
    Object
}


public class Manipulator : MonoBehaviour
{
    [SerializeField] private Geometries _geom;
    //Ajout des references pour les colisions
    private PointRenderer _point;
    private SegmentRenderer _line;
    private PolygonRenderer _face;
    private PolyedreRenderer _object;

    private UnityEngine.XR.InputDevice device;
    //Ajout d'une variable SelectionMode
    [SerializeField] private SelectionMode _selectionMode = SelectionMode.Object;
    //Ajout Liste des points à selectionner pour bouger la ligne et une face
    private List<PointRenderer> _selectedPointsSegment = new List<PointRenderer>();
    private List<PolygonRenderer> _selectedPointsFace = new List<PolygonRenderer>();


    private void Start()
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, devices);

        if (devices.Count == 1)
        {
            device = devices[0];
            Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
        }
        else if (devices.Count > 1)
        {
            Debug.Log("Found more than one left hand!");
        }
    }
    private bool _release;

    /*
    private void Update()
    {

        Debug.Log("Point : " + (_point != null));
        if (Input.GetKey(KeyCode.Space) || VRInput())
        {
            var pos = transform.position;
            if (_point != null)
            {
                _point.Data.Set(pos.x, pos.y, pos.z);
            }
            else if(_release)
            {
                _geom.AddPoint(new Point(pos.x, pos.y, pos.z));
                //TODO
                _geom.AddIndex(_geom.PolygonesCount, _geom.PointsCount - 1);
                _release = false;
            }
        }
        else
        {
            _release = true;
        }
    }
    */

    private void Update()
    {
        Debug.Log("Point : " + (_point != null));
        if (Input.GetKey(KeyCode.Space) || VRInput())
        {
            var pos = transform.position;
            switch (_selectionMode)
            {
                case SelectionMode.Point:
                    if (_point != null)
                    {
                        _point.Data.Set(pos.x, pos.y, pos.z);
                    }
                    break;
                case SelectionMode.Line:
                    // Logique de manipulation d'une ligne
                    //On selectionne d'abord la ligne et ses points
                    if (_line !=null)
                    {
                        _selectedPointsSegment = GetLinePoints();
                        //On deplace la ligne
                        MoveLine(pos);
                    }
                    break;

                case SelectionMode.Face:
                    // Logique de manipulation d'une face
                    // Selection des points de la face à manipuler
                    if (_face!=null)
                    {
                        //_selectedPoints = GetFacePoints();
                        // On deplace la face
                        //MoveSelectedPoints(pos);
                    }
                    break;

                case SelectionMode.Object:
                    if (_object!=null)
                    {
                        MoveObject(pos);
                    }
                    break;
            }
        }
        else
        {
            _release = true;
        }
    }

    private void MoveObject(Vector3 newPos)
    {
        foreach (var point in _geom.Points)
        {
            point.Set(newPos.x, newPos.y, newPos.z);
        }
    }

//DEBUT DU CODE QUE J AI AJOUTE

    // Fonction permettant le deplacement d'une ligne selectionnée
    private void MoveLine(Vector3 newPos)
    {
        foreach (var point in _selectedPointsSegment)
        {
            point.Data.Set(newPos.x, newPos.y, newPos.z);
        }
    }

    // Ajout fonction pour récupérer les points d'une ligne
     private List<PointRenderer> GetLinePoints()
    {
        List<PointRenderer> linePoints = new List<PointRenderer>();

        linePoints.Add(_line._start);
        linePoints.Add(_line._end);
      
        return linePoints;
    }
    

    //FIN DU CODE QUE J AI AJOUTE


    private bool VRInput()
    {
        bool triggerValue;
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            return true;
        }

        return false;
    }

    // Ajout des differentes references pour les colisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _point))
        {
            Debug.Log("Point found");
        }
        else if (other.TryGetComponent(out _line))
        {
            Debug.Log("Line found");
        }
        else if (other.TryGetComponent(out _face))
        {
            Debug.Log("Face found");
        }
        else if (other.TryGetComponent(out _object))
        {
            Debug.Log("object found");
        }
        else
        {
            _point = null;
            _face = null;
            _line = null;
            _object = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _point = null;
        _face = null;
        _line = null;
        _object = null;
    }

}