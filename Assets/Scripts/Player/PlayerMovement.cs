using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //Player Movement
    [Header("Player Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck = null;
    [SerializeField] Transform headPosition = null;
    [SerializeField] Transform crouchPosition = null;
    [SerializeField] Transform camera = null;

    private CharacterController playerController = null;
    private float gravity = -30f;
    private Vector3 velocity = Vector3.zero;
    private int totalJumps = 2;
    private int jumpsLeft;

    private bool isGrounded = true;
    private bool isRunning = false;
    private bool isSliding = false;


    // Start is called before the first frame update
    void Start()
    {
        //Initializing Player
        playerController = GetComponent<CharacterController>();

        jumpsLeft = totalJumps;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();

        if (isSliding)
        {
            playerController.Move(transform.forward * 10f * Time.deltaTime);
            StartCoroutine(StopSliding());
        }

    }

    //Player Movement Function
    void PlayerMove()
    {
        //--------------------------Basic Movement---------------------------------
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.right * horizontal + transform.forward * vertical;
        playerController.Move(movement * speed * Time.deltaTime);

        //Running 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!isRunning)
            {
                speed *= 2;
                isRunning = true;
            }
            else if (isRunning)
            {
                speed /= 2;
                isRunning = false;
            }
        }

        //--------------------------------------------------------------------------

        //--------------------------- GroundCheck/Jump-------------------------------
        //GroundCheck
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && velocity.y < 0f)
        {
            jumpsLeft = totalJumps;
            velocity.y = -1f;
        }
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpsLeft > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpsLeft--;
            isGrounded = false;

            if (isSliding)
            {
                isSliding = false;
                StopCoroutine(StopSliding());
            }
        }
        //Applying Gravity
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
        //---------------------------------------------------------------------------------

        //---------------------Crouch/Slide------------------------------------
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            camera.position = Vector3.Lerp(camera.position, crouchPosition.position, 1f);

            if (isRunning)
            {
                Slide();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl) && !isRunning)
        {
            camera.position = Vector3.Lerp(camera.position, headPosition.position, 1f);
        }
        //---------------------------------------------------------------------
    }

    void Slide()
    {
        isSliding = true;


    }

    IEnumerator StopSliding()
    {
        yield return new WaitForSeconds(3f);

        isSliding = false;
    }
}
