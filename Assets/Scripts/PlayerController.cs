using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    

    private float direction;
    private float jump = 900;
    private bool isJumping;
    private bool grounded;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.5f;
    private float jumpBufferCounter;
   
    private bool isFacingRight = true;
    private BoxCollider2D coll;
    

    [SerializeField] private float maxSpeed = 15;
    [SerializeField] public float accelRate = 0.5f;
    [SerializeField] private float fallSpeed = 350;

    [SerializeField] private Animator squashStretcheAnimator;
    
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private SpriteRenderer sprite;


    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {



        Move();
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            squashStretcheAnimator.SetTrigger("Crouch");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            squashStretcheAnimator.SetTrigger("Stand");
        }
        
        grounded = isGrounded();
        Jump();
        Flip();
    }


    private void Flip()
    {
        if (isFacingRight && RB.velocity.x < 0)
        {
            isFacingRight = !isFacingRight;
        }

        if (!isFacingRight && RB.velocity.x > 0)
        {
            isFacingRight = !isFacingRight;
        }
        sprite.flipX = !isFacingRight;
    }

    private bool isGrounded()
    {
        //bool result = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        bool result = Physics2D.OverlapCircle(groundCheck.position, 0.2f, jumpableGround);

        if (result)
        {
            if (grounded == false)
            {
                squashStretcheAnimator.SetTrigger("Landing");
            }
        }
        return result;
    }

    #region Horizontal Movement
    private void Move()
    {
        direction = Input.GetAxis("Horizontal");
        float speedDif = maxSpeed * direction - RB.velocity.x;
        float movement = speedDif * accelRate;
        RB.AddForce(movement * Vector2.right);
    }

    #endregion

    #region Jump

    private void Jump()
    {
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            squashStretcheAnimator.SetTrigger("Jump");
            RB.AddForce(new Vector2(RB.velocity.x, jump));

            jumpBufferCounter = 0f;
            StartCoroutine(JumpCoolDown());
        }


        if (Input.GetButtonUp("Jump") && RB.velocity.y > 0)
        {
            RB.AddForce(new Vector2(RB.velocity.x, -fallSpeed));
            coyoteTimeCounter = 0;
        }
    }

    private IEnumerator JumpCoolDown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }

    #endregion




}

