using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 7;
    public float jumpImpulse = 7;
    public float gravityModifier;
    public float climbSpeed;
    public Transform groundCheck1;
    public Transform groundCheck2;
    public Transform rightWallCheck1;
    public Transform rightWallCheck2;
    public Transform leftWallCheck1;
    public Transform leftWallCheck2;
    public LayerMask groundLayer;
    public float checkDistance;
    public Transform currentSpawn;

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
    private bool canClimb = false;
    private bool climbing = false;
    private bool stairTop = false;
    private bool isSpawning = false;
    private bool isDead = false;
    private float currentGrowth = 0;
    private float growthFactor = 1;
    private bool hasWon = false;

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
            CheckClimb();
        }
    }

    private void CheckMove()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (!nextToRightWall)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                anim.SetBool("Walking", true);
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
                anim.SetBool("Walking", true);
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
        rb.velocity = new Vector2(0, rb.velocity.y);
        anim.SetBool("Walking", false);
    }

    private void CheckJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            isGrounded = false;
            climbing = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            anim.SetBool("Jumping", true);

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
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isGrounded = true;
            endJump = false;
            anim.SetBool("Jumping", false);
        }
        else
        {
            if (!climbing)
            {
                rb.AddForce(Vector2.down * gravityModifier);
                isGrounded = false;
            }
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

    private void CheckClimb()
    {
        if (canClimb && !jumping)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                if (!stairTop)
                {
                    transform.Translate(Vector2.up * climbSpeed * Time.deltaTime);
                    StartClimb();
                }
            }
            
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector2.down * climbSpeed * Time.deltaTime);
                StartClimb();
            }
        }
    }

    private void StartClimb()
    {
        anim.SetBool("Jumping", false);
        anim.SetBool("Walking", false);
        anim.SetBool("Climbing", true);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        climbing = true;
        isGrounded = true;
        endJump = false;
    }

    private void Flip()
    {
        spriteRenderer.flipX = !facingRight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            canClimb = true;
        }

        else if (other.CompareTag("StairTop"))
        {
            stairTop = true;
        }

        else if (other.CompareTag("Spikes"))
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

        else if (other.CompareTag("Ending"))
        {
            if (!hasWon)
            {
                anim.SetTrigger("Win");
                hasWon = true;
                StopHorizontalMove();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Stairs"))
        {
            canClimb = false;
            climbing = false;
            anim.SetBool("Climbing", false);
        }

        else if (other.CompareTag("StairTop"))
        {
            stairTop = false;
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
        anim.SetBool("Walking", false);

        Invoke(nameof(Spawn), 1.5f);
    }

    private void EnableClimbAfterJump()
    {
        jumping = false;
    }
}