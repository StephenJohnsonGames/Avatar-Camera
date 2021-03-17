using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CameraAnchorAlison : MonoBehaviour
{
    [SerializeField]
    Text cameraPosLabel;
    Camera nosecamera;
    private GameObject UI;
    KeyCode uiButton;
    private bool uiToggle = true;

    void Start()
    {
        nosecamera = GameObject.Find("HeadCameraNose").GetComponent<Camera>();
        UI = GameObject.Find("HeadBodyUI2");
        uiButton = KeyCode.JoystickButton5;

    }

    void Update()
    {

        if (Input.GetKeyDown(uiButton))
        {

            UISwitch();

        }


    }

    public void UISwitch()
    {
        uiToggle = !uiToggle; // if toggle is false then it would be true, if toggle is true then it would be false like on and off
        UI.SetActive(uiToggle); // pass the toggle value
        
    }

    // Update is called once per frame
    void LateUpdate () {
        cameraPosLabel.text = transform.position.ToString();
	}
}
