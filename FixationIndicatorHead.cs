using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationIndicatorHead : MonoBehaviour
{
    [SerializeField] Transform target;

    GameObject HeadSphere;

    void Awake()
    {
        HeadSphere = GameObject.Find("HeadLookSphere");
        target = HeadSphere.transform;

    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target, Vector3.up);
            transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.localRotation.eulerAngles.z));
        }
    }
}

