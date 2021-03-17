using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationIndicatorBody : MonoBehaviour {
    [SerializeField] Transform target;

    GameObject BodySphere;

    void Awake()
    {
        BodySphere = GameObject.Find("BodySphere");
        target = BodySphere.transform;

    }

    void Update ()
    {
        if (target != null)
        {
            transform.LookAt(target, Vector3.up);
            transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(transform.localRotation.eulerAngles.x, 0, transform.localRotation.eulerAngles.z));
        }
	}
}
