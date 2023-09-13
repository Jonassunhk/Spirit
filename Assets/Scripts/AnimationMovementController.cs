using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationMovementController : MonoBehaviour
{
    PlayerInput playerInput;
    CharacterController characterController;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;

    private void Awake()
    {
        Debug.Log("here");
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        playerInput.CharacterControls.Move.started += context => {

            Debug.Log("Key Pressed");
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;

        };

    }

    // Update is called once per frame
    void Update()
    {
        characterController.Move(currentMovement);
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
