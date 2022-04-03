using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    //Player Movement
    [Header("Player Movement")]
    [SerializeField] private Transform playerOrientation= null;
    private Rigidbody playerRb;
    private Vector3 moveDirection = Vector3.zero;
    private float playerHeight;
    private float speed = 4f;
    private int slideSpeed = 50;
    private float movementMultiplier = 10f;
    private float airMovementMultiplier = 1f;
    private float groundDrag = 5f;
    private float airDrag = 0.5f;
    private float downForce = 300f; //For Extra Gravity
    
    //Jump Variables
    private float jumpForce = 10f;
    private int jumpsLeft;
    public int JumpsLeft { get { return jumpsLeft; } set { jumpsLeft = value; } }
    public const int MAXJUMPS = 1;

    private bool isRunning = false;
    public bool IsRunning { get { return isRunning; } }
    private bool isCrouching = false;
    private bool isSliding = false;

    //Ground variables
    [Header("Ground Variables")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] Transform groundCheck = null;
    private bool isGrounded = true;

    //Wall Running Variables
    [SerializeField] private LayerMask wallRunLayer;
    private float wallRunForce = 3000f;
    private float maxWallRunSpeed = 15f;
    private float wallDistance = 1f;
    private bool isWallRight = false;
    public bool IsWallRight { get { return isWallRight; } }
    private bool isWallLeft = false;
    public bool IsWallLeft { get { return isWallLeft; } }
    private bool isWallRunning = false;
    public bool IsWallRunning { get { return isWallRunning; } }

    //Player Death Canvas
    [SerializeField] private GameObject playerDeathCanvas = null;

    RaycastHit slopeHit;
    private Vector3 SlopeMoveDirection = Vector3.zero;

    private void Awake()
    {
        //Setting Instance
        instance = this;

        //Initializing Player
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        playerHeight = transform.localScale.y;

        //Initializing jumpsleft
        jumpsLeft = MAXJUMPS;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Start Game Time
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //Check Ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if(isGrounded)
        {
            if(JumpsLeft != MAXJUMPS)
                jumpsLeft = MAXJUMPS;

            if (isWallRunning)
                StopWallRun();
        }

        //Calculate Slope Direction
        SlopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        //Calling Functions
        PlayerInput();
        ControlDrag();
        CheckForWall();
        WallRunInput();

        //Fall Death Condition
        if(transform.position.y < -150f)
        {
            KillPlayer();
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void PlayerInput()
    {
        //------------------Basic Movement----------------------------
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        int slidingModifier = isSliding ? 0 : 1;

        moveDirection = playerOrientation.right * horizontal + playerOrientation.forward * vertical * slidingModifier;

        //-----------------------Running------------------------------
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = !isRunning ? true : false;
            speed = isRunning ? speed * 2 : speed / 2;
        }

        //--------------------Crouch/Sliding--------------------------
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouch();
        }
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopCrouch();
        }

        //JUMP
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0) 
        {
            Jump();
        }
    }

    void Movement()
    {
        //Extra Gravity
        if(!isWallRunning)
            playerRb.AddForce(Vector3.down * downForce * Time.deltaTime);

        //Moving Player
        if (isGrounded && !OnSlope())
        {
            playerRb.AddForce(moveDirection.normalized * speed * movementMultiplier, ForceMode.Acceleration);
        }

        if (isGrounded && OnSlope())
        {
            playerRb.AddForce(SlopeMoveDirection.normalized * speed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            playerRb.AddForce(moveDirection.normalized * speed * airMovementMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag()
    {
        if(isGrounded)
        {
            playerRb.drag = groundDrag;
        }
        else if (!isGrounded)
        {
            playerRb.drag = airDrag;
        }
    }

    void Jump()
    {
        if(!isWallRunning)
        {
            jumpsLeft--;
            playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        if(isWallRunning)
        {
            if((isWallLeft && !Input.GetKey(KeyCode.D)) || (isWallRight && !Input.GetKey(KeyCode.A)))
            {
                playerRb.AddForce(playerOrientation.up * jumpForce, ForceMode.Impulse);
            }

            if (isWallRight || isWallLeft || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                playerRb.AddForce(-playerOrientation.up * jumpForce);

            if(isWallRight && Input.GetKey(KeyCode.A))
                playerRb.AddForce(-playerOrientation.right * jumpForce, ForceMode.Impulse);
            if(isWallLeft && Input.GetKey(KeyCode.D))
                playerRb.AddForce(playerOrientation.right * jumpForce, ForceMode.Impulse);

        }
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 2f / 2 + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    #region Crouch Code
    void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = new Vector3(transform.localScale.x, playerHeight / 2, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        //Sliding
        if (isRunning && isCrouching && isGrounded && playerRb.velocity.magnitude > 0.5f)
        {
            isSliding = true;
            playerRb.AddForce(playerOrientation.forward * slideSpeed, ForceMode.Impulse);
        }
        if (isSliding) isSliding = false;
    }
    void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, playerHeight, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }
    #endregion

    #region Wall Run 

    void CheckForWall()
    {
        if (!isWallRunning)
        {
            isWallRight = Physics.Raycast(transform.position, playerOrientation.right, wallDistance, wallRunLayer);
            isWallLeft = Physics.Raycast(transform.position, -playerOrientation.right, wallDistance, wallRunLayer);
        }
        if (isWallRunning)
        {
            isWallRight = Physics.Raycast(transform.position, playerOrientation.right, wallDistance * 3f, wallRunLayer);
            isWallLeft = Physics.Raycast(transform.position, -playerOrientation.right, wallDistance * 3f, wallRunLayer);
        }

        //Stop Wall Run if there is no wall
        if (!isWallRight && !isWallLeft) StopWallRun();

        if(isWallLeft && isWallRight)
        {
            jumpsLeft = MAXJUMPS;
        }

    }

    void WallRunInput()
    {
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallRun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallRun();
    }

    void StartWallRun()
    {
        playerRb.useGravity = false;
        isWallRunning = true;

        if (playerRb.velocity.magnitude <= maxWallRunSpeed)
        {
            playerRb.AddForce(playerOrientation.forward * wallRunForce * Time.deltaTime);

            if (isWallRight)
                playerRb.AddForce(playerOrientation.right * wallRunForce / 5 * Time.deltaTime);
            else if (isWallLeft)
                playerRb.AddForce(-playerOrientation.right * wallRunForce / 5 * Time.deltaTime);
        }
    }

    void StopWallRun()
    {
        playerRb.useGravity = true;
        isWallRunning = false;
    }

    #endregion


    #region Player Death

    public void KillPlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        playerDeathCanvas.SetActive(true);
    }

    #endregion
}