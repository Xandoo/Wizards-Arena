using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;

public class S_PlayerMovement_X : NetworkedBehaviour
{
    

    CharacterController cc;
    public Transform camTransform;
    public Transform groundCheckOrigin;
    public float speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;
    public float sensitivity = 3f;
    public float groundCheckDistance = 5f;
    public LayerMask groundLayer;
    

    [SerializeField]
    private Vector3 velocity;

    private Vector3 move;
    private Vector2 mouseInput;
    private S_Player_X player;
    private Animator[] anim;

    private float pitch;

    [SerializeField]
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer)
        {
            camTransform.GetComponent<AudioListener>().enabled = false;
            camTransform.GetComponent<Camera>().enabled = false;
        }
        else
        {
            cc = GetComponent<CharacterController>();
            player = GetComponent<S_Player_X>();
            anim = GetComponentsInChildren<Animator>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsLocalPlayer)
        {
            MovePlayer();
            RotatePlayer();
            JumpPlayer();
            CheckIsGrounded();
            ApplyGravity();
            Attack();
        }

    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            anim[0].SetTrigger("Attack");
            anim[1].SetTrigger("Attack");
        }
    }

    private void JumpPlayer()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //anim.SetFloat("JumpHeight", jumpHeight * -2f * gravity);
        }
    }

    void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheckOrigin.position, groundCheckDistance, groundLayer);
        anim[0].SetBool("Jump", !isGrounded);
        anim[1].SetBool("Jump", !isGrounded);
    }

    void MovePlayer()
    {
        move = new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime, 0, Input.GetAxis("Vertical") * Time.deltaTime);
        move = Vector3.ClampMagnitude(move, 1f);
        move = transform.TransformDirection(move);
        
        cc.Move(move * speed);
        anim[0].SetFloat("Speed", Input.GetAxis("Vertical"));
        anim[1].SetFloat("Speed", Input.GetAxis("Vertical"));
        anim[0].SetFloat("Direction", Input.GetAxis("Horizontal"));
        anim[1].SetFloat("Direction", Input.GetAxis("Horizontal"));

        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0)
        {
            anim[0].SetBool("IsMoving", false);
            anim[1].SetBool("IsMoving", false);
        }
        else
        {
            anim[0].SetBool("IsMoving", true);
            anim[1].SetBool("IsMoving", true);
        }
    }

    void RotatePlayer()
    {
        mouseInput.x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseInput.y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        transform.Rotate(0, mouseInput.x, 0);

        pitch -= mouseInput.y;
        pitch = Mathf.Clamp(pitch, -45f, 45f);
        camTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckOrigin.position, groundCheckDistance);
    }
}
