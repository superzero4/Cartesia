using System;
using System.Collections.Generic;
using System.Linq;
using Control;
using NUnit.Framework;
using Renderers;
using Sound;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;


public class Manipulator : MonoBehaviour
{
    [SerializeField] private SelectionModeControl _selection;
    [SerializeField] private Main _main;
    [SerializeField] private Manipulator _other;
    [SerializeField] private SoundByMovement _sound;
    [SerializeField] private XRNode _hand;

    private IGeometries _geom;
    private PointRenderer _point;
    private SegmentRenderer _line;
    private PolygonRenderer _face;
    private PolyedreRenderer _object;
    private Vector3 _lastPos;

    private UnityEngine.XR.InputDevice device;

    //Ajout d'une variable SelectionMode
    [SerializeField] private SelectionMode _selectionMode => _selection.SelectionMode;

    //Ajout Liste des points à selectionner pour bouger la ligne et une face


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
        //Debug.Log("Poyledre : " + (_object != null));
        //Debug.Log("Polyg : " + (_face != null));
        var pos = transform.position;
        var delta = pos - _lastPos;
        if (externalFakeInput || VRInput(UnityEngine.XR.CommonUsages.triggerButton))
            OnTrigger(delta);

        var pos2 = _other.transform.position;
        if (externalFakeInput2 || VRInput(UnityEngine.XR.CommonUsages.gripButton))
            OnGrip(pos, pos2);
        else
            _release = true;
        _lastPos = pos;
    }

    private void OnGrip(Vector3 pos, Vector3 pos2)
    {
        if (!_release) return;
        _release = false;
        Point Point;
        switch (_selectionMode)
        {
            case SelectionMode.Point:

                Point = new Point(pos.x, pos.y, pos.z);
                _geom.AddPoint(Point);
                break;
            case SelectionMode.Line:

                Point = new Point(pos.x, pos.y, pos.z);
                _geom.AddPoint(Point);
                _geom.AddPoint(new Point(pos2.x, pos2.y, pos2.z));
                _geom.AddSegment(_geom.PointsCount - 2, _geom.PointsCount - 1);
                //Point already got added, no additional treatement for these case, ww just add the point and cannot add it to a sublist or anything
                break;
            case SelectionMode.Face:
                //Add empty polygon
                _geom.AddPolygon();


                break;
            case SelectionMode.Object:
                //Debug.LogWarning("Trying to create something in object mode, this is not implemented");
                //TODO create new triangular based pyramid or something or consider creating a new face if an object is selected
                _geom.AddPolyedre();
                break;
        }
    }

    private void OnTrigger(Vector3 delta)
    {
        _sound.Offset(delta);
        switch (_selectionMode)
        {
            case SelectionMode.Point:
                if (_point != null)
                {
                    _point.Data.Offset(delta.x, delta.y, delta.z);
                }
                break;
            case SelectionMode.Line:
                // Logique de manipulation d'une ligne
                //On selectionne d'abord la ligne et ses points
                if (_line != null)
                {
                    //On deplace la ligne
                    MovePoints(delta, GetLinePoints());
                }

                break;
            case SelectionMode.Face:
                // Logique de manipulation d'une face
                // Selection des points de la face à manipuler
                if (_face != null)
                {
                    // On deplace la face
                    MovePoints(delta, _face.Data.sommets);
                }

                break;

            case SelectionMode.Object:
                if (_object != null)
                {
                    MovePoints(delta, _object.Data.Faces.SelectMany(f => f.sommets));
                }

                break;
        }

    }

    //DEBUT DU CODE QUE J AI AJOUTE

    // Fonction permettant le deplacement d'une ligne selectionnée
    private void MovePoints(Vector3 newPos, IEnumerable<Point> points)
    {
        foreach (var point in points)
        {
            point.Offset(newPos.x, newPos.y, newPos.z);
        }
    }

    // Ajout fonction pour récupérer les points d'une ligne
    private List<Point> GetLinePoints()
    {
        List<Point> linePoints = new List<Point>();

        linePoints.Add(_line._start.Data);
        linePoints.Add(_line._end.Data);

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
        Enter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Exit(other);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Enter(collision.collider);
    }
    private void OnCollisionExit(Collision collision)
    {
        Exit(collision.collider);
    }
    private void Enter(Collider other)
    {
        if (_point == null)
        {
            PointRenderer point = other.GetComponent<PointRenderer>();
            if (point != null)
            {
                _point = point;
                _sound.Play();
                Debug.Log("Point found");
            }
        }
        if (_line == null)
        {
            var line = other.transform.parent.GetComponent<SegmentRenderer>();
            if (line != null)
            {
                _line = line;
                _sound.Play();
                Debug.Log("Line found");
            }
        }
        if (_face == null)
        {
            var face = other.transform.parent.GetComponent<PolygonRenderer>();
            _sound.Play();
            if (face != null)
            {
                _face = face;
                Debug.Log("Face found");
            }
        }
        if (_object == null)
        {
            var obj = other.transform.parent.GetComponent<PolyedreRenderer>();
            if (obj != null)
            {
                _object = obj;
                Debug.Log("object found");
            }
        }
    }
    private void Exit(Collider other)
    {
        if (_point != null && other.gameObject == _point.gameObject)
            _point = null;
        if (_face != null && other.transform.parent.gameObject == _face.gameObject)
            _face = null;
        if (_line != null && other.transform.parent.gameObject == _line.gameObject)
            _line = null;
        if (_object != null && other.transform.parent.gameObject == _object.gameObject)
            _object = null;
    }
}