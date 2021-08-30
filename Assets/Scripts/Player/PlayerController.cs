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
    public float hookedAcceleration;
    public float jumpImpulse = 7;
    //public float gravityModifier;

    [Header("Raycasts")]
    public Transform groundCheck1;
    public Transform groundCheck2;
    public Transform groundCheckM;
    public Transform wallCheck1;
    public Transform wallCheck2;
    //public Transform leftWallCheck1;
    //public Transform leftWallCheck2;
    public LayerMask groundLayer;
    public float checkDistance;

    [Header("Miscelaneous")]
    public Transform startingSpawn;
    public float CoyoteTime = 0.35f;
    public Animator HeadAnimator;
    public GameObject GroundPS;
    public GameObject GroundSmallPS;
    public Hair hair;
    public bool Tutorial;

    [Header("Blood")]
    public GameObject[] BloodStain;

    [Header("Shadow")]
    public GameObject Sombra;
    public float MaxDistanceShadow = 5f;
    public float ShadowScaleMax = 0.45f;

    private Transform currentSpawn;
    //private static float HookRatioSpeed = 0.45f;

    private bool facingRight = true;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private bool isGrounded = false;
    private bool nextToRightWall = false;
    private bool nextToLeftWall = false;
    private bool endJump = false;
    private bool isSpawning = false;
    private bool isDead = false;
    private float currentGrowth = 0;
    private float growthFactor = 1;
    private float _timerCoyote;
    private bool _hooked;

    private void OnDrawGizmos()
    {
        Debug.DrawRay(groundCheck1.position, Vector2.down * 0.02f);
        Debug.DrawRay(groundCheck2.position, Vector2.down * 0.02f);
        //Debug.DrawRay(leftWallCheck1.position, Vector2.left * 0.02f);
        //Debug.DrawRay(leftWallCheck2.position, Vector2.left * 0.02f);
        Debug.DrawRay(wallCheck1.position, Vector2.right * 0.02f);
        Debug.DrawRay(wallCheck2.position, Vector2.right * 0.02f);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        //hair = transform.parent.GetComponentInChildren<Hair>();
    }

    private void Start()
    {
        currentSpawn = startingSpawn;

        //Spawn();    // ESTO EN OTRO SITIO?
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _hooked && !hair._trapped)
        {
            UnhookPlayer();
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            endJump = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (!isDead && !isSpawning)
        {
            CheckWall();
            CheckMove();
            CheckJump();
            CheckShadow();
        }
    }


    private void CheckMove()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (!nextToRightWall)
            {
                if (_hooked && !isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x + (hookedAcceleration * Time.deltaTime), rb.velocity.y);
                    //if(rb.velocity.x < moveSpeed)
                    //{
                    //    rb.velocity = new Vector2(rb.velocity.x + (hookedAcceleration * hair.hairSegLen * Time.deltaTime), rb.velocity.y);
                    //}
                    //else
                    //{
                    //    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    //}
                }
                else
                {
                    rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                    anim.SetBool("IsRunning", true);
                }
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
                if (_hooked && !isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x - (hookedAcceleration * Time.deltaTime), rb.velocity.y);
                    //if (rb.velocity.x > -moveSpeed)
                    //{
                    //    rb.velocity = new Vector2(rb.velocity.x - (hookedAcceleration * hair.hairSegLen * Time.deltaTime), rb.velocity.y);
                    //}
                    //else
                    //{
                    //    rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                    //}
                }
                else
                {
                    rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                    anim.SetBool("IsRunning", true);
                }
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
        if(!_hooked || isGrounded)
            rb.velocity = new Vector2(0, rb.velocity.y);

        anim.SetBool("IsRunning", false);
    }

    private void CheckJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            AudioManager.Instance.PlayJumpSound();

            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
            anim.SetBool("IsGrounded", false);
            anim.SetTrigger("Jump");

            HeadAnimator.ResetTrigger("Grounded");
            HeadAnimator.SetTrigger("Jump");
        }

        if (endJump || _hooked)
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
            if(rb.velocity.y < 0.1f)
            {
                anim.SetBool("IsGrounded", true);
                HeadAnimator.SetTrigger("Grounded");

                _timerCoyote = CoyoteTime;
                //rb.velocity = new Vector2(rb.velocity.x, 0);
                isGrounded = true;
                endJump = false;
            }
        }
        else
        {
            //rb.AddForce(Vector2.down * gravityModifier);

            if (rb.velocity.y < -15)
            {
                rb.velocity = new Vector2(rb.velocity.x, -15);
            }
            anim.SetBool("IsGrounded", false);

            _timerCoyote -= Time.deltaTime;

            if(_timerCoyote <= 0)
            {
                if (isGrounded)
                {
                    anim.SetTrigger("Jump");
                }
                isGrounded = false;
            }
        }
    }
    private void CheckShadow()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(groundCheckM.position, Vector2.down, MaxDistanceShadow, groundLayer);
        if (hit)
        {
            Sombra.SetActive(true);
            Sombra.transform.position = hit.point;
            Sombra.transform.localScale = Mathf.Lerp(0.05f, ShadowScaleMax, 1 - hit.distance*2f / MaxDistanceShadow) * Vector3.one;
        }
        else
            Sombra.SetActive(false);
       

    }

    private void CheckWall()
    {
        if (Physics2D.Raycast(wallCheck1.position, Vector2.right, checkDistance, groundLayer) ||
            Physics2D.Raycast(wallCheck2.position, Vector2.right, checkDistance, groundLayer))
        {
            nextToRightWall = true;
        }
        else
        {
            nextToRightWall = false;
        }

        if (Physics2D.Raycast(wallCheck1.position, Vector2.left, checkDistance, groundLayer) ||
            Physics2D.Raycast(wallCheck2.position, Vector2.left, checkDistance, groundLayer))
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
        Debug.Log(other.tag);

        if (other.CompareTag("Spikes"))
        {
            Die();
            Vector2 Midpoint = transform.position - (transform.position - other.transform.position)/2;
            int index = UnityEngine.Random.Range(0, BloodStain.Length);
            Instantiate(BloodStain[index], Midpoint, Quaternion.identity, other.GetComponentInChildren<SpriteRenderer>().transform);
        }

        if (other.CompareTag("Checkpoint"))
        {
            if (currentSpawn != other.transform && !isDead)
            {
                currentSpawn = other.transform;
            }
        }
    }

    private void Spawn()
    {
        ResetHair();
        UnhookPlayer();

        capsuleCollider.enabled = true;

        anim.SetTrigger("Spawn");
        HeadAnimator.SetTrigger("Spawn");

        //transform.parent.position = currentSpawn.position;
        transform.position = currentSpawn.position;
        rb.velocity = Vector2.zero;
        spriteRenderer.flipY = false;

        facingRight = true;
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
        hair.EnableCollider();
    }

    public void Die()
    {
        if (!isDead)
        {
            AudioManager.Instance.PlayDeathSound();

            ResetHair();
            UnhookPlayer();
            hair.DisableCollider();
            isDead = true;
            spriteRenderer.flipY = true;
            rb.velocity = Vector2.up * 5;
            //capsuleCollider.enabled = false;

            anim.SetTrigger("Death");
            HeadAnimator.SetTrigger("Death");

            Invoke(nameof(Spawn), 1.5f);

            if (Tutorial)
                hair.hairSegLen = 0.05f;
        }
    }

    public void HookPlayer()
    {
        AudioManager.Instance.PlayHookSound();

        _hooked = true;
    }

    public void UnhookPlayer()
    {
        _hooked = false;
        hair.Unhook();
        ResetHair();
    }

    void ResetHair()
    {
        hair.TrapHair(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            if (collision.relativeVelocity.y >= 7)
                Instantiate(GroundPS, collision.contacts[0].point, Quaternion.identity);
            else
                Instantiate(GroundSmallPS, collision.contacts[0].point, Quaternion.identity);
        }
    }
}