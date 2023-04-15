using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement_Esencial : MonoBehaviour
{
    [Header("Prototipo")]
    [SerializeField] private TMP_Text estadoText;
    [SerializeField] private TMP_Text isGroundedText;
    [SerializeField] private TMP_Text velocidadText;
    [SerializeField] private TMP_Text saltoText;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    /*
    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;*/

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Distance Check")]
    /*public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;*/
    public Transform groundCheck;
    public Transform headCheck;
    public float groundDistance = 0.1f;
    public float headDistance = 1.0f;
    public LayerMask whatIsGround;
    public LayerMask whatIsObstacle;
    bool grounded;
    bool roofed;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public enum MovementState //dashing, atacking
    {
        walking,
        air
    }

    public bool isRunning;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //staminaBar.maxValue = MaxStamina;

        //jump
        readyToJump = true;

        //startYScale = transform.localScale.y;

        isRunning = false;
    }

    private void Update()
    {
        ParaElPrototipo();

        //ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        roofed = Physics.CheckSphere(headCheck.position, headDistance, whatIsObstacle);

        MyInput();
        SpeedControl();
        StateHandler();

        //handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (transform.position.y > 1)
        {
            Vector3 friccionFix = transform.position;
            friccionFix.y = 1;
            transform.position = friccionFix;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        /*
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }*/
        // Mode - Walking
         if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        //calc move direc
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //reset yVel
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    private void ParaElPrototipo()
    {
        estadoText.text = "Estado: " + state.ToString();
        isGroundedText.text = "isGrounded: " + grounded.ToString();
        velocidadText.text = "velocidad: " + walkSpeed.ToString();
        saltoText.text = "salto: " +jumpForce.ToString();
    }
}
