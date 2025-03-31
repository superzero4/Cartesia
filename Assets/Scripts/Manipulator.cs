using System;
using System.Collections.Generic;
using Control;
using NUnit.Framework;
using Renderers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;


public class Manipulator : MonoBehaviour
{
    [SerializeField] private SelectionModeControl _selection;
    [SerializeField] private Main _main;
    [SerializeField] private Manipulator _other;
    [SerializeField] private XRNode _hand;

    private IGeometries _geom;
    private PointRenderer _point;
    private SegmentRenderer _line;
    private PolygonRenderer _face;
    private PolyedreRenderer _object;

    private UnityEngine.XR.InputDevice device;

    //Ajout d'une variable SelectionMode
    [SerializeField] private SelectionMode _selectionMode => _selection.SelectionMode;

    //Ajout Liste des points à selectionner pour bouger la ligne et une face
    private List<PointRenderer> _selectedPointsSegment = new List<PointRenderer>();
    private List<PolygonRenderer> _selectedPointsFace = new List<PolygonRenderer>();


    private void Start()
    {
        Assert.IsTrue(_hand != _other._hand && _hand == XRNode.LeftHand || _hand == XRNode.RightHand);
        device = default;
        _geom = _main.RuntimeGeometry;
        var devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(_hand, devices);

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
    [NonSerialized] public bool externalFakeInput;
    [NonSerialized] public bool externalFakeInput2;

    private void Update()
    {
        Debug.Log("Point : " + (_point != null));
        var pos = transform.position;
        if (externalFakeInput || VRInput(UnityEngine.XR.CommonUsages.triggerButton))
            OnTrigger(pos);

        var pos2 = _other.transform.position;
        if (externalFakeInput2 || VRInput(UnityEngine.XR.CommonUsages.gripButton))
            OnGrip(pos, pos2);
        else
            _release = true;
    }

    private void OnGrip(Vector3 pos, Vector3 pos2)
    {
        if (!_release) return;
        var Point = new Point(pos.x, pos.y, pos.z);
        switch (_selectionMode)
        {
            case SelectionMode.Point:
                _geom.AddPoint(Point);
                break;
            case SelectionMode.Line:
                _geom.AddPoint(Point);
                _geom.AddPoint(new Point(pos2.x, pos2.y, pos2.z));
                _geom.AddSegment(_geom.PointsCount - 2, _geom.PointsCount - 1);
                //Point already got added, no additional treatement for these case, ww just add the point and cannot add it to a sublist or anything
                break;
            case SelectionMode.Face:
                //Add the point to the face
                _geom.AddPoint(Point);
                if (_face != null)
                {
                    _geom.AddPointToPolygon(_face.Index, _geom.PointsCount - 1);
                }
                else
                {
                    //TODO create new face from 3 newly created points
                }

                break;
            case SelectionMode.Object:
                Debug.LogWarning("Trying to create something in object mode, this is not implemented");
                //TODO create new triangular based pyramid or something or consider creating a new face if an object is selected

                break;
        }
    }

    private void OnTrigger(Vector3 pos)
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
                    //else if (_release)
                    //{
                    //    _geom.AddPoint(new Point(pos.x, pos.y, pos.z));
                    //    //TODO
                    //    _geom.AddIndex(_geom.PolygonesCount-1, _geom.PointsCount - 1);
                    //    _release = false;
                    //}
            break;
                case SelectionMode.Line:
                    // Logique de manipulation d'une ligne
                    //On selectionne d'abord la ligne et ses points
                    if (_line != null)
                    {
                        _selectedPointsSegment = GetLinePoints();
                        //On deplace la ligne
                        MoveLine(pos);
                    }
                    break;
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
                if (_line != null)
                {
                    _selectedPointsSegment = GetLinePoints();
                    //On deplace la ligne
                    MoveLine(pos);
                }

                break;

            case SelectionMode.Face:
                // Logique de manipulation d'une face
                // Selection des points de la face à manipuler
                if (_face != null)
                {
                    //_selectedPoints = GetFacePoints();
                    // On deplace la face
                    //MoveSelectedPoints(pos);
                }

                break;
                case SelectionMode.Face:
                    // Logique de manipulation d'une face
                    // Selection des points de la face à manipuler
                    if (_face != null)
                    {
                        //_selectedPoints = GetFacePoints();
                        // On deplace la face
                        //MoveSelectedPoints(pos);
                    }
                    break;

            case SelectionMode.Object:
                if (_object != null)
                {
                    MoveObject(pos);
                }

                break;
                case SelectionMode.Object:
                    if (_object != null)
                    {
                        MoveObject(pos);
                    }
                    break;
            }
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


    private bool VRInput(InputFeatureUsage<bool> button)
    {
        bool triggerValue = false;
        if (device.isValid && device.TryGetFeatureValue(button, out triggerValue) && triggerValue)
        {
            return true;
        }

        return false;
    }

    // Ajout des differentes references pour les colisions
    private void OnTriggerEnter(Collider other)
    {
        if (_point == null && other.TryGetComponent(out _point))
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
            //_point = null;
            _face = null;
            _line = null;
            _object = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_point != null && other.gameObject == _point.gameObject)
            _point = null;
        _face = null;
        _line = null;
        _object = null;
    }
}