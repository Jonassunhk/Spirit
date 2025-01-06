using Cinemachine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class AnimationMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;
    public UIManager UIManager;
    public PoolManager poolManager;
    public CameraManager cameraManager;
    public soundManager soundManager;
    public DissolveAnimation dissolveAnimation;

    int isRunningHash, isJumpingHash, inWaterHash, isSwimmingHash, leftLegFrontHash, isPushingHash, isDeadHash, respawnHash, poolClimbHash, isFallingHash;
    Quaternion targetRotation;

    public GameObject basketball_cage;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    public bool canUpdate = true;
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
    private float previousYVelocity = 0f;

    //swimming variables
    float waterRotationFactorPerFrame = 1.0f;
    float swimSpeed = 4.5f;
    //bool isSwimming = false;
    float waterLevel = -19f;
    bool inWater = false;
    float waterReducer = 0.05f;
    bool diveTransitioning = false;

    // obstacle detection
    float detectDistance = 2f;
    float pushStrength = 0.5f;
    float pushRunSpeed = 3f;

    // light detection
    public GameManager gameManager;
    public lightFlashScript lightFlash;
    float blockDetectDistance = 20f;

    // pool edge 
    bool atPoolEdge = false;
    bool poolTransitioning = false;
    System.DateTime startTime;
    float transitionPeriod = 0.8f;
    float speedX = -3f;
    float speedY = 3f;


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
        poolClimbHash = Animator.StringToHash("poolClimb");
        isFallingHash = Animator.StringToHash("isFalling");

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;
        playerInput.CharacterControls.Interact.started += onInteract;

        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;

        targetRotation = Quaternion.Euler(0, 0, 0);
        setupJumpVariables();
    }

    public void respawn(GameState gameState)
    {
        Vector3 pos = new Vector3(0,0,0);
        

        if (gameState == GameState.Hallway || gameState == GameState.MainScreen) { 
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

        // resetting
        characterController.enabled = false;
        characterController.transform.position = pos;
        characterController.enabled = true;
        inWater = false;
        currentMovement = Vector3.zero;
        animator.SetTrigger(respawnHash);
        cameraManager.resetToDefaultPriority();
        resetParameters();
        animator.Play("Idle");
        canUpdate = true;
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

    void onJump(InputAction.CallbackContext context) // jump button pressed
    {
        isJumpPressed = context.ReadValueAsButton();
        if (!diveTransitioning && isJumpPressed && inWater && atPoolEdge && !animator.GetBool(isSwimmingHash)) // pool edge movement
        {
            animator.SetBool(poolClimbHash, true); // trigger animation
            startTime = System.DateTime.UtcNow;
            poolTransitioning = true;
            // animator.SetBool(inWaterHash, false);
        }
    }

    void onMovementInput (InputAction.CallbackContext context)
    {
        if (!diveTransitioning && !poolTransitioning)
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            if (inWater)
            {
                currentMovement.y = currentMovementInput.y;
                isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
            } else
            {
                isMovementPressed = currentMovementInput.x != 0;
            }
        }
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        
        
        if (characterController.isGrounded) {
           
            if (previousYVelocity < -1f)
            {
                soundManager.PlaySound("landing");
            }
            previousYVelocity = currentMovement.y;

            if (isJumpAnimating) {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
                soundManager.PlaySound("landing");
            }
            currentMovement.y = groundedGravity;
        } else if (isFalling) {
            
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelocity;

        } else { // caculating the change in y position for the next frame
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime); // vf = vi + at (equation of motion)
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f; //  delta x = 0.5 * (vi + vf) (equation of motion)
            currentMovement.y = nextYVelocity;
        }

        if (currentMovement.y < -2.5f) {
            animator.SetBool(isFallingHash, true);
        } else {
            animator.SetBool(isFallingHash, false);
        }
    }

    
    public void resetParameters() {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }
    }

    public void resetAnimation()
    {
        animator.Play("Idle");
    }
    public void exitWater()
    {
        Debug.Log("Player exiting water!");
        resetParameters();
        inWater = false;
        animator.SetBool(inWaterHash, false);
        soundManager.PlaySound("exitWater");
        soundManager.FadeOut("swimmingPoolBackgroundMusic", 1f);
        soundManager.FadeOut("underwaterAmbience", 1f);
    }
    private void OnTriggerEnter(Collider other) // collided with something
    {
        Debug.Log("Collided");
        if (other.CompareTag("Water")) // if hit water
        {
            resetParameters();
            animator.SetBool(inWaterHash, true);
            //animator.SetBool(isSwimmingHash, true);
            Debug.Log("Collided with Water! Current state: " + animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
            
            inWater = true;
            isJumping = false;
            diveTransitioning = true;
            currentMovement.x = 0;
            currentMovement.y = Math.Min(-7f, currentMovement.y);

            // sounds
            soundManager.PlaySound("waterSplash");
            soundManager.PlaySound("underwaterAmbience");
            soundManager.FadeIn("swimmingPoolBackgroundMusic", 7f, 0.3f);

            Debug.Log("Collided with Water! Current y movement: " + currentMovement.y);
        } else if (other.CompareTag("Death Block"))
        {
            canUpdate = false;
            gameManager.UpdateGameState(GameState.PlayerDeath);

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
                    throw new ArgumentOutOfRangeException("Name", other.name, null);

            }
        } else if (other.CompareTag("Dialogue")) // hit a dialogue block
        {
            int number = int.Parse(other.name);
            //gameManager.updateDialogue(number);
        } else if (other.CompareTag("Pool Edge")) // hit pool edge while in water
        {
            Debug.Log("Collided with pool edge");
            if (other.transform.name == "PoolEdgeFront") // going front
            {
                speedX = -5f;
            } else if (other.transform.name == "PoolEdgeBack") // going back
            {
                speedX = 5f;
            }
            atPoolEdge = true;

        } else if (other.CompareTag("Underwater Button")) // in range of button to lower water level
        {
            UIManager.showStoryboard("Press E to interact");
            poolManager.onPlayerInRange();
        } else if (other.CompareTag("Pool Button")) // player pressed pool mechanism button
        {
            UIManager.showStoryboard("Press E to interact");
            poolManager.onPlayerInRange2();
        } else if (other.CompareTag("Save Adrian Button") && poolManager.waterDescended)
        {
            UIManager.showStoryboard("Press E to combine");
            poolManager.onPlayerInRange3();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collision exit");
        if (other.CompareTag("Water"))
        {
            exitWater();
            cameraManager.resetToDefaultPriority();
            Debug.Log("Collision exited with Water!");
        } else if (other.CompareTag("Pool Edge"))
        {
            Debug.Log("Collision exited with pool edge");
            atPoolEdge = false;
        } else if (other.CompareTag("Underwater Button"))
        {
            UIManager.storyboardPage.SetActive(false);
            poolManager.onPlayerOutOfRange();
        } else if (other.CompareTag("Pool Button"))
        {
            UIManager.storyboardPage.SetActive(false);
            poolManager.onPlayerOutOfRange2();
        }
    }

    void onInteract(InputAction.CallbackContext context)
    {
        poolManager.onButtonPress();
    }

    void handleObstacle()
    {
        RaycastHit hit;
        animator.SetBool(isPushingHash, false);
        soundManager.muteSound("BasketballCage");
        if (Physics.Raycast(characterController.transform.position, characterController.transform.forward, out hit, detectDistance)) { 
            if (hit.collider.tag == "Obstacle")
            {
                Debug.Log("Obstacle in range");
                bool isRunning = animator.GetBool(isRunningHash);
                if (isRunning)
                {
                    animator.SetBool(isPushingHash, true);
                    Rigidbody body = hit.collider.attachedRigidbody;
                    if (hit.transform.name == "basketball_cage")
                    {
                        if (!hit.transform.gameObject.GetComponent<basketballCage>().positionFixed) {
                            soundManager.unmuteSound("BasketballCage");
                 
                        }
                    }
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
            canUpdate = false;
            soundManager.PlaySound("deathSound");
            soundManager.StopSound("underwaterAmbience");
            soundManager.FadeOut("swimmingPoolBackgroundMusic", 2f);
            Debug.Log("death animation played");
            if (!inWater)
            {
                animator.SetTrigger(isDeadHash);

            }
            dissolveAnimation.dissolve(true, 0.4f, 1f);
        }
    }

    void handleRotation()
    {
        Vector3 positionToLookAt;
        float rotationFactor;
        
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.z = 0; // currentMovement.z;

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

    void setDefaultCamera()
    {
        if (characterController.transform.position.x >= -18.76 && characterController.transform.position.x <= 38.25)
        {
            cameraManager.setCameraPriority("classroomCamera", 100);
        } else
        {
            cameraManager.setCameraPriority("classroomCamera", 0);
        }
        if (inWater)
        {
            if (characterController.transform.position.x < 105)
            {
                cameraManager.setCameraPriority("Tunnel Camera", 6);
            }
            else if (characterController.transform.position.x > 105 && characterController.transform.position.x < 150)
            {
                cameraManager.setCameraPriority("Tunnel Camera (2)", 6);
            }
            else
            {
                cameraManager.setCameraPriority("Underwater Camera", 6);
            }
        }
    }
    void checkTransitioning()
    {
        if (poolTransitioning)
        {
            System.TimeSpan ts = System.DateTime.UtcNow - startTime;
            if (ts.Seconds < transitionPeriod)
            {
                Debug.Log("pool transitioning");
                currentMovement.y = speedY;
                currentMovement.x = speedX;
                characterController.Move(currentMovement * Time.deltaTime);
            }
            else
            {
                currentMovement.y = 0;
                currentMovement.x = 0;
                animator.SetBool(poolClimbHash, false);
                poolTransitioning = false;
            }
            return;
        }
        if (diveTransitioning)
        {
            currentMovement.y += waterReducer;
            characterController.Move(currentMovement * Time.deltaTime);
            if (currentMovement.y > -0.03)
            {
                currentMovement.y = 0;
                diveTransitioning = false;
            }
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

        setDefaultCamera();
        if (!canUpdate) { return; }
        handleObstacle();
        handleLightBlockers();
        handleRotation();

        // check if the player is rising above the water
        if (inWater && !diveTransitioning && characterController.transform.position.y + currentMovement.y * Time.deltaTime * swimSpeed >= waterLevel) {
            currentMovement.y = 0;
        }

        if (diveTransitioning || poolTransitioning)
        {
            checkTransitioning();
            return;
        }

        handleAnimation();

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
        
        if (!inWater){
            handleGravity();
            handleJump();
        }

        // auto z adjustment
        Vector3 zAdjustment = new Vector3 (0, 0, -254.5f - characterController.transform.position.z);
        characterController.Move(zAdjustment);
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
