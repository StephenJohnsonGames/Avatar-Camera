using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationSphere : MonoBehaviour {

    [SerializeField] Transform rotateAroundTarget;
    [SerializeField] float rotationSpeed = 1;
    Vector3 initialDistance;

    private void Start()
    {
        initialDistance = transform.position - rotateAroundTarget.position;
    }
    void Update () {
		float inputHorizontal = Input.GetAxisRaw("DashHorizontal");
        transform.RotateAround(rotateAroundTarget.position, Vector3.up, inputHorizontal * rotationSpeed * Time.deltaTime);

        transform.position = rotateAroundTarget.position + initialDistance;
    }
}
