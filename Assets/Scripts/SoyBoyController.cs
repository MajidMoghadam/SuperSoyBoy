﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D),typeof(Animator))]
public class SoyBoyController : MonoBehaviour
{
    public float speed = 14f;
    public float accel = 6f;
    public float airAccel = 3f;
    private Vector2 input;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator animator;

    public bool isJumping;
    public float jumpSpeed = 8f;
    private float rayCastLengthCheck = 0.05f;
    private float width;
    private float height;

    public float jumpDurationThreshold = 0.25f;
    private float jumpDuration;
    public float jump = 14f;

    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip slideClip;
    private AudioSource audioSource;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool PlayerIsOnGround()
    {
        // 1
        bool groundCheck1 = Physics2D.Raycast(
                                new Vector2(transform.position.x, transform.position.y - height/2), 
                                -Vector2.up, 
                                rayCastLengthCheck
                            );
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - height / 2),
                                -Vector2.up * rayCastLengthCheck,
                                Color.red);

        bool groundCheck2 = Physics2D.Raycast(
                                new Vector2(transform.position.x + (width - 0.2f), transform.position.y - height / 2),
                                -Vector2.up,
                                rayCastLengthCheck
                            );
        Debug.DrawRay(new Vector2(transform.position.x + (width - 0.2f), transform.position.y - height/2),
                                -Vector2.up * rayCastLengthCheck,
                                Color.red);

        bool groundCheck3 = Physics2D.Raycast(
                                new Vector2(transform.position.x - (width - 0.2f), transform.position.y - height/2), 
                                -Vector2.up, 
                                rayCastLengthCheck
                            );    
        Debug.DrawRay(new Vector2(transform.position.x - (width - 0.2f), transform.position.y - height/2),
                                -Vector2.up * rayCastLengthCheck,
                                Color.red);

        // 2
        if (groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //this method somply wants to check for any wall on either side
    //if it detects a wall (on left or right) it returns true
    public bool IsWallToLeftOrRight()
    {
        // check for wall on left
        bool wallOnleft = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y),
                                            -Vector2.right,
                                            rayCastLengthCheck);

        Debug.DrawRay(new Vector2(transform.position.x - width, transform.position.y),
                                            -Vector2.right * rayCastLengthCheck,
                                            Color.red);
        //check for wall on the right
        bool wallOnRight = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y),
                                            Vector2.right, 
                                            rayCastLengthCheck);

        Debug.DrawRay(new Vector2(transform.position.x + width, transform.position.y),
                                            Vector2.right * rayCastLengthCheck,
                                            Color.red);

        // check for wall on left OR right (either is a success)
        if (wallOnleft || wallOnRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool PlayerIsTouchingGroundOrWall()
    {
        if (PlayerIsOnGround() || IsWallToLeftOrRight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //this function returns -1 for wall on left, 1 for wall on right, 0 for no walls
    public int GetWallDirection()
    {
        //send a ray to the left to check for a wall
        bool isWallLeft = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y),
                                            -Vector2.right, 
                                            rayCastLengthCheck);

        //send a ray to the right to check for a wall
        bool isWallRight = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y),
                                             Vector2.right, 
                                             rayCastLengthCheck);
        if (isWallLeft)
        {
            return -1;
        }
        else if (isWallRight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        // 1
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Jump");

        animator.SetFloat("Speed", Mathf.Abs(input.x));


        // 2
        if (input.x > 0f)
        {
            sr.flipX = false;
        }
        else if (input.x < 0f)
        {
            sr.flipX = true;
        }

        if (input.y >= 1f)
        {
            jumpDuration += Time.deltaTime;
            animator.SetBool("IsJumping", true);
        }
        else
        {
            isJumping = false;
            animator.SetBool("IsJumping", false);
            jumpDuration = 0f;
        }

        if (PlayerIsOnGround() && !isJumping)
        {
            if (input.y > 0f)
            {
                isJumping = true;
                PlayAudioClip(jumpClip);
            }
            animator.SetBool("IsOnWall", false);
            if (input.x < 0f || input.x > 0f)
            {
                PlayAudioClip(runClip);
            }
        }

        if (jumpDuration > jumpDurationThreshold) input.y = 0f;
    }

    void FixedUpdate()
    {
        // 1
        var acceleration = 0f;
        if (PlayerIsOnGround())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }

        var xVelocity = 0f;
        // 2
        if (PlayerIsOnGround() && input.x == 0)
        {
            xVelocity = 0f;
        }
        else
        {
            xVelocity = rb.velocity.x;
        }


        var yVelocity = 0f;
        if (PlayerIsTouchingGroundOrWall() && input.y == 1)
        {
            yVelocity = jump;
        }
        else
        {
            yVelocity = rb.velocity.y;
        }

        // 3
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x)* acceleration,0));
        // 4
        rb.velocity = new Vector2(xVelocity, yVelocity);

        if (IsWallToLeftOrRight() && !PlayerIsOnGround() && input.y == 1)
        {
            rb.velocity = new Vector2(-GetWallDirection() * speed * 0.75f, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
            PlayAudioClip(jumpClip);
        }
        else if (!IsWallToLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
        }
        if (IsWallToLeftOrRight() && !PlayerIsOnGround())
        {
            animator.SetBool("IsOnWall", true);
            PlayAudioClip(slideClip);
        }

        if (isJumping && jumpDuration < jumpDurationThreshold)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    void PlayAudioClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            if (!audioSource.isPlaying) audioSource.PlayOneShot(clip);
        }
    }
}
