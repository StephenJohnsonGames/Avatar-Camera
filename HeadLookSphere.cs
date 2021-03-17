using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLookSphere : MonoBehaviour {

    [SerializeField] Transform rotateAroundTarget;
	//[SerializeField] Transform rotateAroundTargetHead;
	public GameObject HeadLookS;
    public GameObject BodyLookS;

    public GameObject Eric;

    public GameObject Alison;
 

    [SerializeField] float rotationSpeedH = 0.5f;
	[SerializeField] float rotationSpeedV = 0.5f;
	[SerializeField] float maxAngleDif = 65;
    Vector3 initialDistance;

    private void Start()
    {
        initialDistance = transform.position - rotateAroundTarget.position;
        //		initialDistance = new Vector3(5,0,0);
        
        Eric = GameObject.Find("rp_eric_rigged_001_yup_t");

        Alison = GameObject.Find("rp_alison_rigged_001_yup_t");
    


    }
    void Update ()
    {

        if (FPSCamera.char1Active == true)
        {
            Eric = GameObject.Find("eric_blender");

            rotateAroundTarget = Eric.transform;

        }

        if (FPSCamera.char2Active == true)
        {
            Alison = GameObject.Find("alison_blender");



            rotateAroundTarget = Alison.transform;

        }

        float inputHorizontal = Input.GetAxisRaw ("DashHorizontal");
		float inputVertical = Input.GetAxisRaw ("DashVertical");
        //float inputHorizontalMouse = Input.GetAxisRaw("DashHorizontalMouse");
        //float inputVerticalMouse = Input.GetAxisRaw("DashVerticalMouse");

        transform.RotateAround (rotateAroundTarget.position, Vector3.up, inputHorizontal * rotationSpeedH * 100 * Time.deltaTime);
		transform.RotateAround (rotateAroundTarget.position, HeadLookS.transform.right, inputVertical * rotationSpeedV * 100 * Time.deltaTime);
        //transform.RotateAround(rotateAroundTarget.position, Vector3.up, inputHorizontalMouse * rotationSpeedH * 100 * Time.deltaTime);
        //transform.RotateAround(rotateAroundTarget.position, HeadLookS.transform.right, inputVerticalMouse * rotationSpeedV * 100 * Time.deltaTime);

        transform.position = rotateAroundTarget.position + initialDistance;

        //rotation clamping
        ClampSphere();


    }

    void ClampSphere()
    {
        Quaternion relative = transform.rotation * Quaternion.Inverse(rotateAroundTarget.rotation);
        Vector3 dr = relative.eulerAngles;
        if (dr.x > 180)
            dr.x = dr.x - 360;
        if (dr.y > 180)
            dr.y = dr.y - 360;
        if (dr.z > 180)
            dr.z = dr.z - 360;

        Vector3 bPos = BodyLookS.transform.parent.rotation.eulerAngles;
        Vector3 hPos = transform.rotation.eulerAngles;

        float yAngleDif = 0;
        bool under = false;
        if(hPos.x > 180)
        {
            hPos.x = 360 - hPos.x;
            under = true;
        }

        if (bPos.x > hPos.x)
            yAngleDif = bPos.x - hPos.x;
        else
            yAngleDif = hPos.x - bPos.x;

        if (yAngleDif > maxAngleDif)
        {
            if (under)
                transform.Rotate(-(maxAngleDif - yAngleDif), 0, 0);
            else
                transform.Rotate(maxAngleDif - yAngleDif, 0, 0);
        }

        if (dr.y > maxAngleDif)
        {
            transform.Rotate(0, maxAngleDif - dr.y, 0);
            Vector3 v = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(v.x, v.y, 0);
        }
        else if (dr.y < -maxAngleDif)
        {
            transform.Rotate(0, -maxAngleDif - dr.y, 0);
            Vector3 v = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(v.x, v.y, 0);
        }

    }

}
