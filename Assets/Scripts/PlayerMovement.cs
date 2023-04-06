using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    
    public AudioSource pasos;
    public bool Hactivo;
    public bool Vactivo;
    public bool Sactivo;

    public Slider staminaBar;

    public AudioSource cansancio;
    public AudioSource correr;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;

    [Header("Stamina")]
    public float Stamina = 100.0f;
    public float MaxStamina = 100.0f;
    [SerializeField]
    private float StaminaRegenTimer = 0.0f;
    [SerializeField]
    private float StaminaDecreasePerFrame = 1.0f;
    [SerializeField]
    private float StaminaIncreasePerFrame = 5.0f;
    [SerializeField]
    private float StaminaTimeToRegen = 3.0f;
    public TMP_Text staminaText;


    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

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

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    public bool isRunning;

    private void Start()
    {        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        staminaBar.maxValue = MaxStamina;

        //jump
        readyToJump = true;

        startYScale = transform.localScale.y;

        isRunning = false;
    }

    private void Update()
    {
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
        
        if (isRunning)
        {
            Stamina = Mathf.Clamp(Stamina - (StaminaDecreasePerFrame * Time.deltaTime), 0.0f, MaxStamina);
            StaminaRegenTimer = 0.0f;
        }
        else if (Stamina < MaxStamina)
        {
            if (StaminaRegenTimer >= StaminaTimeToRegen)
                Stamina = Mathf.Clamp(Stamina + (StaminaIncreasePerFrame * Time.deltaTime), 0.0f, MaxStamina);
            else
                StaminaRegenTimer += Time.deltaTime;
        }
        if(Stamina <= 0)
        {
            if (!cansancio.isPlaying)
            {
                cansancio.Play();
            }
        }else if (Stamina >= 50)
        {
            if (cansancio.isPlaying)
            {
                cansancio.Stop();
            }
        }
        //staminaText.text = "Stamina: " + Stamina.ToString("0");

        if (Input.GetButtonDown("Horizontal") && (!correr.isPlaying))
        {

            pasos.Play();
            correr.Pause();

        }
        if (Input.GetButtonDown("Horizontal") && (Sactivo == true))
        {

            pasos.Pause();
            correr.Play();

        }

        if (Input.GetButtonDown("Vertical") && (!correr.isPlaying))
        {

            pasos.Play();
            correr.Pause();

        }
        if (Input.GetButtonDown("Vertical") && (Sactivo == true))
        {

            pasos.Pause();
            correr.Play();

        }

        if (Input.GetKeyDown(sprintKey) && (Hactivo == true))
        {

            correr.Play();
            pasos.Pause();

        }

        if (Input.GetKeyDown(sprintKey) && (Vactivo == true))
        {

            correr.Play();
            pasos.Pause();

        }

        if (Input.GetButtonUp("Horizontal"))
        {

            pasos.Pause();
            correr.Pause();

        }

        if (Input.GetButtonUp("Vertical"))
        {

            pasos.Pause();
            correr.Pause();

        }
        if (Input.GetKeyUp(sprintKey) && (!pasos.isPlaying))
        {
            correr.Pause();
        }

        if (Input.GetKeyUp(sprintKey) && (Hactivo == true))
        {
            correr.Pause();
            pasos.Play();
        }

        if (Input.GetKeyUp(sprintKey) && (Vactivo == true))
        {
            correr.Pause();
            pasos.Play();
        }

        if (transform.position.y > 1)
        {
            Vector3 friccionFix = transform.position; 
            friccionFix.y = 1;
            transform.position = friccionFix;
        }

        staminaBar.value = Stamina;
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
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        //stop crouch
        if (!Input.GetKey(crouchKey) && state!=MovementState.crouching && !roofed)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);            
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            Hactivo = true;
        }


        if (Input.GetButtonUp("Horizontal"))
        {
            Hactivo = false;
        }


        if (Input.GetButtonDown("Vertical"))
        {
            Vactivo = true;
        }


        if (Input.GetButtonUp("Vertical"))
        {
            Vactivo = false;
        }

        if (Input.GetKeyDown(sprintKey))
        {
            Sactivo = true;
        }


        if (Input.GetKeyUp(sprintKey))
        {
            Sactivo = false;
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey) && Stamina > 0)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Mode - Walking
        else if (grounded)
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
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
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


}
