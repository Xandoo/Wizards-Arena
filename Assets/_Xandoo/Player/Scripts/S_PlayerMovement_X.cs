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
	private bool isSpectating = false;
    

    [SerializeField]
    private Vector3 velocity;

    private Vector3 move;
    private Vector2 mouseInput;
    private S_Player_X player;
    [SerializeField]
    private Animator[] anim;

    private float pitch;
    private float cooldownTimeStamp;

    [SerializeField]
    private bool isGrounded;

	private float colliderHeight;

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
			Cursor.lockState = CursorLockMode.Locked;
        }
        //anim = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsLocalPlayer)
        {
			if (isSpectating)
			{
				MovePlayer();
				RotatePlayer();
				specVerticalMovement();
			}
			else
			{
				if (!S_GameManager_X.Singleton.isPaused)
				{
					MovePlayer();
					RotatePlayer();
					JumpPlayer();
					Attack();
				}
				CheckIsGrounded();
				ApplyGravity();
			}
        }

    }

	private void specVerticalMovement()
	{
		Vector3 vMove = new Vector3();
		if (Input.GetButton("Jump"))
		{
			Debug.Log("Moving up");
			vMove = Vector3.up * Time.deltaTime;
		}
		if (Input.GetButton("Crouch"))
		{
			Debug.Log("Moving down");
			vMove = Vector3.down * Time.deltaTime;
		}
		//vMove = Vector3.ClampMagnitude(move, 1f);

		cc.Move(vMove * speed);
	}

	void Attack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (cooldownTimeStamp <= Time.time)
            {
                //This is where animaiton time remapping should go for changing cast time.
                //I am not at that point yet. I just wanna make something fun.
                anim[0].SetTrigger("Attack");
                anim[1].SetTrigger("Attack");
                StartCoroutine(player.CastSpell());
                cooldownTimeStamp = Time.time + player.spellSettings.GetCooldown();
            }
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

	public void SetSpectating()
	{
		isSpectating = true;

		CharacterController cc = GetComponent<CharacterController>();
		colliderHeight = cc.height;
		cc.height = 0;
	}

	public void SetNotSpectating()
	{
		isSpectating = false;

		CharacterController cc = GetComponent<CharacterController>();
		cc.height = colliderHeight;
	}
}
