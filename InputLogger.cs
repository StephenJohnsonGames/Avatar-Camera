using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputLogger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
            PrintInput("0");
        else if (Input.GetKeyDown(KeyCode.JoystickButton1))
            PrintInput("1");
        else if (Input.GetKeyDown(KeyCode.JoystickButton2))
            PrintInput("2");
        else if (Input.GetKeyDown(KeyCode.JoystickButton3))
            PrintInput("3");
        else if (Input.GetKeyDown(KeyCode.JoystickButton4))
            PrintInput("4");
        else if (Input.GetKeyDown(KeyCode.JoystickButton5))
            PrintInput("5");
        else if (Input.GetKeyDown(KeyCode.JoystickButton6))
            PrintInput("6");
        else if (Input.GetKeyDown(KeyCode.JoystickButton7))
            PrintInput("7");
        else if (Input.GetKeyDown(KeyCode.JoystickButton8))
            PrintInput("8");
        else if (Input.GetKeyDown(KeyCode.JoystickButton9))
            PrintInput("9");
        else if (Input.GetKeyDown(KeyCode.JoystickButton10))
            PrintInput("10");
        else if (Input.GetKeyDown(KeyCode.JoystickButton11))
            PrintInput("11");
        else if (Input.GetKeyDown(KeyCode.JoystickButton12))
            PrintInput("12");
        else if (Input.GetKeyDown(KeyCode.JoystickButton13))
            PrintInput("13");
        else if (Input.GetKeyDown(KeyCode.JoystickButton14))
            PrintInput("14");
        else if (Input.GetKeyDown(KeyCode.JoystickButton15))
            PrintInput("15");
        else if (Input.GetKeyDown(KeyCode.JoystickButton16))
            PrintInput("16");
        else if (Input.GetKeyDown(KeyCode.JoystickButton17))
            PrintInput("17");
        else if (Input.GetKeyDown(KeyCode.JoystickButton18))
            PrintInput("18");
        else if (Input.GetKeyDown(KeyCode.JoystickButton19))
            PrintInput("19");
    }

    void PrintInput(string inputKey)
    {
        Debug.Log("Joystick Input: " + inputKey);
    }
}
