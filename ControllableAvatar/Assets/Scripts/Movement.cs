using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    //running fields
    public CharacterController controller;
    public Transform cam;
    public float speed;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;



    //jump field
    public float jumpSpeed = 6.0f;
    private bool DoubleJump = false;
    private float jumpTimer = 6;
    private bool onGround = true;
    private const int maxJump = 2;
    private int currentJump = 0;

    Vector3 move;


    //Speed boost
    private float boostTimer = 6;
    private bool boosting;
    private bool BoostSpeed = false;
    PlayerControls controls;

    //animation field
    private Animator animator;
    private float ySpeed;

    private float moveX;
    private float moveY;

    private float moveZ;

    private float targetAngle;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => move = Vector2.zero;
        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Attack.performed += Punch;

        controls.Enable();
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 moveDirection = movementValue.Get<Vector2>();

        moveX = moveDirection.x;
        moveZ = moveDirection.y;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boost")
        {
            BoostSpeed = true;
            Destroy(other.gameObject);
        }

        if (other.tag == "DoubleJump")
        {
            DoubleJump = true;
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        onGround = true;
        currentJump = 0;
    }


    void Update()
    {
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(moveX, moveY, moveZ).normalized;
        controller.SimpleMove(Vector3.forward * 0);
        animator.SetBool("IsIdle", true);

        if (Mathf.Abs(direction.x) >= 0.1f)
        {
            animator.SetBool("IsRun", true);
            animator.SetBool("IsJumping", false);
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Debug.Log("Should be moving");
        }

        else
        {
            //animator.SetBool("IsIdle", true);
        }

        if ((Mathf.Abs(direction.y) >= 0.1f))
        {
            animator.SetBool("IsJumping", true);
            moveY = moveY + (Physics.gravity.y * 2.0f * Time.deltaTime);
            onGround = false;

        }
        else
        {
            onGround = true;
            animator.SetBool("IsIdle", true);
        }

        controller.Move(direction.normalized * speed * Time.deltaTime);    

        if (controller.isGrounded)
        {
            print("CharacterController is grounded");
        }

        //Speed boost
        if (BoostSpeed)
        {
            speed = 20;
            boostTimer -= Time.deltaTime;
            if(boostTimer <= 0)
            {
                speed = 10;
                BoostSpeed = false;
                boostTimer = 3;
            }

        }

        //Dive
        if (Input.GetButtonDown("Dive"))
        {
            animator.SetTrigger("IsDiving");
        }

        //Double Jump
        if (DoubleJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                DoubleJump = false;
                boostTimer = 3;
                moveY = moveY + (Physics.gravity.y * 4.0f * Time.deltaTime);
            }
        }

    }
    void Jump()
    {
        //moveY = jumpSpeed;
        //moveY -= ySpeed * Time.deltaTime;
        //ySpeed += Physics.gravity.y * Time.deltaTime;
        //Debug.Log("jump input");

        moveY = 20.0f;

    }

    //Attack
    void Punch(InputAction.CallbackContext context)
    {
        animator.SetTrigger("IsPunching");
    }
}