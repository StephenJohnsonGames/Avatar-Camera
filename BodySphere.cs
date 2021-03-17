using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySphere : MonoBehaviour {

    [SerializeField] Transform rotateAroundTarget;
    [SerializeField] float rotationSpeedH = 1;
	[SerializeField] float rotationSpeedV = 1;
	Vector3 initialDistance;

    //dual stick movement using gameobject

    private void Start()
    {
        initialDistance = transform.position - rotateAroundTarget.position;


    }
    void Update () {

        float inputHorizontal = Input.GetAxisRaw("Mouse X");

		transform.RotateAround(rotateAroundTarget.position, Vector3.forward, 0.0f * rotationSpeedH * 100 * Time.deltaTime);

        transform.position = rotateAroundTarget.position + initialDistance;
    }
}
