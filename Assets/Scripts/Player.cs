using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using Input = UnityEngine.Input;



[RequireComponent(typeof(Controller2D))]



public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeedX = 0.325f;
    bool fastXMovementFixedUpdate = false;
    float moveSpeedXScalar = 1f;
    [SerializeField] float runningMoveSpeedX = 1.4f;
    float velocityXSmoothing;
    float accelerationTimeAirborn = 0.2f;
    float accelerationTimeGrounded = 0.1f;



    [SerializeField] float jumpingHeight = 4f;
    [SerializeField] float lenghtOfJump = 8f;
    bool hasJumped = false;
    float jumpBufferTimeMidAir = 1000;
    [SerializeField] float midAirJumpBufferTime = 0.1f;
    float jumpBufferTimeFromGround = 1000;
    [SerializeField] float groundJumpBufferTime = 0.08f;
    bool hasJumpedAndNotLanded = false;
    [SerializeField] float velocityWhenReleasingSpaceScalar = 0.62f;
    bool velocityWhenReleasingSpaceScalarBool = false;
    bool FixedUpdateJump = false;



    [SerializeField] float floatyJumpGravity = 0.45f;
    [SerializeField] float sJumpGravity = 1.6f;
    [SerializeField] float normalJumpGravity = 0.83f;
    [SerializeField] float releaseSpaceJumpGravity = 0.61f;
    float gravityConstant = 1f;
    bool sFalling = false;
    float gravity;



    Vector3 velocity;



    Controller2D controller;



    void Start()
    {
        controller = GetComponent<Controller2D>();
    }



    private void FixedUpdate()
    {
        MovingPlayer();



        Jump();



        FallingVelocityScalar();



        controller.Move(velocity);
    }



    private void Update()
    {
        JumpingFunction();



        FastXMovemvent();
    }



    private void MovingPlayer()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }



        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));



        gravity = gravityConstant * (-8) * jumpingHeight * Mathf.Pow(moveSpeedX, 2) / Mathf.Pow(lenghtOfJump, 2);



        if (fastXMovementFixedUpdate)
        {
            moveSpeedXScalar = runningMoveSpeedX;
        }
        else
        {
            moveSpeedXScalar = 1f;
        }



        float targetVelocityX = input.x * moveSpeedX * moveSpeedXScalar;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirborn);
        velocity.y += gravity;
    }



    private void Jump()
    {
        if (FixedUpdateJump)
        {
            velocity.y = 4 * jumpingHeight * moveSpeedX / lenghtOfJump;
            gravityConstant = floatyJumpGravity;
            hasJumped = true;
            hasJumpedAndNotLanded = true;
            FixedUpdateJump = false;
        }
    }



    private void FallingVelocityScalar()
    {
        if (velocityWhenReleasingSpaceScalarBool)
        {
            velocity.y *= velocityWhenReleasingSpaceScalar;
            velocityWhenReleasingSpaceScalarBool = false;
        }
    }



    private void JumpingFunction()
    {
        NormalJump();



        MidAirBufferJump();



        GroundBufferJump();
    }



    private void NormalJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            FixedUpdateJump = true;
        }



        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            sFalling = true;
        }
        else
        {
            sFalling = false;
        }



        if (sFalling)
        {
            gravityConstant = sJumpGravity;
        }
        else if (velocity.y == 0f || velocity.y < 0f)
        {
            gravityConstant = normalJumpGravity;
        }
        else if (Input.GetKeyUp(KeyCode.Space) && hasJumped)
        {
            gravityConstant = releaseSpaceJumpGravity;
            velocityWhenReleasingSpaceScalarBool = true;
        }



        if (Input.GetKeyUp(KeyCode.Space))
        {
            hasJumped = false;
        }
    }



    private void MidAirBufferJump()
    {
        jumpBufferTimeMidAir += 1 * Time.deltaTime;



        if (Input.GetKeyDown(KeyCode.Space) && !controller.collisions.below)
        {
            jumpBufferTimeMidAir = 0;
        }



        if (controller.collisions.below && jumpBufferTimeMidAir < midAirJumpBufferTime)
        {
            FixedUpdateJump = true;
        }
    }



    private void GroundBufferJump()
    {
        if (hasJumpedAndNotLanded && jumpBufferTimeFromGround > 0.05f && controller.collisions.below)
        {
            hasJumpedAndNotLanded = false;
        }



        jumpBufferTimeFromGround += 1 * Time.deltaTime;



        if (controller.collisions.below)
        {
            jumpBufferTimeFromGround = 0;
        }



        if (Input.GetKeyDown(KeyCode.Space) && !controller.collisions.below && !hasJumpedAndNotLanded && jumpBufferTimeFromGround < groundJumpBufferTime)
        {
            FixedUpdateJump = true;
        }
    }



    private void FastXMovemvent()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            fastXMovementFixedUpdate = true;
        }
        else
        {
            fastXMovementFixedUpdate = false;
        }
    }
}