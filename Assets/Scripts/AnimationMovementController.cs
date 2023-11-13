using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;


public class AnimationMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;

    int isRunningHash, isJumpingHash, inWaterHash, isSwimmingHash, leftLegFrontHash, isPushingHash, isDeadHash, respawnHash;
    Quaternion targetRotation;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    Animator animator;

    //constants
    float rotationFactorPerFrame = 10.0f;
    float runSpeed = 7f;
    float gravity = -6f;
    float groundedGravity = -0.05f;

    //jumping variables
    bool isJumpPressed = false;
    float maxJumpHeight = 0.4f;
    float jumpRunSpeed = 8.8f;
    float initialJumpVelocity;
    float maxJumpTime = 0.8f;
    float fallMultiplier = 1.2f;
    bool isJumping = false;
    bool isJumpAnimating = false;

    //swimming variables
    float waterRotationFactorPerFrame = 1.0f;
    float swimSpeed = 4.5f;
    //bool isSwimming = false;
    float waterLevel = -21f;
    bool inWater = false;

    // obstacle detection
    float detectDistance = 2f;
    float pushStrength = 0.5f;
    float pushRunSpeed = 3f;

    // light detection
    public GameManager gameManager;
    public lightFlashScript lightFlash;
    float blockDetectDistance = 20f;



    private void Awake()
    {
        Debug.Log("here");
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        inWaterHash = Animator.StringToHash("inWater");
        isSwimmingHash = Animator.StringToHash("isSwimming");
        leftLegFrontHash = Animator.StringToHash("leftLegFront");
        isPushingHash = Animator.StringToHash("isPushing");
        isDeadHash = Animator.StringToHash("isDead");
        respawnHash = Animator.StringToHash("respawn");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        targetRotation = Quaternion.Euler(0, 0, 0);
        setupJumpVariables();
    }

    public void respawn(GameState gameState)
    {
        Vector3 pos = new Vector3(0,0,0);

        if (gameState == GameState.Hallway) { 
            pos = gameManager.hallwaySpawn.transform.position;
        } else if (gameState == GameState.Gym)
        {
            pos = gameManager.gymSpawn.transform.position;
        } else if (gameState == GameState.Classroom)
        {
            pos = gameManager.classroomSpawn.transform.position;
        } else if (gameState == GameState.Pool)
        {
            pos = gameManager.poolSpawn.transform.position;
        }
        characterController.enabled = false;
        characterController.transform.position = pos;
        characterController.enabled = true;
        animator.SetTrigger(respawnHash);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed && !inWater && !animator.GetBool(isPushingHash)) {
            
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
        if (inWater) {
            currentMovement.y = currentMovementInput.y;
        }
        
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
    
    void resetParameters() {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.CompareTag("Water"))
        {
            resetParameters();
            inWater = true;
            isJumping = false;
            animator.SetBool(inWaterHash, true);

            Debug.Log("Collided with Water!");
        } else if (other.CompareTag("Respawn Point"))
        {
            switch (other.name)
            {
                case "Pool":
                    gameManager.UpdateGameState(GameState.Pool);
                    break;
                case "Gym":
                    gameManager.UpdateGameState(GameState.Gym);
                    break;
                case "Hallway":
                    gameManager.UpdateGameState(GameState.Hallway);
                    break;
                case "Classroom":
                    gameManager.UpdateGameState(GameState.Classroom);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Name", other.name,null);
   
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision exit");
        if (other.CompareTag("Water"))
        {
            resetParameters();
            inWater = false;
            Debug.Log("Collision exited with Water!");
        }
    }

    void handleObstacles()
    {
        RaycastHit hit;
        animator.SetBool(isPushingHash, false);
        if (Physics.Raycast(characterController.transform.position, characterController.transform.forward, out hit, detectDistance)) { 
            if (hit.collider.tag == "Obstacle")
            {
                Debug.Log("Obstacle in range");
                bool isRunning = animator.GetBool(isRunningHash);
                if (isRunning)
                {
                    animator.SetBool(isPushingHash, true);
                    Rigidbody body = hit.collider.attachedRigidbody;
                    if (body != null)
                    {
                        body.AddForce(characterController.transform.forward * pushStrength);
                    }
                }
            }
        }
    }
    void handleLightBlockers()
    {
        
            bool avoided = true;
            RaycastHit hit;
            if (Physics.Raycast(characterController.transform.position, new Vector3(0,0,1), out hit, blockDetectDistance))
            {
                if (hit.collider.tag == "Harmful Light")
                {
                    avoided = false;
                    Debug.Log("player hit harmful light");
                }
            }
            if (!avoided && gameManager.State != GameState.PlayerDeath && lightFlash.lightOn)
            {
                // player death
                gameManager.UpdateGameState(GameState.PlayerDeath);
                Debug.Log("player is dead");
            }
        
    }

    void GameManagerOnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.PlayerDeath) // player died, play death animation
        {
            Debug.Log("death animation played");
            animator.SetTrigger(isDeadHash);

        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        float rotationFactor;
        

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.z = currentMovement.z;

        if (inWater) {
            positionToLookAt.y = currentMovement.y;
            rotationFactor = waterRotationFactorPerFrame;
        } else {
            positionToLookAt.y = 0.0f;
            rotationFactor = rotationFactorPerFrame;
        }

        Quaternion currentRotation = transform.rotation;


        if (isMovementPressed) { // when moving
            
            targetRotation = Quaternion.LookRotation(positionToLookAt);
            //Debug.Log(targetRotation.eulerAngles.y);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactor * Time.deltaTime);

        } else if (inWater) { // when not moving in water
            Quaternion idletargetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(currentRotation, idletargetRotation, rotationFactor * Time.deltaTime);
        }
        
    }

    void handleAnimation()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool inWater = animator.GetBool(inWaterHash);
        bool isSwimming = animator.GetBool(isSwimmingHash);
        
        if (inWater) { // character is underwater
            if (isMovementPressed && !isSwimming) {
                animator.SetBool(isSwimmingHash, true);
            }

            else if (!isMovementPressed && isSwimming) {
                animator.SetBool(isSwimmingHash, false);
            }
        } else { // not underwater
            if (isMovementPressed && !isRunning) {
                animator.SetBool(isRunningHash, true);
            }

            else if (!isMovementPressed && isRunning) {
                animator.SetBool(isRunningHash, false);
            }
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
            Debug.Log("OnAnimatorIK Running");
            Vector3 leftFootT = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
            Vector3 rightFootT = animator.GetIKPosition(AvatarIKGoal.RightFoot);
            if (leftFootT.x > rightFootT.x)
            {
                if (currentMovement.x > 0)
                {
                    animator.SetBool(leftLegFrontHash, false);
                }
                else
                {
                    animator.SetBool(leftLegFrontHash, true);
                }

            }
            else
            {
                if (currentMovement.x > 0)
                {
                    animator.SetBool(leftLegFrontHash, true);
                }
                else
                {
                    animator.SetBool(leftLegFrontHash, false);
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
        handleObstacles();
        handleLightBlockers();
        handleRotation();
        handleAnimation();

        // check if the player is rising above the water
        if (inWater && characterController.transform.position.y + currentMovement.y * Time.deltaTime * swimSpeed >= waterLevel) {
            currentMovement.y = 0;
        }

        //Debug.Log(currentMovement);
        if (isJumping) {
            Debug.Log("Player is JUMPING");
            characterController.Move(currentMovement * Time.deltaTime * jumpRunSpeed);
        } else if (inWater) {
            characterController.Move(currentMovement * Time.deltaTime * swimSpeed);
        } else if (animator.GetBool(isPushingHash)) {
            characterController.Move(currentMovement * Time.deltaTime * pushRunSpeed);
        } else
        {
            characterController.Move(currentMovement * Time.deltaTime * runSpeed);
        }
        if (!inWater)
        {
            handleGravity();
            handleJump();
        }
       // characterController.transform.position = new Vector3(-140.56f, transform.position.y, transform.position.z); // lock x axis
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
