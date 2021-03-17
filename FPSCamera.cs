using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using HighlightingSystem;
using UnityEngine.UI;

public class FPSCamera : MonoBehaviour
{
    /* These variables are what tell the camera how its going to function by
	 * setting the viewing target, and other properties such as viewing 
	 * angles and bobbing amount */
    public Transform viewTarget, targetRotation, aimPoint;
    public LayerMask collisionLayers;
    public float distance = 2.5f;
    public float TPSheight = 1.75f;
    public float TPSoffset = 0.65f;
    public float collisionOffset = 0.3f;
    public float horizontalRotationSpeed = 250.0f;
    public float verticalRotationSpeed = 150.0f;
    public bool viewBobbing = false;
    private float bobbingAmount = 0.1f;
    private float bobbingSpeed = 0.4f;
    public float height = 0.2f;
    public float horizontalSensitivity = 250.0f;
    public float verticalSensitivity = 150.0f;
    public float rotationDampening = 0.75f;
    public float minVerticalAngle = -60.0f;
    public float maxVerticalAngle = 60.0f;
    public float minHorizontalAngle = -60.0f;
    public float maxHorizontalAngle = 60.0f;
    public float OffsetForward;
    public float OffsetUp;
    private bool navMovement;
    private bool navMeshAutoPlay;
    public bool TPSbool;
    Rigidbody char1rb;
    Rigidbody char2rb;
    public Transform navTarget;
    public Transform[] paths;
    private int destPoint = 0;
    public bool DisableMouse = false;
    public bool DisableAllRotations = false;
    public bool rotatePlayerWithCamera = true;
    public bool invertPlayerAim = false;

    //variables to be used in conjunction with Fixation Points
    public GameObject NR_StandardLookAt;
    public GameObject NR_OffsetLookAt;
    public GameObject NR_FixationPoint;
    public GameObject FPSCameraObject;
    public GameObject LoungeLight;
    public GameObject DiningLight;
    public GameObject RotateT;

    /* These variables are meant to store values given by the script and
	 * not the user */
    public float h, v, offset, period;
    public float sith, sitv, angleh;
    private Vector3 newPosition, lastTargetPos, chairPosition, sitPos, standPos;
    private Quaternion newRotation, newRotationSit, smoothRotation, sitRotation;
    private Transform cameraTransform;

    //variables for changing characters

    public static bool char1Active = true;
    public static bool char2Active = false;
    //public static bool char3Active = false;

    public GameObject char1, char2, char1bone, char2bone;
    // public GameObject char3;

    // Used for left/right binocular cameras
    public Vector3 InterOccularOffset;

    bool PeripheralToggle = true;
    bool HighlightToggle = true;
    bool DofToggle = true;
    public GameObject EffectsSliderUI;
    bool HideUI = false;
    bool HideLight;
    bool HideDining;


    public Toggle ToggleY;
    public Toggle ToggleU;
    public Toggle ToggleI;

    KeyCode swap1;
    KeyCode swap2;
    //KeyCode swap3;

    public Vector3 tmpRot;

    /* This is where we initialize our script */
    void Start()
    {
        Initialize();
        HideLight = LoungeLight.activeSelf;
        HideDining = DiningLight.activeSelf;

        //Find our avatars and set them to characters, take their bone pinning transforms, switch 1 char on, get their animator + rigidbodies for navmesh
        char1 = GameObject.Find("eric_blender");
        char2 = GameObject.Find("alison_blender");

        char1bone = GameObject.Find("Bone_Position_Er");
        char2bone = GameObject.Find("Bone_Position_Al");

        char2.SetActive(false);

        char1.GetComponent<RPGCharacterController>();

        char1.GetComponent<Animator>();

        char1rb = char1.GetComponent<Rigidbody>();
        char2rb = char2.GetComponent<Rigidbody>();

        swap1 = KeyCode.Alpha1;
        swap2 = KeyCode.Alpha2;

        PeripheralToggle = false;
        GameObject.Find("FPSCamera").GetComponent<NR_PeripheralBlur>().enabled = PeripheralToggle;
        ToggleY.isOn = PeripheralToggle;

        HighlightToggle = false;
        GameObject.Find("FPSCamera").GetComponent<HighlightingRenderer>().enabled = HighlightToggle;
        ToggleU.isOn = HighlightToggle;


        DofToggle = false;
        GameObject.Find("FPSCamera").GetComponent<DofRadialOptimised>().enabled = DofToggle;
        ToggleI.isOn = DofToggle;

        HideUI = false;
        
        EffectsSliderUI.SetActive(!HideUI);
   
        GameObject.Find("FPSCamera").GetComponent<FPSDisplay>().enabled = !HideUI;
        GameObject.Find("Lounge_Light");
        GameObject.Find("Dining_Room_Light");

    }

    /* This is where we set our private variables, check for null errors,
	 * and anything else that needs to be called once during startup */
    void Initialize()
    {
        h = this.transform.eulerAngles.x;
        v = this.transform.eulerAngles.y;

        sith = this.transform.eulerAngles.x;
        sitv = this.transform.eulerAngles.y;

        cameraTransform = this.transform;

        NullErrorCheck();

    }

    /* We check for null errors or warnings and notify the user to fix them */
    void NullErrorCheck()
    {
        if (!viewTarget)
        {
            Debug.LogError("Please make sure to assign a view target!");
            Debug.Break();
        }
    }

    void Update()
    {
        if (NR_StandardLookAt.GetComponent<NR_StandardLookAt>().FreezeMe == true)
        {

            if (TPSbool == true)
            {
                NR_StandardLookAt.transform.position = viewTarget.transform.position + viewTarget.forward.normalized * OffsetForward;

                tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                targetRotation.rotation = Quaternion.Euler(tmpRot);

                newPosition = viewTarget.position;
                newPosition.y += height + offset;


                lastTargetPos = viewTarget.position;
            }

            else
            {
                NR_StandardLookAt.transform.position = viewTarget.transform.position + viewTarget.forward.normalized * OffsetForward;

                tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                targetRotation.rotation = Quaternion.Euler(tmpRot);

                newPosition = viewTarget.position;
                newPosition.y += height + offset;


                lastTargetPos = viewTarget.position;

   
                
            }

        }

        //Debug.Log("h = " + h.ToString("F1") + " v = " + v.ToString("F1") + " sith = " + sith.ToString("F1") + " sitv = " + sitv.ToString("F1"));

        //Inputs for switching characters

        #region KeyInputs

        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(swap1))
        {

            char2.SetActive(false);
            char1.SetActive(true);

            char1Active = true;
            char2Active = false;

            viewTarget = char1bone.transform;
            targetRotation = char1.transform;
        }

        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(swap2))
        {
            char1.SetActive(false);
            char2.SetActive(true);

            char1Active = false;
            char2Active = true;

            viewTarget = char2bone.transform;
            targetRotation = char2.transform;



            //cameraTransform.rotation = 

        }

        //Inputs using a controller

        if (Input.GetAxisRaw("Character1") < -0.95f)
        {
            char2.SetActive(false);
            char1.SetActive(true);

            char1Active = true;
            char2Active = false;

            viewTarget = char1bone.transform;
            targetRotation = char1.transform;

        }

        if (Input.GetAxisRaw("Character2") > 0.95f)
        {
            char1.SetActive(false);
            char2.SetActive(true);

            char1Active = false;
            char2Active = true;
            viewTarget = char2bone.transform;
            targetRotation = char2.transform;

        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            PeripheralToggle = !PeripheralToggle;
            GameObject.Find("FPSCamera").GetComponent<NR_PeripheralBlur>().enabled = PeripheralToggle;
            ToggleY.isOn = PeripheralToggle;
        }

        if (Input.GetKeyUp(KeyCode.U))
        {
            HighlightToggle = !HighlightToggle;
            GameObject.Find("FPSCamera").GetComponent<HighlightingRenderer>().enabled = HighlightToggle;
            ToggleU.isOn = HighlightToggle;
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            DofToggle = !DofToggle;
            GameObject.Find("FPSCamera").GetComponent<DofRadialOptimised>().enabled = DofToggle;
            ToggleI.isOn = DofToggle;

        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            HideUI = !HideUI;
            EffectsSliderUI.SetActive(!HideUI);
            GameObject.Find("FPSCamera").GetComponent<FPSDisplay>().enabled = !HideUI;
            GameObject.Find("FPSCamera").GetComponent<NR_VisualVariable>().enabled = !HideUI;
        }

        if (Input.GetKeyUp(KeyCode.LeftBracket))
        {
            HideLight = !HideLight;
            LoungeLight.SetActive(HideLight);

        }

        if (Input.GetKeyUp(KeyCode.RightBracket))
        {
            HideDining = !HideDining;
            DiningLight.SetActive(HideDining);

        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            TPSbool = !TPSbool;
        }
        #endregion
    }


    /* This is where we do all our camera updates. This is where the camera
	 * gets all of its functionality. From setting the position and rotation,
	 * to adjusting the camera to avoid geometry clipping */
    void LateUpdate()
    {
        if (!viewTarget)
            return;


        /* Calls the head bobbing function if the user specifies it */
        if (viewBobbing)
        {
            HeadBobbing();
        }
        else
        {
            offset = 0.0f;
        }

        /* We check to make sure the game isn't paused then lock the cursor */
        if (Time.timeScale > 0.0f)
        {
            //Screen.lockCursor = true;
            //Cursor.visible = false;
        }
        else
        {
            Screen.lockCursor = false;
            //Cursor.visible = true;
        }

        if (NR_StandardLookAt.GetComponent<NR_StandardLookAt>().FreezeMe == false)
        {
            if (char1Active && !char2Active)
            {
                OffsetForward = 0.1f;
                //Unity tagging system used to track progression of anims from avatars and have separate camera functions for each part of the anim

                if (char1.CompareTag("navMeshAuto") || char1.CompareTag("navMeshPath"))
                {
                    if(TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);
                    }

                    else if (TPSbool == false)
                    {
                        Vector3 direction = (targetRotation.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.1f;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngle(sith, -40.0f, 40.0f);
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        newRotation = Quaternion.Euler(sitv, sith, 0.0f);


                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (targetRotation.transform.forward * OffsetForward);

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        //sith = h;
                        //sitv = v;

                        //cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 5.0f);

                        cameraTransform.rotation = (lookRotation * smoothRotation).normalized;
                    }
             

                }

                if (char1.CompareTag("Sitting"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //simplified version of our EricCamera function found below, doesn't take mouse input to allow natural translation using our rb anim
                        //Vector3 direction = (viewTarget.forward * 10.0f);
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.15f;
                        height = 0.0f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        cameraTransform.position = Vector3.Lerp(cameraTransform.position, (newPosition + ((viewTarget.transform.forward * OffsetForward) + (viewTarget.transform.up * OffsetUp))), Time.deltaTime * 8.0f);

                        /*We set the position and rotation with a smooth linear interpolation for the camera
                         * rotation for a nicer viewing effect */
                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);
                        cameraTransform.rotation = smoothRotation;

                        Vector3 tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        viewTarget.rotation = Quaternion.Euler(tmpRot);

                        newRotation = Quaternion.Euler(v, h, 0.0f);


                        smoothRotation = Quaternion.Slerp(smoothRotation, lookRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        h = smoothRotation.eulerAngles.y;
                        v = smoothRotation.eulerAngles.x;

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);

                        sith = h;
                        sitv = v;

                        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 5.0f);
                    }
                }
                if (char1.CompareTag("Chair"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {

                        //most of the code relating to chair movement found in RPG_Character_Controller, camera follows avatar movement then clamps for headlook
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);

                        OffsetForward = 0.15f;
                        height = -0.1f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngle(sith, -45.0f, 45.0f);
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        newRotationSit = Quaternion.Euler(sitv, sith, 0.0f).normalized;
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotationSit, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);
                        cameraTransform.rotation = (lookRotation * smoothRotation).normalized;

                    }
                }

                if (char1.CompareTag("Sit"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //uses both our h and sith mouse inputs to avoid clashing between our clamp and mouse rotation
                        h = viewTarget.eulerAngles.y;

                        standPos = cameraTransform.position;


                        Vector3 direction = viewTarget.forward;
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;


                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngleSit(sith, (h - 45.0f), (h + 45.0f));
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        sitRotation = Quaternion.Euler(sitv, sith, 0.0f);
                        smoothRotation = Quaternion.Slerp(smoothRotation, sitRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.rotation = smoothRotation;
                    }
                }
                if (char1.CompareTag("Stand"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //simplified version of our EricCamera function found below, doesn't take mouse input to allow natural translation using our rb anim
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.1f;
                        height = 0.0f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, (newPosition + ((viewTarget.transform.forward * OffsetForward) + (viewTarget.transform.up * OffsetUp))), Time.deltaTime * 8.0f);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        smoothRotation = Quaternion.Slerp(smoothRotation, lookRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        h = smoothRotation.eulerAngles.y;

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);

                        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 8.0f);

                    }
                }

                else if (char1.CompareTag("NR_Proximity") || char1.CompareTag("Headbob"))
                {

                    EricCamera();
                }



            }

            if (char2Active && !char1Active)
            {
                OffsetForward = 0.1f;
                //Unity tagging system used to track progression of anims from avatars and have separate camera functions for each part of the anim

                //location of door from avatar
                //x 5.7
                //y 0
                //z -5.8

                if (char2.CompareTag("navMeshAuto") || char2.CompareTag("navMeshPath"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);
                    }

                    else if (TPSbool == false)
                    {
                        Vector3 direction = (targetRotation.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.1f;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngle(sith, -40.0f, 40.0f);
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        newRotation = Quaternion.Euler(sitv, sith, 0.0f);


                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (targetRotation.transform.forward * OffsetForward);

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        //sith = h;
                        //sitv = v;

                        //cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 5.0f);

                        cameraTransform.rotation = (lookRotation * smoothRotation).normalized;
                    }


                }

                if (char2.CompareTag("Sitting"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //simplified version of our EricCamera function found below, doesn't take mouse input to allow natural translation using our rb anim
                        //Vector3 direction = (viewTarget.forward * 10.0f);
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.15f;
                        height = 0.0f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        cameraTransform.position = Vector3.Lerp(cameraTransform.position, (newPosition + ((viewTarget.transform.forward * OffsetForward) + (viewTarget.transform.up * OffsetUp))), Time.deltaTime * 8.0f);

                        /*We set the position and rotation with a smooth linear interpolation for the camera
                         * rotation for a nicer viewing effect */
                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);
                        cameraTransform.rotation = smoothRotation;

                        Vector3 tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        viewTarget.rotation = Quaternion.Euler(tmpRot);

                        newRotation = Quaternion.Euler(v, h, 0.0f);


                        smoothRotation = Quaternion.Slerp(smoothRotation, lookRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        h = smoothRotation.eulerAngles.y;
                        v = smoothRotation.eulerAngles.x;

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);

                        sith = h;
                        sitv = v;

                        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 5.0f);
                    }
                }
                if (char2.CompareTag("Chair"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {

                        //most of the code relating to chair movement found in RPG_Character_Controller, camera follows avatar movement then clamps for headlook
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);

                        OffsetForward = 0.15f;
                        height = -0.1f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngle(sith, -45.0f, 45.0f);
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        newRotationSit = Quaternion.Euler(sitv, sith, 0.0f).normalized;
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotationSit, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        ///*We set the position and rotation with a smooth linear interpolation for the camera
                        // * rotation for a nicer viewing effect */

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);
                        cameraTransform.rotation = (lookRotation * smoothRotation).normalized;

                    }
                }

                if (char2.CompareTag("Sit"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //uses both our h and sith mouse inputs to avoid clashing between our clamp and mouse rotation
                        h = viewTarget.eulerAngles.y;

                        standPos = cameraTransform.position;


                        Vector3 direction = viewTarget.forward;
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;


                        sith += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                        sitv -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                        sith = ClampAngleSit(sith, (h - 45.0f), (h + 45.0f));
                        sitv = ClampAngle(sitv, minVerticalAngle, maxVerticalAngle);

                        sitRotation = Quaternion.Euler(sitv, sith, 0.0f);
                        smoothRotation = Quaternion.Slerp(smoothRotation, sitRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.rotation = smoothRotation;
                    }
                }
                if (char2.CompareTag("Stand"))
                {
                    if (TPSbool == true)
                    {
                        if (!viewTarget)
                            return;

                        /* We check that the game isn't paused and lock the cursor */
                        if (Time.timeScale > 0.0f)
                            ;// Screen.lockCursor = true;
                        else
                            Screen.lockCursor = false;

                        h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
                        v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

                        h = ClampAngle(h, -360.0f, 360.0f);
                        v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        /* We smooth the camera rotation using a growth function for a nicer viewing effect */
                        smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        newPosition = viewTarget.position;
                        newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

                        /* This calls our function to avoid camera clipping */
                        CheckSphere();

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        cameraTransform.position = newPosition;
                        cameraTransform.rotation = smoothRotation;

                        Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

                        //if (rotatePlayerWithCamera)
                        //{
                        //    /* We set our player's rotation to the cameras y rotation */
                        //    targetRotation.rotation = Quaternion.Euler(newTargetRot);
                        //}

                        /* We set the players aim rotation to the cameras x rotation */
                        newTargetRot = aimPoint.eulerAngles;
                        if (invertPlayerAim)
                            newTargetRot.x = (cameraTransform.eulerAngles.x);
                        else
                            newTargetRot.x = -(cameraTransform.eulerAngles.x);
                        aimPoint.rotation = Quaternion.Euler(newTargetRot);

                    }

                    else if (TPSbool == false)
                    {
                        //simplified version of our EricCamera function found below, doesn't take mouse input to allow natural translation using our rb anim
                        Vector3 direction = (viewTarget.forward * 10.0f);
                        Quaternion lookRotation = Quaternion.LookRotation(direction);

                        OffsetForward = 0.1f;
                        height = 0.0f;

                        NR_StandardLookAt.transform.position = viewTarget.position;

                        newPosition = viewTarget.position;
                        newPosition.y += height + offset;

                        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, (newPosition + ((viewTarget.transform.forward * OffsetForward) + (viewTarget.transform.up * OffsetUp))), Time.deltaTime * 8.0f);

                        newRotation = Quaternion.Euler(v, h, 0.0f);

                        smoothRotation = Quaternion.Slerp(smoothRotation, lookRotation, TimeSignature((1 / rotationDampening) * 100.0f));

                        smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

                        h = smoothRotation.eulerAngles.y;

                        cameraTransform.position = newPosition + (viewTarget.transform.forward * OffsetForward);

                        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, smoothRotation, Time.deltaTime * 8.0f);

                    }
                }

                else if (char2.CompareTag("NR_Proximity") || char2.CompareTag("Headbob"))
                {
                    AlisonCamera();
                }

            }

        

        }
    }

    void EricCamera()
    {
        
        OffsetForward = 0.1f;

        if (NR_StandardLookAt.GetComponent<NR_StandardLookAt>().FreezeMe == true)
        {
            //function content located in Update()
         

        }

        if(TPSbool == true)
        {
            if (!viewTarget)
                return;

            /* We check that the game isn't paused and lock the cursor */
            if (Time.timeScale > 0.0f)
                ;// Screen.lockCursor = true;
            else
                Screen.lockCursor = false;

            h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
            v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

            h = ClampAngle(h, -360.0f, 360.0f);
            v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

            newRotation = Quaternion.Euler(v, h, 0.0f);

            /* We smooth the camera rotation using a growth function for a nicer viewing effect */
            smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

            newPosition = viewTarget.position;
            newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

            /* This calls our function to avoid camera clipping */
            CheckSphere();

            smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

            cameraTransform.position = newPosition;
            cameraTransform.rotation = smoothRotation;

            Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

            if (rotatePlayerWithCamera)
            {
                /* We set our player's rotation to the cameras y rotation */
                targetRotation.rotation = Quaternion.Euler(newTargetRot);
            }

            /* We set the players aim rotation to the cameras x rotation */
            newTargetRot = aimPoint.eulerAngles;
            if (invertPlayerAim)
                newTargetRot.x = (cameraTransform.eulerAngles.x);
            else
                newTargetRot.x = -(cameraTransform.eulerAngles.x);
            aimPoint.rotation = Quaternion.Euler(newTargetRot);

        }

        else
        {

            transform.position = NR_StandardLookAt.transform.position + (viewTarget.forward * OffsetForward);

            viewBobbing = false;

            rotationDampening = 0.75f;
            sith = 0.0f;
            sitv = 0.0f;

            OffsetForward = 0.1f;

            if (Time.timeScale > 0.0f)
            {
                //Screen.lockCursor = true;
            }

            Vector3 direction = (viewTarget.forward * 10.0f);
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);


            //this is hard set from natural rendering manager 
            if (DisableAllRotations == false)
            {
                h += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                v -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                h = ClampAngle(h, -360.0f, 360.0f);
                v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);
            }

            newRotation = Quaternion.Euler(v, h, 0.0f);
            smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

            smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

            ///*We set the position and rotation with a smooth linear interpolation for the camera
            // * rotation for a nicer viewing effect */

            cameraTransform.rotation = smoothRotation;

            tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

            targetRotation.rotation = Quaternion.Euler(tmpRot);

            newPosition = viewTarget.position;
            newPosition.y += height + offset;

            NR_StandardLookAt.transform.position = newPosition + (viewTarget.forward * OffsetForward);

            lastTargetPos = viewTarget.position;

        }
    }

    void AlisonCamera()
    {
        OffsetForward = 0.1f;

        if (NR_StandardLookAt.GetComponent<NR_StandardLookAt>().FreezeMe == true)
        {
            //function content located in Update()

        }

        if (TPSbool == true)
        {

            //Vector3 rotateAv = NR_StandardLookAt.transform.eulerAngles;
            //rotateAv.y = 90.0f;
            //NR_StandardLookAt.transform.rotation = Quaternion.Euler(rotateAv);

            if (!viewTarget)
                return;

            /* We check that the game isn't paused and lock the cursor */
            if (Time.timeScale > 0.0f)
                ;// Screen.lockCursor = true;
            else
                Screen.lockCursor = false;

            h += Input.GetAxis("Mouse X") * (horizontalRotationSpeed * Time.deltaTime);
            v -= Input.GetAxis("Mouse Y") * (verticalRotationSpeed * Time.deltaTime);

            h = ClampAngle(h, -360.0f, 360.0f);
            v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);

            newRotation = Quaternion.Euler(v, h, 0.0f);

            /* We smooth the camera rotation using a growth function for a nicer viewing effect */
            smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

            newPosition = viewTarget.position;
            newPosition += smoothRotation * new Vector3(TPSoffset, TPSheight, -distance);

            /* This calls our function to avoid camera clipping */
            CheckSphere();

            smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

            cameraTransform.position = newPosition;
            cameraTransform.rotation = smoothRotation;

            Vector3 newTargetRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

            if (rotatePlayerWithCamera)
            {
                /* We set our player's rotation to the cameras y rotation */
                targetRotation.rotation = Quaternion.Euler(newTargetRot);
            }

            /* We set the players aim rotation to the cameras x rotation */
            newTargetRot = aimPoint.eulerAngles;
            if (invertPlayerAim)
                newTargetRot.x = (cameraTransform.eulerAngles.x);
            else
                newTargetRot.x = -(cameraTransform.eulerAngles.x);
            aimPoint.rotation = Quaternion.Euler(newTargetRot);

        }

        else
        {
            transform.position = NR_StandardLookAt.transform.position + (viewTarget.forward * OffsetForward);

            viewBobbing = false;

            rotationDampening = 0.75f;
            sith = 0.0f;
            sitv = 0.0f;

            OffsetForward = 0.1f;

            if (Time.timeScale > 0.0f)
            {
                //Screen.lockCursor = true;
            }

            Vector3 direction = (viewTarget.forward * 10.0f);
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);


            //this is hard set from natural rendering manager 
            if (DisableAllRotations == false)
            {
                h += Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
                v -= Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

                h = ClampAngle(h, -360.0f, 360.0f);
                v = ClampAngle(v, minVerticalAngle, maxVerticalAngle);
            }

            newRotation = Quaternion.Euler(v, h, 0.0f);
            smoothRotation = Quaternion.Slerp(smoothRotation, newRotation, TimeSignature((1 / rotationDampening) * 100.0f));

            smoothRotation.eulerAngles = new Vector3(smoothRotation.eulerAngles.x, smoothRotation.eulerAngles.y, 0.0f);

            ///*We set the position and rotation with a smooth linear interpolation for the camera
            // * rotation for a nicer viewing effect */

            cameraTransform.rotation = smoothRotation;

            tmpRot = new Vector3(0.0f, cameraTransform.eulerAngles.y, 0.0f);

            targetRotation.rotation = Quaternion.Euler(tmpRot);

            newPosition = viewTarget.position;
            newPosition.y += height + offset;

            NR_StandardLookAt.transform.position = newPosition + (viewTarget.forward * OffsetForward);

            lastTargetPos = viewTarget.position;

        }
    }


    /* This is where the camera checks for a collsion hit within a specified radius,
	 * and then moves the camera above the location it hit with an offset value */
    void CheckSphere()
    {
        /* Add height to our spherecast origin */
        Vector3 tmpVect = viewTarget.position;
        //tmpVect.x += offset;
        tmpVect.y += height;

        RaycastHit hit;

        /* Get the direction from the camera position to the origin */
        Vector3 dir = (newPosition - tmpVect).normalized;

        /* Check a radius for collision hits and then set the new position for
		 * the camera */
        if (Physics.SphereCast(tmpVect, 0.3f, dir, out hit, distance, collisionLayers))
        {
            newPosition = hit.point + (hit.normal * collisionOffset);
        }
    }
    /* This function uses a custom sine wave function to offset the camera height
     * giving the impression of head bobbing while walking */
    void HeadBobbing()
    {
        /* We check the speed of the player so the head bobbing only occurs when
		 * the player is moving */
        float tmpMag = (viewTarget.position - lastTargetPos).magnitude;

        offset = Wave(bobbingAmount, bobbingSpeed, period);
        period += tmpMag;

        if (period > 2.0f * Mathf.PI)
        {
            period -= 2.0f * Mathf.PI;
        }
    }


    /* Keeps the angles values within their specificed minimum and maximum
	 * inputs while at the same time putting the values back to 0 if they 
	 * go outside of the 360 degree range */
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private float ClampAngleSit(float angle, float min, float max)
    {
        if (angle < -540)

            angle += 540;

        if (angle > 540)
            angle -= 540;

        return Mathf.Clamp(angle, min, max);

    }

    /* This is our custom logistic growth time signature with speed as input */
    private float TimeSignature(float speed)
    {
        return 1.0f / (1.0f + 80.0f * Mathf.Exp(-speed * 0.02f));
    }

    /* This is our custom sine wave function with amplitude, frequency, and time as input */
    private float Wave(float amplitude, float freq, float time)
    {
        return amplitude * Mathf.Sin(2.0f * Mathf.PI * freq * time);
    }


}
