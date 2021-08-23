using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Paramenters")]
    public float moveSpeed = 7;
    public float jumpImpulse = 7;
    public float gravityModifier;

    [Header("Raycasts")]
    public Transform groundCheck1;
    public Transform groundCheck2;
    public Transform rightWallCheck1;
    public Transform rightWallCheck2;
    public Transform leftWallCheck1;
    public Transform leftWallCheck2;
    public LayerMask groundLayer;
    public float checkDistance;

    [Header("Miscelaneous")]
    public Transform currentSpawn;
    public float CoyoteTime = 0.35f;

    private static float HookRatioSpeed = 0.45f;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private bool isGrounded = false;
    private bool nextToRightWall = false;
    private bool nextToLeftWall = false;
    private bool endJump = false;
    private bool jumping = false;
    private bool isSpawning = false;
    private bool isDead = false;
    private float currentGrowth = 0;
    private float growthFactor = 1;
    private float _timerCoyote;
    private bool hasWon = false;
    private bool _hooked;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(groundCheck1.position, Vector2.down * 0.02f);
        Debug.DrawRay(groundCheck2.position, Vector2.down * 0.02f);
        Debug.DrawRay(leftWallCheck1.position, Vector2.left * 0.02f);
        Debug.DrawRay(leftWallCheck2.position, Vector2.left * 0.02f);
        Debug.DrawRay(rightWallCheck1.position, Vector2.right * 0.02f);
        Debug.DrawRay(rightWallCheck2.position, Vector2.right * 0.02f);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        Spawn();    // ESTO EN OTRO SITIO?
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            endJump = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (!isDead && !isSpawning && !hasWon)
        {
            CheckWall();
            CheckMove();
            CheckJump();
        }
    }

    private void CheckMove()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (!nextToRightWall)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                anim.SetBool("IsRunning", true);
            }
            else
            {
                StopHorizontalMove();
            }

            if (!facingRight)
            {
                facingRight = true;
                Flip();
            }
        }

        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (!nextToLeftWall)
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                anim.SetBool("IsRunning", true);
            }
            else
            {
                StopHorizontalMove();
            }

            if (facingRight)
            {
                facingRight = false;
                Flip();
            }
        }

        else
        {
            StopHorizontalMove();
        }
    }

    private void StopHorizontalMove()
    {
        if(!_hooked)
            rb.velocity = new Vector2(0, rb.velocity.y);

        anim.SetBool("IsRunning", false);
    }

    private void CheckJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            anim.SetBool("IsGrounded", false);
            anim.SetTrigger("Jump");

            jumping = true;
            Invoke(nameof(EnableClimbAfterJump), 0.3f);
        }

        if (endJump)
        {
            if (rb.velocity.y > 2)
            {
                rb.AddForce(Vector2.down * 0.25f, ForceMode2D.Impulse);
            }
            else if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector2.down * 0.1f, ForceMode2D.Impulse);
            }
        }
    }

    private void CheckGround()
    {
        if (Physics2D.Raycast(groundCheck1.position, Vector2.down, checkDistance, groundLayer) ||
            Physics2D.Raycast(groundCheck2.position, Vector2.down, checkDistance, groundLayer))
        {
            if(rb.velocity.y < 0)
            {
                anim.SetBool("IsGrounded", true);

                _timerCoyote = CoyoteTime;
                //rb.velocity = new Vector2(rb.velocity.x, 0);
                isGrounded = true;
                endJump = false;
            }
        }
        else
        {
            //rb.AddForce(Vector2.down * gravityModifier);
            anim.SetBool("IsGrounded", false);

            _timerCoyote -= Time.deltaTime;
            if(_timerCoyote <= 0)
                isGrounded = false;
        }
    }

    private void CheckWall()
    {
        if (Physics2D.Raycast(rightWallCheck1.position, Vector2.right, checkDistance, groundLayer) ||
            Physics2D.Raycast(rightWallCheck2.position, Vector2.right, checkDistance, groundLayer))
        {
            nextToRightWall = true;
        }
        else
        {
            nextToRightWall = false;
        }

        if (Physics2D.Raycast(leftWallCheck1.position, Vector2.left, checkDistance, groundLayer) ||
            Physics2D.Raycast(leftWallCheck2.position, Vector2.left, checkDistance, groundLayer))
        {
            nextToLeftWall = true;
        }
        else
        {
            nextToLeftWall = false;
        }
    }



    private void Flip()
    {
        transform.localScale *= new Vector2(-1, 1);
        //spriteRenderer.flipX = !facingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spikes"))
        {
            Die();
        }

        else if (other.CompareTag("Checkpoint"))
        {
            if (currentSpawn != other.transform)
            {
                currentSpawn = other.transform;
            }
        }
    }

    private void Spawn()
    {
        capsuleCollider.enabled = true;
        transform.position = currentSpawn.position;
        rb.velocity = Vector2.zero;
        spriteRenderer.flipY = false;
        isSpawning = true;
        isDead = false;

        StartCoroutine(nameof(SpawnAnimation));
    }

    private IEnumerator SpawnAnimation()
    {
        currentGrowth = 0;

        while (currentGrowth < 1)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, currentGrowth);
            currentGrowth += growthFactor * Time.deltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one;
        isSpawning = false;
    }

    private void Die()
    {
        isDead = true;
        spriteRenderer.flipY = true;
        rb.velocity = Vector2.up * 5;
        capsuleCollider.enabled = false;
        anim.SetTrigger("Death");

        Invoke(nameof(Spawn), 1.5f);
    }

    private void EnableClimbAfterJump()
    {
        jumping = false;
    }

    public void HookPlayer()
    {
        _hooked = true;
        moveSpeed *= HookRatioSpeed;
    }

    public void UnhookPlayer()
    {
        _hooked = false;
        moveSpeed /= HookRatioSpeed;
    }
}