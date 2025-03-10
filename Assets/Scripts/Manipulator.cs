using System;
using System.Collections.Generic;
using Renderers;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    private PointRenderer _point;
    private UnityEngine.XR.InputDevice device;

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

    private void Update()
    {
        var posi = transform.position;
        posi.x = Mathf.Sin(Time.time) * 2;
        transform.position = posi;
        Debug.Log("Point : " + (_point != null));
        if (Input.GetKey(KeyCode.Space) || VRInput())
        {
            if (_point != null)
            {
                var pos = transform.position;
                _point.Data.Set(pos.x, pos.y, pos.z);
            }
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