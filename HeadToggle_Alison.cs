using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadToggle_Alison : MonoBehaviour
{
    public GameObject head;
    KeyCode headButtonController;
    KeyCode headButtonKeyboard;
    private bool headToggle = true;

    //body toggle on off
    public GameObject Me;
    public KeyCode MakeMeInvisible;

    // Start is called before the first frame update
    void Start()
    {
        head = GameObject.Find("rp_alison_rigged_001_geo.head");
        headButtonController = KeyCode.JoystickButton3;
        headButtonKeyboard = KeyCode.BackQuote;
        headToggle = false;
        head.SetActive(headToggle);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(headButtonController))
        {
            HeadFunction();
        }

        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(headButtonKeyboard))
        {

            HeadFunction();
        }

        // topggle body skinned mesh renderer to visible or not 
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(MakeMeInvisible))
        {
            Me.GetComponent<SkinnedMeshRenderer>().enabled = !Me.GetComponent<SkinnedMeshRenderer>().enabled;
        }

    }


    public void HeadFunction()
    {
        headToggle = !headToggle;
        head.SetActive(headToggle);

    }
}
