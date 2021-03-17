
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public enum Weapon{
	UNARMED = 0,
	TWOHANDSWORD = 1,
	TWOHANDSPEAR = 2,
	TWOHANDAXE = 3,
	TWOHANDBOW = 4,
	TWOHANDCROSSBOW = 5,
	STAFF = 6,
	ARMED = 7,
	RELAX = 8,
	RIFLE = 9,
	TWOHANDCLUB = 10,
	SHIELD = 11,
	ARMEDSHIELD = 12
}

public enum RPGCharacterState{
	DEFAULT,
	BLOCKING,
	STRAFING,
	CLIMBING,
	SWIMMING
}

public class RPGCharacterController : MonoBehaviour{
	#region Variables

	//Components
    [Header("Components")]
	[HideInInspector]
	public static UnityEngine.AI.NavMeshAgent navMeshAgent;
	[HideInInspector]
	public Rigidbody rb;
    public GameObject NR_StandardLookAt;
    public Transform[] paths;
    private int destPoint = 0;
    public bool navMovement;
    public bool navMeshPathing;
    public bool navMeshAutoPlay;
    bool rotate;
    float rotateTime = 0;
    public Transform currentTarget;
    Vector2 smoothDeltaPositionNav = Vector2.zero;
    Vector2 velocityNav = Vector2.zero;
    public Slider scaleSlider;
    public Text scaleText;
    public Vector3 scaleAvatar;
    public Vector3 scaleAvatarReset;
    private float scaleX;
    private float scaleY;
    private float scaleZ;
    private float scaleSave;

    public Animator animator;
	//public Transform target;
	[HideInInspector]
	public Vector3 targetDashDirection;
	CapsuleCollider capCollider;
	ParticleSystem FXSplash;
	public Camera FPSCamera;
	public Vector3 waistRotationOffset;
	public RPGCharacterState rpgCharacterState = RPGCharacterState.DEFAULT;

    //jumping variables
    [Header("Jump")]
    public float gravity = -9.8f;
	[HideInInspector]
	public float gravityTemp = 0f;
	[HideInInspector]
	public bool canJump;
	bool isJumping = false;
	[HideInInspector]
	public bool isGrounded;
	public float jumpSpeed = 10;
	public float doublejumpSpeed = 10;
	bool doJump = false;
	bool doublejumping = true;
	[HideInInspector]
	public bool canDoubleJump = false;
	[HideInInspector]
	public bool isDoubleJumping = false;
	bool doublejumped = false;
	bool isFalling;
	bool startFall;
	float fallingVelocity = -0.5f;

    // Used for continuing momentum while in air
    [Header("Jump Momentum")]
    public float inAirSpeed = 4f;
	float maxVelocity = 2f;
	float minVelocity = -2f;

    //rolling variables
    [Header("Rolling")]
    public float rollSpeed = 8;
	bool isRolling = false;
	public float rollduration;

    //movement variables
    [Header("Movement")]
	public bool useMeshNav;
	[HideInInspector]
	public bool isMoving = false;
	[HideInInspector]
	public bool canMove = true;
    private bool pickupbool = false;
    private bool interactbool = false;
	public float walkSpeed = 1.0f;
	float moveSpeed;
	public float runSpeed = 2.0f;

    public float SpeedH, SpeedV, CompareH, CompareV, Acceleration, MaxH, MaxV;
    public float inputHorizontal, inputVertical;
    public bool ChangeDirH, ChangeDirV;

    float rotationSpeed = 20f;
  
	float x;
	float z;
	float dv;
	float dh;
	Vector3 inputVec;
	Vector3 newVelocity;

    //Weapon and Shield
    [Header("Weapong and Shield")]
    public Weapon weapon;
	[HideInInspector]
	public int rightWeapon = 0;
	[HideInInspector]
	public int leftWeapon = 0;
	[HideInInspector]
	public bool isRelax = true;
	int LockCount = 0;

    //isStrafing/action variables
    [Header("Straf/Action")]
    public bool hipShooting = false;
	[HideInInspector]
	public bool canAction = true;
	bool isStrafing = false;
	[HideInInspector]
	public bool isDead = false;
	[HideInInspector]
	public bool isBlocking = false;
	public float knockbackMultiplier = 1f;
	bool isKnockback;
//	[HideInInspector]
	public bool isSitting = false;
    public bool sittingDown = false;
//    [HideInInspector]
    public bool isNearChair = false;
    Chair chairNearby = null;
    public bool isNearStairs = false;
    Stairs stairsNearby = null;
    bool isAiming = false;
	[HideInInspector]
	public bool isClimbing = false;
	[HideInInspector]
	public bool isNearLadder = false;
	[HideInInspector]
	public bool isNearCliff = false;
	[HideInInspector]
	public GameObject ladder;
	[HideInInspector]
	public GameObject cliff;

    //Swimming variables
    [Header("Swimming")]
    public float inWaterSpeed = 8f;

    //Weapon Models
    [Header("Weapons")]
    public GameObject twoHandAxe;
	public GameObject twoHandSword;
	public GameObject twoHandSpear;
	public GameObject twoHandBow;
	public GameObject twoHandCrossbow;
	public GameObject twoHandClub;
	public GameObject staff;
	public GameObject swordL;
	public GameObject swordR;
	public GameObject maceL;
	public GameObject maceR;
	public GameObject daggerL;
	public GameObject daggerR;
	public GameObject itemL;
	public GameObject itemR;
	public GameObject shield;
	public GameObject pistolL;
	public GameObject pistolR;
	public GameObject rifle;
	public GameObject spear;

    #endregion

    //[Header("Fixation")]
    //public Transform lookTarget;

    #region Initialization

    void Awake(){
		//set the components
		navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		animator = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody>();
		capCollider = GetComponent<CapsuleCollider>();
		FXSplash = transform.GetChild(2).GetComponent<ParticleSystem>();
        NR_StandardLookAt = GameObject.Find("NR_StandardLookAt");
		//hide all weapons
		if(twoHandAxe != null){
			twoHandAxe.SetActive(false);
		}
		if(twoHandBow != null){
			twoHandBow.SetActive(false);
		}
		if(twoHandCrossbow != null){
			twoHandCrossbow.SetActive(false);
		}
		if(twoHandSpear != null){
			twoHandSpear.SetActive(false);
		}
		if(twoHandSword != null){
			twoHandSword.SetActive(false);
		}
		if(twoHandClub != null){
			twoHandClub.SetActive(false);
		}
		if(staff != null){
			staff.SetActive(false);
		}
		if(swordL != null){
			swordL.SetActive(false);
		}
		if(swordR != null){
			swordR.SetActive(false);
		}
		if(maceL != null){
			maceL.SetActive(false);
		}
		if(maceR != null){
			maceR.SetActive(false);
		}
		if(daggerL != null){
			daggerL.SetActive(false);
		}
		if(daggerR != null){
			daggerR.SetActive(false);
		}
		if(itemL != null){
			itemL.SetActive(false);
		}
		if(itemR != null){
			itemR.SetActive(false);
		}
		if(shield != null){
			shield.SetActive(false);
		}
		if(pistolL != null){
			pistolL.SetActive(false);
		}
		if(pistolR != null){
			pistolR.SetActive(false);
		}
		if(rifle != null){
			rifle.SetActive(false);
		}
		if(spear != null){
			spear.SetActive(false);
		}

        //target = GameObject.Find("taurus").transform;
        //sceneCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        //lookTarget = GameObject.Find("").transform;
        //lookTarget = target.transform;

        //scaleAvatarReset = transform.localScale;
        

        navMeshAgent.updateRotation = false;

        scaleX = transform.localScale.x;
        scaleY = transform.localScale.y;
        scaleZ = transform.localScale.z;
        
        scaleAvatar = new Vector3(scaleX, scaleY, scaleZ);

        scaleSlider = GameObject.Find("Avatar_Scale").GetComponent<Slider>();
        scaleSlider.onValueChanged.AddListener(delegate { scaleAvatarFunc(); });
        scaleAvatarFunc();
        scaleSave = GameObject.Find("Avatar_Scale").GetComponent<Slider>().value;
        //scaleSave = 0.9f;
        //scaleText = GameObject.Find("ScaleText").GetComponent<Text>();

    }

    private void OnEnable(){
        AndrewSMB.OnStandupEnded += OnStandupEnded;

        AndrewSMB.OnSitdown += OnSitdown;
    }

    private void OnDisable(){
        AndrewSMB.OnStandupEnded -= OnStandupEnded;

        AndrewSMB.OnSitdown -= OnSitdown;
    }
    #endregion

    #region UpdateAndInput

    void Update(){

        

        transform.localScale = scaleAvatar;

        // only trigger input change if direction has not changed, else use the initial value (1 or -1) 
        if (ChangeDirH == false)
        {
            CompareH = inputHorizontal;
        }

        // only trigger input change if direction has not changed, else use the initial value (1 or -1)
        if (ChangeDirV == false)
        {
            CompareV = inputVertical;
        }

        // if input is not zero then multiple speed by time by acceleration 
        if (inputHorizontal != 0 && ChangeDirH == false)
        {
            SpeedH = SpeedH + Acceleration * Time.deltaTime * 3.0f;

        }
        // else decelerate 
        else
        {
            SpeedH = SpeedH - Acceleration * Time.deltaTime * 7.0f;
        }

        // if input is not zero then multiple speed by time by acceleration
        if (inputVertical != 0 && ChangeDirV == false)
        {
            SpeedV = SpeedV + Acceleration * Time.deltaTime * 3.0f;

        }
        // else decelerate 
        else
        {
            SpeedV = SpeedV - Acceleration * Time.deltaTime * 7.0f;
        }

        // set speed up with a limit 
        SpeedH = Mathf.Clamp(SpeedH, 0.0f, MaxH);
        SpeedV = Mathf.Clamp(SpeedV, 0.0f, MaxV);

        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

		//input abstraction for easier asset updates using outside control schemes
		bool inputJump = Input.GetButtonDown("Jump");
		bool inputLightHit = Input.GetButtonDown("LightHit");
		bool inputDeath = Input.GetButtonDown("Death");
		bool inputUnarmed = Input.GetButtonDown("Unarmed");
		bool inputShield = Input.GetButtonDown("Shield");
		bool inputAttackL = Input.GetButtonDown("AttackL");
		bool inputAttackR = Input.GetButtonDown("AttackR");
		bool inputCastL = Input.GetButtonDown("CastL");
		bool inputCastR = Input.GetButtonDown("CastR");
		float inputSwitchUpDown = Input.GetAxisRaw("SwitchUpDown");
		float inputSwitchLeftRight = Input.GetAxisRaw("SwitchLeftRight");
		bool inputStrafe = Input.GetKey(KeyCode.LeftShift);
		float inputTargetBlock = Input.GetAxisRaw("TargetBlock");
		float inputDashVertical = Input.GetAxisRaw("DashVertical");
		float inputDashHorizontal = Input.GetAxisRaw("DashHorizontal");
		bool inputAiming = Input.GetButtonDown("Aiming");
        bool inputSitStand = Input.GetButtonDown("SitStand");
        bool inputNavMesh = Input.GetButtonDown("NavMesh");
        bool inputAvatarControl = Input.GetButtonDown("AvatarControl");
        bool inputNavMeshAutoPlay = Input.GetButtonDown("NavMeshAutoPlay");


        //Camera relative movement
        //Transform cameraTransform = sceneCamera.transform;
        //Forward vector relative to the camera along the x-z plane   
        //Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        Vector3 forward = transform.forward;
		forward.y = 0;
		forward = forward.normalized;
        //Right vector relative to the camera always orthogonal to the forward vector
        //Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 right = transform.right;
        //directional inputs
        dv = inputDashVertical;
		dh = inputDashHorizontal;

        

		if(!isRolling && !isAiming){
			targetDashDirection = dh * right + dv * -forward;
		}
		x = inputHorizontal;
//		x = 0;
		z = inputVertical;

        //Debug.Log(x + " --- " + z);

        inputVec = ((CompareH*SpeedH) * right) + ((CompareV * SpeedV) * forward);
        //inputVec = new Vector3(forward.x * x, 0, forward.z * z);

        //make sure there is animator on character
        if (animator){
			if(canMove && !isBlocking && !isDead && !useMeshNav){
			}
			else{
				inputVec = new Vector3(0, 0, 0);
			}
			if(inputJump){
				doJump = true;
			}
			else{
				doJump = false;
			}
			if(rpgCharacterState != RPGCharacterState.SWIMMING){
				Rolling();
				Jumping();
				Blocking();
			}
            if(NR_StandardLookAt.GetComponent<NR_StandardLookAt>().FreezeMe == false)
            {
                Look();
            }
			if(inputLightHit && canAction && isGrounded && !isBlocking){
				GetHit();
			}
			if(inputDeath && canAction && isGrounded && !isBlocking){
				if(!isDead){
					StartCoroutine(_Death());
				}
				else{
					StartCoroutine(_Revive());
				}
			}
			if(inputUnarmed && canAction && isGrounded && !isBlocking && weapon != Weapon.UNARMED){
				StartCoroutine(_SwitchWeapon(0));
			}
			if(inputShield && canAction && isGrounded && !isBlocking && leftWeapon != 7){
				StartCoroutine(_SwitchWeapon(7));
			}
			if(inputAttackL && canAction && isGrounded && !isBlocking){
				Attack(1);
			}
			if(inputAttackL && canAction && isGrounded && isBlocking){
				StartCoroutine(_BlockHitReact());
			}
			if(inputAttackR && canAction && isGrounded && !isBlocking){
				Attack(2);
			}
			if(inputAttackR && canAction && isGrounded && isBlocking){
				StartCoroutine(_BlockHitReact());
			}
			if(inputCastL && canAction && isGrounded && !isBlocking && !isStrafing){
				AttackKick(1);
			}
			if(inputCastL && canAction && isGrounded && isBlocking){
				StartCoroutine(_BlockBreak());
			}
			if(inputCastR && canAction && isGrounded && !isBlocking && !isStrafing){
				AttackKick(2);
			}
			if(inputCastR && canAction && isGrounded && isBlocking){
				StartCoroutine(_BlockBreak());
			}
			if(inputSwitchUpDown < -0.1f && canAction && !isBlocking && isGrounded){  
				SwitchWeaponTwoHand(0);
			}
			else if(inputSwitchUpDown > 0.1f && canAction && !isBlocking && isGrounded){  
				SwitchWeaponTwoHand(1);
			}
			if(inputSwitchLeftRight < -0.1f && canAction && !isBlocking && isGrounded){  
				SwitchWeaponLeftRight(0);
			}
			else if(inputSwitchLeftRight > 0.1f && canAction && !isBlocking && isGrounded){  
				SwitchWeaponLeftRight(1);
			}
            if(inputSitStand && isGrounded && !isBlocking && !isStrafing) {
                if (isSitting)
                    Stand();
                else
                    OnSitdown();
            }
			//if strafing 
			if(inputStrafe || inputTargetBlock > 0.1f && canAction && weapon != Weapon.RIFLE){  
				if(!isRelax){
					isStrafing = true;
					animator.SetBool("Strafing", true);
				}
				else{
					Aiming();
				}
				if(inputCastL && canAction && isGrounded && !isBlocking){
					CastAttack(1);

				}
				if(inputCastR && canAction && isGrounded && !isBlocking){
					CastAttack(2);
				}
			}
			else{  
				isStrafing = false;
				animator.SetBool("Strafing", false);
			}
			//Aiming, Good for testing navmesh
			if(Input.GetButtonDown("Pickup")){
                //if(useMeshNav){
                //	RaycastHit hit;
                //	if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
                //		navMeshAgent.destination = hit.point;
                //	}
                //}
                pickupbool = !pickupbool;

                if(pickupbool == true)
                {
                    Pickup();

                    canMove = false;
                    canAction = false;
                    isMoving = false;

                    animator.SetBool("Moving", false);

                    LockCount++;

                }

                if(pickupbool == false)
                {
                    LockCount--;

                    PickupBreak();



                }
                

          

				//if(weapon == Weapon.RIFLE){
				//	animator.SetTrigger("Attack2Trigger");
				//	if(hipShooting == true){
				//		animator.SetTrigger("Attack2Trigger");
				//	}
				//	else{
				//		animator.SetTrigger("Attack1Trigger");
				//	}
				//}
			}

            if (Input.GetButtonDown("Interact"))
            {
                //if(useMeshNav){
                //	RaycastHit hit;
                //	if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
                //		navMeshAgent.destination = hit.point;
                //	}
                //}
                interactbool = !interactbool;

                if (interactbool == true)
                {
                    Activate();

                    canMove = false;
                    canAction = false;
                    isMoving = false;

                    animator.SetBool("Moving", false);

                    LockCount++;

                }

                if (interactbool == false)
                {
                    LockCount--;

                    ActivateBreak();



                }




                //if(weapon == Weapon.RIFLE){
                //	animator.SetTrigger("Attack2Trigger");
                //	if(hipShooting == true){
                //		animator.SetTrigger("Attack2Trigger");
                //	}
                //	else{
                //		animator.SetTrigger("Attack1Trigger");
                //	}
                //}
            }


            //if (Input.GetMouseButtonDown(1))
            //{
            //    Activate();

            //}
            if (Input.GetMouseButtonDown(2)){
				animator.SetTrigger("ReloadTrigger");
			}
			if(inputAiming){
				if(!isAiming){
					isAiming = true;
					animator.SetBool("Aiming", true);
				}
				else{
					isAiming = false;
					animator.SetBool("Aiming", false);
				}
			}
			//Climbing
			if(rpgCharacterState == RPGCharacterState.CLIMBING && !isClimbing){
				if(inputVertical > 0.1f){
					animator.applyRootMotion = true;
					animator.SetTrigger("Climb-UpTrigger");
					isClimbing = true;
				}
				if(inputVertical < -0.1f){
					animator.applyRootMotion = true;
					animator.SetTrigger("Climb-DownTrigger");
					isClimbing = true;
				}
			}
			if(rpgCharacterState == RPGCharacterState.CLIMBING && isClimbing){
				if(inputVertical == 0){
					isClimbing = false;
				}
			}
		}
		else{
			Debug.LogWarning("ERROR: There is no animator for character.");
		}
		if(Input.GetKeyDown(KeyCode.T)){
			if(Time.timeScale != 1){
				Time.timeScale = 1;
			}
			else{
				Time.timeScale = 0.15f;
			}
		}
		if(Input.GetKeyDown(KeyCode.P)){
			if(Time.timeScale != 1){
				Time.timeScale = 1;
			}
			else{
				Time.timeScale = 0f;
			}
		}

               
            
        if (Input.GetButtonDown("NavMesh"))
        {

            rb.isKinematic = true;
            navMeshAgent.enabled = true;
            useMeshNav = true;
            navMeshPathing = true;
            //transform.rotation = Quaternion.Euler(0, 90.0f, 0);
            //Quaternion.FromToRotation(FPSCamera.transform.position, paths[destPoint].position);
            //Quaternion.Lerp(transform.rotation, paths[destPoint].rotation, Time.deltaTime);
        }


        if (Input.GetButtonDown("AvatarControl"))
        {

            navMeshAgent.enabled = false;
            rb.isKinematic = false;
            useMeshNav = false;
            navMeshPathing = false;
            navMeshAutoPlay = false;
            gameObject.tag = ("NR_Proximity");

        }

        if (Input.GetButtonDown("NavMeshAutoPlay"))
        {
            rb.isKinematic = true;
            navMeshAgent.enabled = true;
            useMeshNav = true;
            navMeshAutoPlay = true;

            //GoToNextPoint();


        }

        if (Input.GetKey(KeyCode.RightShift) && (Input.GetKeyDown(KeyCode.Alpha1)))
        {
            transform.position = new Vector3(-12.034f, 0.968f, -6.379f);

        }

        if (Input.GetKey(KeyCode.RightShift) && (Input.GetKeyDown(KeyCode.Alpha2)))
        {
            transform.position = new Vector3(-6.0f, 0, -5.7f);

        }

        //if (useMeshNav)
        //{


        //    if (navMeshAgent.velocity.sqrMagnitude > 0)
        //    {
        //        navMovement = true;

        //    }

        //    if (navMeshAgent.velocity.sqrMagnitude == 0)
        //    {
        //        navMovement = false;
        //    }

        //    if (navMovement == true)
        //    {
        //        //take forward dir 
        //        //add 10 units to forward dir to look at

        //        //Vector3 direction = navMeshAgent.destination - transform.position;
        //        //Quaternion rotation = Quaternion.LookRotation(direction);
        //        //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 30.0f * Time.deltaTime);

        //        //Vector3 cameraDirection = navMeshAgent.destination - FPSCamera.transform.position;
        //        //Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection);
        //        //FPSCamera.transform.rotation = Quaternion.Lerp(FPSCamera.transform.rotation, cameraRotation, 30.0f * Time.deltaTime);

        //        //Vector3 cameraDirection = navMeshAgent.destination - transform.position;
        //        //float angleBetween = Vector3.Angle(FPSCamera.transform.forward, cameraDirection);
        //        //transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 2.0f, angleBetween);

        //        Vector3 navDirection = currentTarget.position - transform.position;
        //        float angleBet = Vector3.Angle(transform.forward, navDirection);
        //        transform.Rotate(new Vector3(0, 1, 0), angleBet);


        //        // FPSCamera.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 1.0f, angleBet);

        //        //transform.LookAt(navMeshAgent.destination);
        //        //FPSCamera.transform.LookAt(navMeshAgent.destination, Vector3.up);


        //        //Vector3 navDirection = currentTarget.position - transform.position;
        //        //Quaternion targetRotation = Quaternion.LookRotation(navDirection);
        //        //rotate = true;
        //        //rotateTime = 0;

        //        //if (rotate)
        //        //{
        //        //    rotateTime += Time.deltaTime;
        //        //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateTime);
        //        //}

        //        //if (rotateTime > 1)
        //        //{
        //        //    rotate = false;

        //        //}

        //        animator.SetBool("Moving", true);
        //        isMoving = true;
        //        animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);

        //    }

        //    if (navMovement == false)
        //    {

        //        animator.SetBool("Moving", false);
        //        isMoving = false;

        //        currentTarget = paths[destPoint].transform;

        //    }


        //}

        //if (navMeshAutoPlay)
        //{

        //    if (paths.Length == 0)
        //        return;



        //    navMeshAgent.destination = paths[destPoint].position;

        //    // Choose the next point in the array as the destination,
        //    // cycling to the start if necessary.
        //    if (transform.position == navMeshAgent.destination)
        //    {
        //        destPoint = (destPoint + 1) % paths.Length;
        //    }


        //    //Vector3 navDirection = navMeshAgent.destination - transform.position;
        //    //float angleBet = Vector3.Angle(transform.forward, navDirection);
        //    //transform.Rotate(new Vector3(0, 1, 0), angleBet);

        //    animator.SetBool("Moving", true);
        //    isMoving = true;
        //    animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);

        //}


        // reset the direction change triggers if speed on either axis hits zero 
        if (SpeedH == 0.0f)
        {
            ChangeDirH = false;
        }
        if (SpeedV == 0.0f)
        {
            ChangeDirV = false;
        }

    }

    #endregion

    #region Fixed/Late Updates

    void FixedUpdate(){
		if(rpgCharacterState != RPGCharacterState.SWIMMING){
			CheckForGrounded();
			//apply gravity force
			rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
			//check if character can move
			if(canMove && !isBlocking && rpgCharacterState != RPGCharacterState.CLIMBING){
				AirControl();
			}
			//check if falling
			if(rb.velocity.y < fallingVelocity && rpgCharacterState != RPGCharacterState.CLIMBING){
				isFalling = true;
				animator.SetInteger("Jumping", 2);
				canJump = false;
			}
			else{
				isFalling = false;
			}
		}
		else{
			WaterControl();
		}
		moveSpeed = UpdateMovement();
	}

	//get velocity of rigid body and pass the value to the animator to control the animations
	void LateUpdate(){

        // use late update to check if direction has changed 
        if (ChangeDirH == false)
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
        }
        if (ChangeDirV == false)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
        }

        // check if sign of h on update has changed to late update 
        // use as a trigger 
        if (Math.Sign(inputHorizontal) != Math.Sign(CompareH) && SpeedH != 0.0f)
        {
            ChangeDirH = true;
        }
        if (Math.Sign(inputVertical) != Math.Sign(CompareV) && SpeedV != 0.0f)
        {
            ChangeDirV = true;
        }



        //Get local velocity of character
        float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
		float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
        //Update animator with movement values
        animator.SetFloat("Velocity X", velocityXel / runSpeed);
		animator.SetFloat("Velocity Z", velocityZel / runSpeed);
		//if character is alive and can move, set our animator
		if(!isDead && canMove){
			if(moveSpeed > 0){
                if (isNearStairs)
                    OnNearStairs(velocityZel / runSpeed);
                 
                    animator.SetBool("Moving", true);
				    isMoving = true;
			}
			else{
                if (isNearStairs)
                    OnNearStairs(velocityZel / runSpeed);

                animator.SetBool("Moving", false);
                isMoving = false;
			}
		}

        if (useMeshNav)
        {

            float distance = Vector3.Distance(transform.position, currentTarget.position);
            
           

            if (navMeshAgent.velocity.sqrMagnitude > 0)
            {
                navMovement = true;

            }

            if (navMeshAgent.velocity.sqrMagnitude == 0)
            {
                navMovement = false;
            }

            if (navMovement == true)
            {
                //take forward dir 
                //add 10 units to forward dir to look at

                //Vector3 direction = navMeshAgent.destination - transform.position;
                //Quaternion rotation = Quaternion.LookRotation(direction);
                //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 30.0f * Time.deltaTime);

                //Vector3 cameraDirection = navMeshAgent.destination - FPSCamera.transform.position;
                //Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection);
                //FPSCamera.transform.rotation = Quaternion.Lerp(FPSCamera.transform.rotation, cameraRotation, 30.0f * Time.deltaTime);

                //Vector3 cameraDirection = navMeshAgent.destination - transform.position;
                //float angleBetween = Vector3.Angle(FPSCamera.transform.forward, cameraDirection);
                //transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 2.0f, angleBetween);
                if(distance > 0.5f)
                {
                    Vector3 navDirection = (currentTarget.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(navDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2.0f);
                }
                

                //Vector3 navDirection = currentTarget.position - transform.position;
                //float angleBet = Vector3.Angle(transform.forward, navDirection);
                //transform.Rotate(new Vector3(0, 1, 0), angleBet);


                // FPSCamera.transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * 1.0f, angleBet);

                //transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);

                //transform.LookAt(navMeshAgent.destination);
                //FPSCamera.transform.LookAt(navMeshAgent.destination, Vector3.up);


                //Vector3 navDirection = currentTarget.position - transform.position;
                //Quaternion targetRotation = Quaternion.LookRotation(navDirection);
                //rotate = true;
                //rotateTime = 0;

                //if (rotate)
                //{
                //    rotateTime += Time.deltaTime;
                //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateTime);
                //}

                //if (rotateTime > 1)
                //{
                //    rotate = false;

                //}

                animator.SetBool("Moving", true);
                isMoving = true;
                animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);

            }

            if (navMovement == false)
            {

                animator.SetBool("Moving", false);
                isMoving = false;

                currentTarget = paths[destPoint].transform;

            }


        }

        if (navMeshAutoPlay)
        {
            //navMeshAgent.autoBraking = false;

            if (paths.Length == 0)
                return;

            gameObject.tag = ("navMeshAuto");

            navMeshAgent.destination = paths[destPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            if (transform.position == navMeshAgent.destination)
            {
                destPoint = (destPoint + 1) % paths.Length;
            }


            //Vector3 navDirection = navMeshAgent.destination - transform.position;
            //float angleBet = Vector3.Angle(transform.forward, navDirection);
            //transform.Rotate(new Vector3(0, 1, 0), angleBet);

            animator.SetBool("Moving", true);
            isMoving = true;
            animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);

            if(destPoint > 2 && destPoint < 5)
            {
                //gameObject.tag = ("Headbob");

            }

        }

        if (navMeshPathing)
        {

            if (paths.Length == 0)
                return;

            gameObject.tag = ("navMeshPath");

            navMeshAgent.destination = paths[destPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            if (transform.position == navMeshAgent.destination)
            {
                navMeshPathing = false;

                destPoint = (destPoint + 1) % paths.Length;
            }

            if (destPoint > 2 && destPoint < 5)
            {
                //gameObject.tag = ("Headbob");

            }

            //Vector3 navDirection = navMeshAgent.destination - transform.position;
            //float angleBet = Vector3.Angle(transform.forward, navDirection);
            //transform.Rotate(new Vector3(0, 1, 0), angleBet);

            animator.SetBool("Moving", true);
            isMoving = true;
            animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);

        }

    }

   



    #endregion

    #region UpdateMovement

    float UpdateMovement(){
        Vector3 motion = inputVec;
        if (isGrounded && rpgCharacterState != RPGCharacterState.CLIMBING)
        {
            //reduce input for diagonal movement
            if (motion.magnitude > 1)
            {
                motion.Normalize();
            }
            if (canMove && !isBlocking && !useMeshNav)
            {
                //set speed by walking / running
                if (isStrafing && !isAiming)
                {
                    newVelocity = motion * walkSpeed;
                }
                else
                {
                    newVelocity = motion * runSpeed;
                }
                //if rolling use rolling speed and direction
                if (isRolling)
                {
                    //force the dash movement to 1
                    targetDashDirection.Normalize();
                    newVelocity = rollSpeed * targetDashDirection;
                }
            }
        }
        else
        {
            if (rpgCharacterState != RPGCharacterState.SWIMMING)
            {
                //if we are falling use momentum
                newVelocity = rb.velocity;
            }
            else
            {
                newVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }
        if (isStrafing && !isRelax)
        {
            //make character point at target
            Quaternion targetRotation;
            //Vector3 targetPos = target.transform.position;
            //targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
            //transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
        }
        else if (isAiming)
        {
            Aiming();
        }
        else
        {
            if (canMove)
            {
                RotateTowardsMovementDir();
            }
        }
        //if we are falling use momentum
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
        //return a movement value for the animator
        return inputVec.magnitude;
    }

	//rotate character towards direction moved
	void RotateTowardsMovementDir(){
		//if(inputVec != Vector3.zero && !isStrafing && !isAiming && !isRolling && !isBlocking && rpgCharacterState != RPGCharacterState.CLIMBING){
		//	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
		//}
	}

    public void scaleAvatarFunc()
    {
        scaleX = scaleSlider.value;
        scaleY = scaleSlider.value;
        scaleZ = scaleSlider.value;

        scaleAvatar = new Vector3(scaleX, scaleY, scaleZ);
        float value = scaleSlider.value;
        //scaleText.text = value.ToString("F2");

    }

    public void scaleReset()
    {

        scaleSlider.value = scaleSave;

    }


	#endregion

	#region Aiming / Turning

	void Aiming(){
		for(int i = 0; i < Input.GetJoystickNames().Length; i++){
			//if the right joystick is moved, use that for facing
			float inputDashVertical = Input.GetAxisRaw("DashVertical");
			float inputDashHorizontal = Input.GetAxisRaw("DashHorizontal");
			if(Mathf.Abs(inputDashHorizontal) > 0.1 || Mathf.Abs(inputDashVertical) < -0.1){
				Vector3 joyDirection = new Vector3(inputDashHorizontal, 0, -inputDashVertical);
				joyDirection = joyDirection.normalized;
				Quaternion joyRotation = Quaternion.LookRotation(joyDirection);
				transform.rotation = joyRotation;
			}
		}
		//no joysticks, use mouse aim
		if(Input.GetJoystickNames().Length == 0){
			Plane characterPlane = new Plane(Vector3.up, transform.position);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Vector3 mousePosition = new Vector3(0, 0, 0);
			float hitdist = 0.0f;
			if(characterPlane.Raycast(ray, out hitdist)){
				mousePosition = ray.GetPoint(hitdist);
			}
			mousePosition = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);
			Vector3 relativePos = transform.position - mousePosition;
			Quaternion rotation = Quaternion.LookRotation(-relativePos);
			transform.rotation = rotation;

		}
	}

	//Turning
	public IEnumerator _Turning(int direction){
		if(direction == 1){
			StartCoroutine(_LockMovementAndAttack(0, 0.55f));
			animator.SetTrigger("TurnLeftTrigger");
		}
		if(direction == 2){
			StartCoroutine(_LockMovementAndAttack(0, 0.55f));
			animator.SetTrigger("TurnRightTrigger");
		}
		yield return null;
	}

	//Dodging
	public IEnumerator _Dodge(int direction){
		if(direction == 1){
			StartCoroutine(_LockMovementAndAttack(0, 0.55f));
			animator.SetTrigger("DodgeLeftTrigger");
		}
		if(direction == 2){
			StartCoroutine(_LockMovementAndAttack(0, 0.55f));
			animator.SetTrigger("DodgeRightTrigger");
		}
		yield return null;
	}

	#endregion

	#region Swimming

	void OnTriggerEnter(Collider collide){
        //If entering a water volume
        if (collide.gameObject.layer == 4) {
            rpgCharacterState = RPGCharacterState.SWIMMING;
            canAction = false;
            rb.useGravity = false;
            animator.SetTrigger("SwimTrigger");
            animator.SetBool("Swimming", true);
            animator.SetInteger("Weapon", 0);
            weapon = Weapon.UNARMED;
            StartCoroutine(_WeaponVisibility(leftWeapon, 0, false));
            StartCoroutine(_WeaponVisibility(rightWeapon, 0, false));
            animator.SetInteger("RightWeapon", 0);
            animator.SetInteger("LeftWeapon", 0);
            animator.SetInteger("LeftRight", 0);
            FXSplash.Emit(30);
        }
        else if (collide.GetComponent<Chair>() != null) {
            isNearChair = true;
            chairNearby = collide.GetComponent<Chair>();
            
            
        }
        else if (collide.GetComponent<Stairs>() != null)
        {
            isNearStairs = true;
            animator.SetBool("Stairs", true);
            stairsNearby = collide.GetComponent<Stairs>();
            rb.useGravity = false;
            walkSpeed = walkSpeed + 1.0f;
            runSpeed = runSpeed + 1.0f;
            navMeshAgent.speed = navMeshAgent.speed + 2.0f;

        }
        else if (collide.transform.parent != null) {
            if (collide.transform.parent.name.Contains("Ladder")) {
                isNearLadder = true;
                ladder = collide.gameObject;
            }
        }
        else if (collide.transform.name.Contains("Cliff")) {
            isNearCliff = true;
            cliff = collide.gameObject;
        }
	}

	void OnTriggerExit(Collider collide){
		//If leaving a water volume
		if(collide.gameObject.layer == 4){
			rpgCharacterState = RPGCharacterState.DEFAULT;
			canAction = true;
			rb.useGravity = true;
			animator.SetInteger("Jumping", 2);
			animator.SetBool("Swimming", false);
			capCollider.radius = 0.5f;
		}
        else if (collide.GetComponent<Chair>()){
            isNearChair = false;
            chairNearby = null;
        }
        else if (collide.GetComponent<Stairs>())
        {
            isNearStairs = false;
            animator.SetBool("Stairs", false);
            stairsNearby = null;
            rb.useGravity = true;
            walkSpeed = walkSpeed - 1.0f;
            runSpeed = runSpeed - 1.0f;
            navMeshAgent.speed = navMeshAgent.speed - 2.0f;
        }
        //If leaving a ladder
        else if(collide.transform.parent != null){
			if(collide.transform.parent.name.Contains("Ladder")){
				isNearLadder = false;
				ladder = null;
			}
		}
	}

	void WaterControl(){
		AscendDescend();
		Vector3 motion = inputVec;
		//dampen vertical water movement
		Vector3 dampenVertical = new Vector3(rb.velocity.x, (rb.velocity.y * 0.985f), rb.velocity.z);
		rb.velocity = dampenVertical;
		Vector3 waterDampen = new Vector3((rb.velocity.x * 0.98f), rb.velocity.y, (rb.velocity.z * 0.98f));
		//If swimming, don't dampen movement, and scale capsule collider
		if(moveSpeed < 0.1f){
			rb.velocity = waterDampen;
			capCollider.radius = 0.5f;
		}
		else{
			capCollider.radius = 1.5f;
		}
		rb.velocity = waterDampen;
		//clamp diagonal movement so its not faster
		motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;
		rb.AddForce(motion * inWaterSpeed, ForceMode.Acceleration);
		//limit the amount of velocity we can achieve to water speed
		float velocityX = 0;
		float velocityZ = 0;
		if(rb.velocity.x > inWaterSpeed){
			velocityX = GetComponent<Rigidbody>().velocity.x - inWaterSpeed;
			if(velocityX < 0){
				velocityX = 0;
			}
			rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
		}
		if(rb.velocity.x < minVelocity){
			velocityX = rb.velocity.x - minVelocity;
			if(velocityX > 0){
				velocityX = 0;
			}
			rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
		}
		if(rb.velocity.z > inWaterSpeed){
			velocityZ = rb.velocity.z - maxVelocity;
			if(velocityZ < 0){
				velocityZ = 0;
			}
			rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
		}
		if(rb.velocity.z < minVelocity){
			velocityZ = rb.velocity.z - minVelocity;
			if(velocityZ > 0){
				velocityZ = 0;
			}
			rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
		}
	}

	void AscendDescend(){
		if(doJump){
			//swim down with left control
			if(isStrafing){
				animator.SetBool("Strafing", true);
				animator.SetTrigger("JumpTrigger");
				rb.velocity -= inWaterSpeed * Vector3.up;
			}
			else{
				animator.SetTrigger("JumpTrigger");
				rb.velocity += inWaterSpeed * Vector3.up;
			}
		}
	}

	#endregion

	#region Jumping

	//checks if character is within a certain distance from the ground, and markes it IsGrounded
	void CheckForGrounded(){
		float distanceToGround;
		float threshold = .45f;
		RaycastHit hit;
		Vector3 offset = new Vector3(0, 0.4f, 0);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)){
			distanceToGround = hit.distance;
			if(distanceToGround < threshold){
				isGrounded = true;
				canJump = true;
				startFall = false;
				doublejumped = false;
				canDoubleJump = false;
				isFalling = false;
				if(!isJumping){
					animator.SetInteger("Jumping", 0);
				}
				//exit climbing on ground
				if(rpgCharacterState == RPGCharacterState.CLIMBING){
					animator.SetTrigger("Climb-Off-BottomTrigger");
					gravity = gravityTemp;
					rb.useGravity = true;
					rpgCharacterState = RPGCharacterState.DEFAULT;
				}
			}
			else{
                if(!isNearStairs)
				    isGrounded = false;
                else
                    isGrounded = true;
            }
        }
	}

	void Jumping(){
		if(isGrounded){
			if(canJump && doJump){
				StartCoroutine(_Jump());
			}
		}
		else{    
			canDoubleJump = true;
			canJump = false;
			if(isFalling){
				//set the animation back to falling
				animator.SetInteger("Jumping", 2);
				//prevent from going into land animation while in air
				if(!startFall){
					animator.SetTrigger("JumpTrigger");
					startFall = true;
				}
			}
			if(canDoubleJump && doublejumping && Input.GetButtonDown("Jump") && !doublejumped && isFalling){
				// Apply the current movement to launch velocity
				rb.velocity += doublejumpSpeed * Vector3.up;
				animator.SetInteger("Jumping", 3);
				doublejumped = true;
			}
		}
	}

	public IEnumerator _Jump(){
		isJumping = true;
		animator.SetInteger("Jumping", 1);
		animator.SetTrigger("JumpTrigger");
		// Apply the current movement to launch velocity
		rb.velocity += jumpSpeed * Vector3.up;
		canJump = false;
		yield return new WaitForSeconds(0.5f);
		isJumping = false;
      
	}

	void AirControl(){
		if(!isGrounded){
			Vector3 motion = inputVec;
			motion *= (Mathf.Abs(inputVec.x) == 1 && Mathf.Abs(inputVec.z) == 1) ? 0.7f : 1;
			rb.AddForce(motion * inAirSpeed, ForceMode.Acceleration);
			//limit the amount of velocity we can achieve
			float velocityX = 0;
			float velocityZ = 0;
			if(rb.velocity.x > maxVelocity){
				velocityX = GetComponent<Rigidbody>().velocity.x - maxVelocity;
				if(velocityX < 0){
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.x < minVelocity){
				velocityX = rb.velocity.x - minVelocity;
				if(velocityX > 0){
					velocityX = 0;
				}
				rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
			}
			if(rb.velocity.z > maxVelocity){
				velocityZ = rb.velocity.z - maxVelocity;
				if(velocityZ < 0){
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
			if(rb.velocity.z < minVelocity){
				velocityZ = rb.velocity.z - minVelocity;
				if(velocityZ > 0){
					velocityZ = 0;
				}
				rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
			}
		}
	}

	#endregion

	#region MiscMethods

	public void Climbing(){
		rpgCharacterState = RPGCharacterState.CLIMBING;
	}

	public void EndClimbing(){
		rpgCharacterState = RPGCharacterState.DEFAULT;
		gravity = gravityTemp;
		rb.useGravity = true;
		animator.applyRootMotion = false;
		canMove = true;
		isClimbing = false;
	}

	//0 = No side
	//1 = Left
	//2 = Right
	//3 = Dual
	//weaponNumber 0 = Unarmed
	//weaponNumber 1 = 2H Sword
	//weaponNumber 2 = 2H Spear
	//weaponNumber 3 = 2H Axe
	//weaponNumber 4 = 2H Bow
	//weaponNumber 5 = 2H Crowwbow
	//weaponNumber 6 = 2H Staff
	//weaponNumber 7 = Shield
	//weaponNumber 8 = L Sword
	//weaponNumber 9 = R Sword
	//weaponNumber 10 = L Mace
	//weaponNumber 11 = R Mace
	//weaponNumber 12 = L Dagger
	//weaponNumber 13 = R Dagger
	//weaponNumber 14 = L Item
	//weaponNumber 15 = R Item
	//weaponNumber 16 = L Pistol
	//weaponNumber 17 = R Pistol
	//weaponNumber 18 = Rifle
	//weaponNumber 19 == Right Spear
	//weaponNumber 20 == 2H Club
	public void Attack(int attackSide){
		if(canAction){
			//No controller input
			if(inputVec.magnitude == 0f){
				if(weapon == Weapon.UNARMED || weapon == Weapon.ARMED || weapon == Weapon.ARMEDSHIELD){
					int maxAttacks = 3;
					int attackNumber = 0;
					//left attacks
					if(attackSide == 1){
						animator.SetInteger("AttackSide", 1);
						//Left sword has 6 attacks
						if(leftWeapon == 8){
							attackNumber = UnityEngine.Random.Range(1, 6);
						}
						else{
							attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						}
					}
					//right attacks
					else if(attackSide == 2){
						animator.SetInteger("AttackSide", 2);
						//Right spear has 7 attacks
						if(rightWeapon == 19){
							attackNumber = UnityEngine.Random.Range(1, 7);
						}
						//Right sword has 6 attacks
						else if(rightWeapon == 9){
							attackNumber = UnityEngine.Random.Range(7, 12);
						}
						else{
							attackNumber = UnityEngine.Random.Range(3, maxAttacks + 3);
						}
					}
					//dual attacks
					else if(attackSide == 3){
						attackNumber = UnityEngine.Random.Range(1, maxAttacks);
					}
					if(isGrounded){
						if(attackSide != 3){
							animator.SetTrigger("Attack" + (attackNumber + 1).ToString() + "Trigger");
							if(leftWeapon == 12 || leftWeapon == 14 || rightWeapon == 13 || rightWeapon == 15 || rightWeapon == 19){
								StartCoroutine(_LockMovementAndAttack(0, 0.75f));
							}
							else{
								StartCoroutine(_LockMovementAndAttack(0, 0.7f));
							}
						}
						//Dual Attacks
						else{
							animator.SetTrigger("AttackDual" + (attackNumber + 1).ToString() + "Trigger");
							StartCoroutine(_LockMovementAndAttack(0, 0.75f));
						}
					}
				}
				else if(weapon == Weapon.SHIELD){
					int maxAttacks = 1;
					{
						int attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						if(isGrounded){
							animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
							StartCoroutine(_LockMovementAndAttack(0, 1.1f));
						}
					}
				}
				else if(weapon == Weapon.TWOHANDSPEAR){
					int maxAttacks = 10;
					{
						int attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						if(isGrounded){
							animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
							StartCoroutine(_LockMovementAndAttack(0, 1.1f));
						}
					}
				}
				else if(weapon == Weapon.TWOHANDCLUB){
					int maxAttacks = 10;
					{
						int attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						if(isGrounded){
							animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
							StartCoroutine(_LockMovementAndAttack(0, 1.1f));
						}
					}
				}
				else if(weapon == Weapon.TWOHANDSWORD){
					int maxAttacks = 11;
					{
						int attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						if(isGrounded){
							animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
							StartCoroutine(_LockMovementAndAttack(0, 1.1f));
						}
					}
				}
				else{
					int maxAttacks = 6;
					{
						int attackNumber = UnityEngine.Random.Range(1, maxAttacks);
						if(isGrounded){
							animator.SetTrigger("Attack" + (attackNumber).ToString() + "Trigger");
							if(weapon == Weapon.TWOHANDSWORD){
								StartCoroutine(_LockMovementAndAttack(0, 0.85f));
							}
							else if(weapon == Weapon.TWOHANDAXE){
								StartCoroutine(_LockMovementAndAttack(0, 1.5f));
							}
							else{
								StartCoroutine(_LockMovementAndAttack(0, 0.75f));
							}
						}
					}
				}
			}
			//Character is Moving, use running attacks.
			else if(weapon == Weapon.ARMED){
				if(attackSide == 1){
					animator.SetTrigger("Attack1Trigger");
				}
				if(attackSide == 2){
					animator.SetTrigger("Attack4Trigger");
				}
				if(attackSide == 3){
					animator.SetTrigger("AttackDual1Trigger");
				}
			}
			else{
			}
		}
	}

	public void AttackKick(int kickSide){
		if(isGrounded){
			if(kickSide == 1){
				animator.SetTrigger("AttackKick1Trigger");
			}
			else{
				animator.SetTrigger("AttackKick2Trigger");
			}
			StartCoroutine(_LockMovementAndAttack(0, 0.8f));
		}
	}

	//0 = No side
	//1 = Left
	//2 = Right
	//3 = Dual
	public void CastAttack(int attackSide){
		if(weapon == Weapon.UNARMED || weapon == Weapon.STAFF || weapon == Weapon.ARMED){
			int maxAttacks = 3;
			if(attackSide == 1){
				int attackNumber = UnityEngine.Random.Range(0, maxAttacks);
				if(isGrounded){
					animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, 0.8f));
				}
			}
			if(attackSide == 2){
				int attackNumber = UnityEngine.Random.Range(3, maxAttacks + 3);
				if(isGrounded){
					animator.SetTrigger("CastAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, 0.8f));
				}
			}
			if(attackSide == 3){
				int attackNumber = UnityEngine.Random.Range(0, maxAttacks);
				if(isGrounded){
					animator.SetTrigger("CastDualAttack" + (attackNumber + 1).ToString() + "Trigger");
					StartCoroutine(_LockMovementAndAttack(0, 1f));
				}
			}
		} 
	}

	public void Blocking(){
		if(Input.GetAxisRaw("TargetBlock") < -0.1f && canAction && isGrounded){
			if(!isBlocking){
				animator.SetTrigger("BlockTrigger");
			}
			isBlocking = true;
			canJump = false;
			animator.SetBool("Blocking", true);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			inputVec = Vector3.zero;
		}
		else{
			isBlocking = false;
			canJump = true;
			animator.SetBool("Blocking", false);
		}
	}

	public void GetHit(){
		if(weapon != Weapon.RIFLE){
			int hits = 5;
			int hitNumber = UnityEngine.Random.Range(0, hits);
			animator.SetTrigger("GetHit" + (hitNumber + 1).ToString() + "Trigger");
			StartCoroutine(_LockMovementAndAttack(0.1f, 0.4f));
			//apply directional knockback force
			if(hitNumber <= 1){
				StartCoroutine(_Knockback(-transform.forward, 8, 4));
			}
			else if(hitNumber == 2){
				StartCoroutine(_Knockback(transform.forward, 8, 4));
			}
			else if(hitNumber == 3){
				StartCoroutine(_Knockback(transform.right, 8, 4));
			}
			else if(hitNumber == 4){
				StartCoroutine(_Knockback(-transform.right, 8, 4));
			}
		}
		else{
			animator.SetTrigger("GetHit1Trigger");
		}
	}

	IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount){
		isKnockback = true;
		StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
		yield return new WaitForSeconds(.1f);
		isKnockback = false;
	}

	IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount){
		while(isKnockback){
			rb.AddForce(knockDirection * ((knockBackAmount + UnityEngine.Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
			yield return null;
		}
	}

	public IEnumerator _Death(){
		animator.SetTrigger("Death1Trigger");
		StartCoroutine(_LockMovementAndAttack(0.1f, 1.5f));
		isDead = true;
		animator.SetBool("Moving", false);
		inputVec = new Vector3(0, 0, 0);
		yield return null;
	}

	public IEnumerator _Revive(){
		animator.SetTrigger("Revive1Trigger");
		StartCoroutine(_LockMovementAndAttack(0f, 1.45f));
		isDead = false;
		yield return null;
	}

    #endregion

    #region Looking
    void Look()
    {
        if (!isSitting && useMeshNav == false)
        {
            //Vector3 lookDirection = target.position - transform.position;
            //lookDirection.y = 0;
            //transform.forward = lookDirection;

           // MouseXNormal = (MouseXNormal)

            animator.SetFloat("RotationLeft", Input.GetAxisRaw("Mouse X"));
            animator.SetFloat("RotationRight", Input.GetAxisRaw("Mouse X"));

            //Debug.Log(MouseXNormal);

      
        }
    }


    #endregion

    #region Rolling

    void Rolling(){
		//if(!isRolling && isGrounded && !isAiming){
		//	if(Input.GetAxis("DashVertical") > 0.5f || Input.GetAxis("DashVertical") < -0.5f || Input.GetAxis("DashHorizontal") > 0.5f || Input.GetAxis("DashHorizontal") < -0.5f){
		//		StartCoroutine(_DirectionalRoll());
		//	}
		//}
	}

	public IEnumerator _DirectionalRoll(){
		//check which way the dash is pressed relative to the character facing
		float angle = Vector3.Angle(targetDashDirection, -transform.forward);
		float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(targetDashDirection, transform.forward)));
		// angle in [-179,180]
		float signed_angle = angle * sign;
		//angle in 0-360
		float angle360 = (signed_angle + 180) % 360;
		//deternime the animation to play based on the angle
		if(angle360 > 315 || angle360 < 45){
			StartCoroutine(_Roll(1));
		}
		if(angle360 > 45 && angle360 < 135){
			StartCoroutine(_Roll(2));
		}
		if(angle360 > 135 && angle360 < 225){
			StartCoroutine(_Roll(3));
		}
		if(angle360 > 225 && angle360 < 315){
			StartCoroutine(_Roll(4));
		}
		yield return null;
	}

	public IEnumerator _Roll(int rollNumber){
		if(rollNumber == 1){
			animator.SetTrigger("RollForwardTrigger");
		}
		if(rollNumber == 2){
			animator.SetTrigger("RollRightTrigger");
		}
		if(rollNumber == 3){
			animator.SetTrigger("RollBackwardTrigger");
		}
		if(rollNumber == 4){
			animator.SetTrigger("RollLeftTrigger");
		}
		isRolling = true;
		yield return new WaitForSeconds(rollduration);
		isRolling = false;
	}

	//Placeholder functions for Animation events
	public void Hit(){
	}

	public void Shoot(){
	}

	public void FootR(){
	}

	public void FootL(){
	}

	public void Land(){
	}

	public void WeaponSwitch(){
	}

    #endregion

    #region Sit/Stand
    void OnSitdown()
    {
        if (!isNearStairs && !isSitting && !isMoving && weapon == Weapon.RELAX)
        {
            canAction = false;
            canMove = false;
            canJump = false;
            gameObject.tag = "Sitting";
            //sittingDown = true;

            if (!isNearChair)
            {
                StartCoroutine(_SittingDown());
            }

            if (isNearChair)
            {
                animator.SetTrigger("ChairTrigger");
                Physics.IgnoreCollision(chairNearby.chairCollider, capCollider, true);
                gameObject.tag = "Chair";
                StartCoroutine(_SitOnChair());

            }

            animator.SetInteger("Idle", 1);
            animator.SetTrigger("IdleTrigger");
            //animator.SetTrigger("SitDown");

            
        }
    }
    void Sit(){
        if (sittingDown && !isNearStairs && !isSitting){

            //canAction = false;
            isSitting = true;
            //canMove = false;
            //canJump = false;
            gameObject.tag = "Sit";
            //sittingDown = false;
            
            //animator.SetTrigger("SitDown");
        }
    }

    void Stand(){
        if (isSitting && sittingDown && !isMoving && weapon == Weapon.RELAX)
        {
            
            animator.SetInteger("Idle", 0);
            animator.SetTrigger("IdleTrigger");
            //animator.SetTrigger("SitDown");
            

            if (isNearChair) {
                Physics.IgnoreCollision(chairNearby.chairCollider, capCollider, false);
                
                StartCoroutine(_StandOfChair());
            }
        }
        gameObject.tag = "Stand";
        sittingDown = false;
        //isSitting = false;
		canMove = true;

    }

    void OnStandupEnded() {
        canAction = true;
        sittingDown = false;
        isSitting = false;
        canMove = true;
        canJump = true;
        gameObject.tag = "NR_Proximity";
    }
    #endregion

    #region Stairs
    public void OnNearStairs(float velZ)
    {
        //Check if going upstairs or downstairs and if going backward or forward of this direction
        if (velZ > 0)
        {
            //Grab the stairs forward and check if character is between X degrees around in that same direction
            float angleB = Vector3.Angle(stairsNearby.transform.forward, transform.forward);
            animator.SetFloat("StairsMovement", (angleB <= 90) ? velZ : -velZ);
        }
        else
        {
            float angleB = Vector3.Angle(stairsNearby.transform.forward, transform.forward);
            animator.SetFloat("StairsMovement", (angleB >= 90) ? velZ : -velZ);
        }
    }

    #endregion

    #region _Coroutines

    //method to keep character from moveing while attacking, etc
    public IEnumerator _LockMovementAndAttack(float delayTime, float lockTime){
		yield return new WaitForSeconds(delayTime);
		LockCount++;
		canAction = false;
		canMove = false;
        isMoving = false;
		animator.SetBool("Moving", false);
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		inputVec = new Vector3(0, 0, 0);
		animator.applyRootMotion = true;
		yield return new WaitForSeconds(lockTime);

		LockCount--;
		if(LockCount <= 0){
			canAction = true;
			canMove = true;
			animator.applyRootMotion = false;
		}
	}

	//for controller weapon switching
	void SwitchWeaponTwoHand(int upDown){
		canAction = false;
		int weaponSwitch = (int)weapon;
		if(upDown == 0){
			weaponSwitch--;
			if(weaponSwitch < 1){
				StartCoroutine(_SwitchWeapon(6));
			}
			else{
				StartCoroutine(_SwitchWeapon(weaponSwitch));
			}
		}
		if(upDown == 1){
			weaponSwitch++;
			if(weaponSwitch > 6){
				StartCoroutine(_SwitchWeapon(1));
			}
			else{
				StartCoroutine(_SwitchWeapon(weaponSwitch));
			}
		}
	}

	//for controller weapon switching
	void SwitchWeaponLeftRight(int upDown){
		int weaponSwitch = 0;
		canAction = false;
		if(upDown == 0){
			weaponSwitch = leftWeapon;
			if(weaponSwitch < 16 && weaponSwitch != 0 && leftWeapon != 7){
				weaponSwitch += 2;
			}
			else{
				weaponSwitch = 8;
			}
		}
		if(upDown == 1){
			weaponSwitch = rightWeapon;
			if(weaponSwitch < 17 && weaponSwitch != 0){
				weaponSwitch += 2;
			}
			else{
				weaponSwitch = 9;
			}
		}
		StartCoroutine(_SwitchWeapon(weaponSwitch));
	}

	//function to switch weapons
	//weaponNumber 0 = Unarmed
	//weaponNumber 1 = 2H Sword
	//weaponNumber 2 = 2H Spear
	//weaponNumber 3 = 2H Axe
	//weaponNumber 4 = 2H Bow
	//weaponNumber 5 = 2H Crowwbow
	//weaponNumber 6 = 2H Staff
	//weaponNumber 7 = Shield
	//weaponNumber 8 = L Sword
	//weaponNumber 9 = R Sword
	//weaponNumber 10 = L Mace
	//weaponNumber 11 = R Mace
	//weaponNumber 12 = L Dagger
	//weaponNumber 13 = R Dagger
	//weaponNumber 14 = L Item
	//weaponNumber 15 = R Item
	//weaponNumber 16 = L Pistol
	//weaponNumber 17 = R Pistol
	//weaponNumber 18 = Rifle
	//weaponNumber 19 == Right Spear
	//weaponNumber 20 == 2H Club
	public IEnumerator _SwitchWeapon(int weaponNumber){	
		//character is unarmed
		if(weapon == Weapon.UNARMED){
			StartCoroutine(_UnSheathWeapon(weaponNumber));
		}
		//character has 2 handed weapon
		else if(weapon == Weapon.STAFF || weapon == Weapon.TWOHANDAXE || weapon == Weapon.TWOHANDBOW || weapon == Weapon.TWOHANDCROSSBOW || weapon == Weapon.TWOHANDSPEAR || weapon == Weapon.TWOHANDSWORD || weapon == Weapon.RIFLE || weapon == Weapon.TWOHANDCLUB){
			StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
			yield return new WaitForSeconds(1.1f);
			if(weaponNumber > 0){
				StartCoroutine(_UnSheathWeapon(weaponNumber));
			}
			//switch to unarmed
			else{
				weapon = Weapon.UNARMED;
				animator.SetInteger("Weapon", 0);
			}
		}
		//character has 1 or 2 1hand weapons and/or shield
		else if(weapon == Weapon.ARMED || weapon == Weapon.SHIELD || weapon == Weapon.ARMEDSHIELD){
			//character is switching to 2 hand weapon or unarmed, put put away all weapons
			if(weaponNumber < 7 || weaponNumber > 17 || weaponNumber == 20){
				//check left hand for weapon
				if(leftWeapon != 0){
					StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
					yield return new WaitForSeconds(1.05f);
					if(rightWeapon != 0){
						StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
						yield return new WaitForSeconds(1.05f);
						//and right hand weapon
						if(weaponNumber != 0){
							StartCoroutine(_UnSheathWeapon(weaponNumber));
						}
					}
					if(weaponNumber != 0){
						StartCoroutine(_UnSheathWeapon(weaponNumber));
					}
				}
				//check right hand for weapon if no left hand weapon
				if(rightWeapon != 0){
					StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
					yield return new WaitForSeconds(1.05f);
					if(weaponNumber != 0){
						StartCoroutine(_UnSheathWeapon(weaponNumber));
					}
				}
			}
			//using 1 handed weapon(s)
			else if(weaponNumber == 7){
				if(leftWeapon > 0){
					StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
					yield return new WaitForSeconds(1.05f);
				}
				StartCoroutine(_UnSheathWeapon(weaponNumber));
			}
			//switching left weapon, put away left weapon if equipped
			else if((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16)){
				if(leftWeapon > 0){
					StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
					yield return new WaitForSeconds(1.05f);
				}
				StartCoroutine(_UnSheathWeapon(weaponNumber));
			}
			//switching right weapon, put away right weapon if equipped
			else if((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19)){
				if(rightWeapon > 0){
					StartCoroutine(_SheathWeapon(rightWeapon, weaponNumber));
					yield return new WaitForSeconds(1.05f);
				}
				StartCoroutine(_UnSheathWeapon(weaponNumber));
			}
		}
		yield return null;
	}

	public IEnumerator _SheathWeapon(int weaponNumber, int weaponDraw){
		if((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16)){
			animator.SetInteger("LeftRight", 1);
		}
		else if((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19)){
			animator.SetInteger("LeftRight", 2);
		}
		else if(weaponNumber == 7){
			animator.SetInteger("LeftRight", 1);
		} 
		if(weaponDraw == 0){
			//if switching to unarmed, don't set "Armed" until after 2nd weapon sheath
			if(leftWeapon == 0 && rightWeapon != 0){
				animator.SetBool("Armed", false);
			}
			if(rightWeapon == 0 && leftWeapon != 0){
				animator.SetBool("Armed", false);
			}
		}
		animator.SetTrigger("WeaponSheathTrigger");
		yield return new WaitForSeconds(0.1f);
		if(weaponNumber < 7 || weaponNumber == 18 || weaponNumber == 19 || weaponNumber == 20){
			leftWeapon = 0;
			animator.SetInteger("LeftWeapon", 0);
			rightWeapon = 0;
			animator.SetInteger("RightWeapon", 0);
			animator.SetBool("Shield", false);
			animator.SetBool("Armed", false);
		}
		else if(weaponNumber == 7){
			leftWeapon = 0;
			animator.SetInteger("LeftWeapon", 0);
			animator.SetBool("Shield", false);
		}
		else if((weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16)){
			leftWeapon = 0;
			animator.SetInteger("LeftWeapon", 0);
		}
		else if((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19)){
			rightWeapon = 0;
			animator.SetInteger("RightWeapon", 0);
		}
		//if switched to unarmed
		if(leftWeapon == 0 && rightWeapon == 0){
			animator.SetBool("Armed", false);
		}
		if(leftWeapon == 0 && rightWeapon == 0){
			animator.SetInteger("LeftRight", 0);
			animator.SetInteger("Weapon", 0);
			animator.SetBool("Armed", false);
			weapon = Weapon.UNARMED;
		}
		StartCoroutine(_WeaponVisibility(weaponNumber, 0.4f, false));
		StartCoroutine(_LockMovementAndAttack(0, 1));
		yield return null;
	}

	public IEnumerator _UnSheathWeapon(int weaponNumber){
		animator.SetInteger("Weapon", -1);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		//two handed weapons
		if(weaponNumber < 7 || weaponNumber == 18 || weaponNumber == 20){
			leftWeapon = weaponNumber;
			animator.SetInteger("LeftRight", 3);
			if(weaponNumber == 0){
				weapon = Weapon.UNARMED;
			}
			if(weaponNumber == 1){
				weapon = Weapon.TWOHANDSWORD;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.4f, true));
			}
			else if(weaponNumber == 2){
				weapon = Weapon.TWOHANDSPEAR;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.5f, true));
			}
			else if(weaponNumber == 3){
				weapon = Weapon.TWOHANDAXE;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.5f, true));
			}
			else if(weaponNumber == 4){
				weapon = Weapon.TWOHANDBOW;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.55f, true));
			}
			else if(weaponNumber == 5){
				weapon = Weapon.TWOHANDCROSSBOW;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.5f, true));
			}
			else if(weaponNumber == 6){
				weapon = Weapon.STAFF;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
			}
			else if(weaponNumber == 18){
				weapon = Weapon.RIFLE;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
			}
			else if(weaponNumber == 20){
				weapon = Weapon.TWOHANDCLUB;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
			}
		}
		//one handed weapons
		else{
			if(weaponNumber == 7){
				leftWeapon = 7;
				animator.SetInteger("LeftWeapon", 7);
				animator.SetInteger("LeftRight", 1);
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
				animator.SetBool("Shield", true);
			}
			else if(weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16){
				animator.SetInteger("LeftRight", 1);
				animator.SetInteger("LeftWeapon", weaponNumber);
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
				leftWeapon = weaponNumber;
				weaponNumber = 7;
			}
			else if(weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19){
				animator.SetInteger("LeftRight", 2);
				animator.SetInteger("RightWeapon", weaponNumber);
				rightWeapon = weaponNumber;
				StartCoroutine(_WeaponVisibility(weaponNumber, 0.6f, true));
				weaponNumber = 7;
				//set shield to false for animator, will reset later
				if(leftWeapon == 7){
					animator.SetBool("Shield", false);
				}
			}
		}
		if(weapon == Weapon.RIFLE){
			animator.SetInteger("Weapon", 8);
		}
		else if(weapon == Weapon.TWOHANDCLUB){
			animator.SetInteger("Weapon", 9);
		}
		else{
			animator.SetInteger("Weapon", weaponNumber);
		}
		animator.SetTrigger("WeaponUnsheathTrigger");
		StartCoroutine(_LockMovementAndAttack(0, 1.1f));
		yield return new WaitForSeconds(0.1f);
		if(leftWeapon == 7){
			if(rightWeapon == 0){
				animator.SetBool("Shield", true);
				weapon = Weapon.SHIELD;
			}
			else{
				animator.SetBool("Shield", true);
				weapon = Weapon.ARMEDSHIELD;
			}
			animator.SetBool("Shield", true);
		}
		if((leftWeapon > 6 || rightWeapon > 6) && weapon != Weapon.RIFLE && weapon != Weapon.TWOHANDCLUB){
			animator.SetBool("Armed", true);
			if(leftWeapon != 7){
				weapon = Weapon.ARMED;
			}
		}
		//For dual blocking
		if(rightWeapon == 9 || rightWeapon == 11 || rightWeapon == 13 || rightWeapon == 15 || rightWeapon == 17){
			if(leftWeapon == 8 || leftWeapon == 10 || leftWeapon == 12 || leftWeapon == 14 || leftWeapon == 16){
				yield return new WaitForSeconds(.1f);
				animator.SetInteger("LeftRight", 3);
			}
		}
		if(leftWeapon == 8 || leftWeapon == 10 || leftWeapon == 12 || leftWeapon == 14 || leftWeapon == 16){
			if(rightWeapon == 9 || rightWeapon == 11 || rightWeapon == 13 || rightWeapon == 15 || rightWeapon == 17){
				yield return new WaitForSeconds(.1f);
				animator.SetInteger("LeftRight", 3);
			}
		}
		yield return null;
	}

	public IEnumerator _WeaponVisibility(int weaponNumber, float delayTime, bool visible){
		yield return new WaitForSeconds(delayTime);
		if(weaponNumber == 1){
			twoHandSword.SetActive(visible);
		}
		if(weaponNumber == 2){
			twoHandSpear.SetActive(visible);
		}
		if(weaponNumber == 3){
			twoHandAxe.SetActive(visible);
		}
		if(weaponNumber == 4){
			twoHandBow.SetActive(visible);
		}
		if(weaponNumber == 5){
			twoHandCrossbow.SetActive(visible);
		}
		if(weaponNumber == 6){
			staff.SetActive(visible);
		}
		if(weaponNumber == 7){
			shield.SetActive(visible);
		}
		if(weaponNumber == 8){
			swordL.SetActive(visible);
		}
		if(weaponNumber == 9){
			swordR.SetActive(visible);
		}
		if(weaponNumber == 10){
			maceL.SetActive(visible);
		}
		if(weaponNumber == 11){
			maceR.SetActive(visible);
		}
		if(weaponNumber == 12){
			daggerL.SetActive(visible);
		}
		if(weaponNumber == 13){
			daggerR.SetActive(visible);
		}
		if(weaponNumber == 14){
			itemL.SetActive(visible);
		}
		if(weaponNumber == 15){
			itemR.SetActive(visible);
		}
		if(weaponNumber == 16){
			pistolL.SetActive(visible);
		}
		if(weaponNumber == 17){
			pistolR.SetActive(visible);
		}
		if(weaponNumber == 18){
			rifle.SetActive(visible);
		}
		if(weaponNumber == 19){
			spear.SetActive(visible);
		}
		if(weaponNumber == 20){
			twoHandClub.SetActive(visible);
		}
		yield return null;
	}

	public IEnumerator _BlockHitReact(){
		int hits = 2;
		int hitNumber = UnityEngine.Random.Range(0, hits);
		animator.SetTrigger("BlockGetHit" + (hitNumber + 1).ToString() + "Trigger");
		StartCoroutine(_LockMovementAndAttack(0.1f, 0.4f));
		StartCoroutine(_Knockback(-transform.forward, 3, 3));
		yield return null;
	}

	public IEnumerator _BlockBreak(){
		animator.applyRootMotion = true;
		animator.SetTrigger("BlockBreakTrigger");
		yield return new WaitForSeconds(1f);
		animator.applyRootMotion = false;
	}

	public void Pickup(){
        animator.SetBool("Pickup", true);
        animator.SetTrigger("PickupTrigger");
        StartCoroutine(_LockMovementAndAttack(0.0f, 0.0f));
    }

    public void PickupBreak()
    {
        animator.SetBool("Pickup", false);
        
        StartCoroutine(_LockMovementAndAttack(0.0f, 0.1f));
    }

    public void Activate()
    {
        animator.SetBool("Interact", true);
        animator.SetTrigger("ActivateTrigger");
        StartCoroutine(_LockMovementAndAttack(0.0f, 0.0f));
    }

    public void ActivateBreak()
    {
        animator.SetBool("Interact", false);

        StartCoroutine(_LockMovementAndAttack(0.0f, 0.1f));
    }


    IEnumerator _SittingDown()
    {

        yield return new WaitForSeconds(0.9f);
        sittingDown = true;
        animator.SetTrigger("SitDown");
        Sit();

    }
   

    IEnumerator _SitOnChair() {
        float duration = 0.5f;
        float t = 0;
        Vector3 initialPos = transform.position;
        Vector3 finalPos = new Vector3(chairNearby.sittingAnchor.position.x, transform.position.y, chairNearby.sittingAnchor.position.z);
        Quaternion initialRot = transform.rotation;
      

        if (chairNearby != null){
            while (t <= duration) {
                transform.position = Vector3.Lerp(initialPos, finalPos, t / duration);
                transform.rotation = Quaternion.Lerp(initialRot, chairNearby.sittingAnchor.rotation, t / duration);
                yield return null;
                t += Time.deltaTime;
            }
        }

        sittingDown = true;
        isSitting = true;
    }

    IEnumerator _StandOfChair()
    {
        float duration = 0.35f;
        float t = 0;
        Vector3 initialPos = transform.position;
       // Vector3 finalPos = transform.position;
        Vector3 finalPos = transform.position + chairNearby.sittingAnchor.forward * 1.0f;

        if (chairNearby != null)
        {
            while (t <= duration)
            {
                transform.position = Vector3.Lerp(initialPos, finalPos, t / duration);

                yield return null;
                t += Time.deltaTime;
            }
        }

        


    }

    #endregion

}