using System;
using System.Collections.Generic;
using Renderers;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    [SerializeField] private Main _main;
    private IGeometries _geom;
    private PointRenderer _point;
    private UnityEngine.XR.InputDevice device;

    private void Start()
    {
        _geom = _main.RuntimeGeometry;
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

    private bool VRInput()
    {
        bool triggerValue;
        if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out triggerValue) && triggerValue)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out _point))
        {
            Debug.Log("Point found");
        }
        else
        {
            _point = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _point = null;
    }
}