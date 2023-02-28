using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    

    private float direction;
    private float jump = 900;
    private bool isJumping;
    private bool grounded;
    private bool alive = true;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.5f;
    private float jumpBufferCounter;
   
    private bool isFacingRight = true;
    private BoxCollider2D coll;
    

    [SerializeField] private float maxSpeed = 15;
    [SerializeField] public float accelRate = 0.5f;
    [SerializeField] private float fallSpeed = 350;

    [SerializeField] private ParticleSystem deathAnimation;
    [SerializeField] private Animator squashStretcheAnimator;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask bounds;

    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private SpriteRenderer sprite;
    

    


    void Start()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    void Update()
    {

        if(alive)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.S))
            {
                squashStretcheAnimator.SetTrigger("Crouch");
                coll.size = coll.size / 2;
                coll.offset = new Vector2(coll.offset.x, coll.offset.y - 0.5f);
                maxSpeed = 5;

            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                squashStretcheAnimator.SetTrigger("Stand");
                coll.size = coll.size * 2;
                coll.offset = new Vector2(coll.offset.x, coll.offset.y + 0.5f);
                maxSpeed = 15;
            }

            grounded = IsGrounded();
            Jump();
            Flip();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Respawn") && alive)
        {
            alive = false;
            var em = deathAnimation.emission;

            em.enabled = true;
            deathAnimation.Play();
            Destroy(sprite);
            Destroy(RB);
        }

        if (other.CompareTag("Bounce"))
        {
            squashStretcheAnimator.SetTrigger("Jump");
        }
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


    private bool IsGrounded()
    {
        bool result = Physics2D.OverlapCircle(groundCheck.position, 0.2f, jumpableGround);

        if (result && !grounded)
        {
                squashStretcheAnimator.SetTrigger("Landing");
        }
        return result;
    }

    #region Horizontal Movement
    private void Move()
    {
        direction = Input.GetAxis("Horizontal");
        if(direction == 1 || direction == -1)
        {
            gameObject.transform.SetParent(null);
        }
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

