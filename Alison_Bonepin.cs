using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alison_Bonepin : MonoBehaviour
{

    public GameObject bone;
    public GameObject alison;
    public Vector3 bonepin, headPos;
    public float height;
    
    //custom pin transform for first person camera
    void Update()
    {
        //bonepin = transform.position;
        headPos = transform.position;
        bonepin = bone.transform.position;

        headPos.y = bonepin.y += height;

        if (alison.CompareTag("Sitting"))
        {
            transform.position = new Vector3(bonepin.x, bonepin.y += height, bonepin.z);

        }
        if (alison.CompareTag("Sit")){

            //headPos.y = bonepin.y += height;

            transform.position = new Vector3(bonepin.x, bonepin.y += height, bonepin.z);

            //transform.position = bonepin;

            //transform.forward = bone.transform.forward;

        }

        if (alison.CompareTag("Chair"))
        {

            transform.position = new Vector3(bonepin.x, bonepin.y += height, bonepin.z);

        }

        if (alison.CompareTag("Stand"))
        {

            //headPos.y = bonepin.y += height;
            transform.position = new Vector3(bonepin.x, bonepin.y += height, bonepin.z);

        }
    }
}
