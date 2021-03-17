using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSwap : MonoBehaviour

{
    public static bool char1Active = true;
    public static bool char2Active = false;
    public static bool char3Active = false;


    public GameObject char1;
    public GameObject char2;
    public GameObject char3;

    KeyCode swap1;
    KeyCode swap2;
    KeyCode swap3;

    float swap1controller;
    float swap2controller;
    float swap3controller;


    // Start is called before the first frame update
    void Start()
    {
        char1 = GameObject.Find("rp_andrew_rigged_003");
        char2 = GameObject.Find("rp_eric_rigged_001_yup_t");
        char3 = GameObject.Find("rp_alison_rigged_001_yup_t");

        char2.SetActive(false);
        char3.SetActive(false);

        swap1 = KeyCode.Alpha1;
        swap2 = KeyCode.Alpha2;
        swap3 = KeyCode.Alpha3;

        swap1controller = Input.GetAxisRaw("Character1");
        swap2controller = Input.GetAxisRaw("Character2");
        swap3controller = Input.GetAxisRaw("Character3");


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(swap1))
        {
            char2.SetActive(false);
            char3.SetActive(false);

            char1.SetActive(true);

            char1Active = true;
            char2Active = false;
            char3Active = false;

}

        if (Input.GetKeyDown(swap2))
        {
            char1.SetActive(false);
            char3.SetActive(false);

            char2.SetActive(true);

            char1Active = false;
            char2Active = true;
            char3Active = false;
        }

        if (Input.GetKeyDown(swap3))
        {
            char2.SetActive(false);
            char1.SetActive(false);

            char3.SetActive(true);

            char1Active = false;
            char2Active = false;
            char3Active = true;
        }

        if (Input.GetAxisRaw("Character1") < -0.9f)
        {
            char2.SetActive(false);
            char3.SetActive(false);

            char1.SetActive(true);

            char1Active = true;
            char2Active = false;
            char3Active = false;

        }

        if (Input.GetAxisRaw("Character2") > 0.9f)
        {
            char1.SetActive(false);
            char3.SetActive(false);

            char2.SetActive(true);

            char1Active = false;
            char2Active = true;
            char3Active = false;
        }

        if (Input.GetAxisRaw("Character3") > 0.9f)
        {
            char2.SetActive(false);
            char1.SetActive(false);

            char3.SetActive(true);

            char1Active = false;
            char2Active = false;
            char3Active = true;
        }

    }
}
