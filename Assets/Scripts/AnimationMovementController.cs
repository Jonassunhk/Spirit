using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimationMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;

    int isRunningHash;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    Animator animator;

    //constants
    float rotationFactorPerFrame = 10.0f;
    float runSpeed = 7f;
    float gravity = -7.8f;
    float groundedGravity = -0.05f;

    //jumping variables
    bool isJumpPressed = false;
    float maxJumpHeight = 0.7f;
    float jumpRunSpeed = 8.8f;
    float initialJumpVelocity;
    float maxJumpTime = 0.8f;
    float fallMultiplier = 1.2f;
    bool isJumping = false;
    int isJumpingHash;
    bool isJumpAnimating = false;


    private void Awake()
    {
        Debug.Log("here");
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed) {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
        } else if (!isJumpPressed && isJumping && characterController.isGrounded) {
            isJumping = false;
        }
    }

    void onJump(InputAction.CallbackContext context) 
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void onMovementInput (InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        
        if (characterController.isGrounded) {
            if (isJumpAnimating) {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false; 
            }
            currentMovement.y = groundedGravity;
        } else if (isFalling) {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelocity;
        } else {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
        }
    }


    void handleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    void handleAnimation()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isJumping = animator.GetBool(isJumpingHash);

        if (isMovementPressed && !isRunning) {
            animator.SetBool(isRunningHash, true);
        }
        
        else if (!isMovementPressed && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();
        Debug.Log(currentMovement);
        if (isJumping) {
            characterController.Move(currentMovement * Time.deltaTime * jumpRunSpeed);
        } else {
            characterController.Move(currentMovement * Time.deltaTime * runSpeed);
        }
        
        handleGravity();
        handleJump();
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();        
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
