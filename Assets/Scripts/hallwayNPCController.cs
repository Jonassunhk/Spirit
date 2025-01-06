using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class hallwayNPCController : MonoBehaviour
{
    public GameObject hallwayNPCs;
    public bool moveCharacters; // default: true
    public float NPCMoveSpeed; // default: 3f
    public float turnPosition; // default: -266f
    public float rotationFactorPerFrame;

    // spawning

    public GameObject sampleHallwayNPC;
    public bool spawnCharacters; // default: true
    public float NPCDistance; // default: 5f;
   

    // Start is called before the first frame update
    void Start()
    {
        sampleHallwayNPC.SetActive(false);
        updateAllAnimations();
    }

    public void stopMovement()
    {
        moveCharacters = false;
        updateAllAnimations();
    }

    public void beginMovement()
    {
        moveCharacters = true;
        updateAllAnimations();
    }

    public void beginmoving() // prompt the characters to start moving
    {
        foreach (Transform NPC in hallwayNPCs.transform) { 
            CharacterController controller = NPC.GetComponent<CharacterController>();
            if (controller != null)
            {
                
                if (controller.transform.position.z >= -170f) // time to delete the character
                {
                    Debug.Log("Character Destroyed");
                    Destroy(NPC.gameObject);
                } else
                {

                    Vector3 MoveDirection;
                    if (controller.transform.position.x > turnPosition) // time to turn
                    {
                        MoveDirection = new Vector3(0, 0, 1);
                    }
                    else // not yet
                    {
                        MoveDirection = new Vector3(1, 0, 0);
                    }
                    controller.Move(MoveDirection * NPCMoveSpeed * Time.deltaTime);
                    handleRotation(MoveDirection, NPC);
                }
               
            }
        }
    }

    void handleRotation(Vector3 currentMovement, Transform NPCTransform)
    {
        Vector3 positionToLookAt;
        float rotationFactor;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.z = currentMovement.z;
        positionToLookAt.y = 0.0f;
        rotationFactor = rotationFactorPerFrame;

        Quaternion currentRotation = NPCTransform.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
        NPCTransform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactor * Time.deltaTime);
    }



    private void checkNPCSpawn() // spawn NPCs
    {
        float minX = 10000f;
        foreach (Transform NPC in hallwayNPCs.transform)
        {
            float dis = Mathf.Abs(sampleHallwayNPC.transform.position.x - NPC.transform.position.x);
            minX = Mathf.Min(dis, minX);
        }
        
        if (minX > NPCDistance) // clone another one
        {
            Debug.Log("cloned");
            GameObject newNPC = GameObject.Instantiate(sampleHallwayNPC);
            newNPC.transform.SetParent(hallwayNPCs.transform);
            newNPC.SetActive(true);
            updateCharacterAnimation(newNPC.gameObject);
        }
    }

    private void updateAllAnimations() // update walking/idle animations
    {
        foreach (Transform NPC in hallwayNPCs.transform)
        {
            updateCharacterAnimation(NPC.gameObject);
        }
    }

    private void updateCharacterAnimation(GameObject NPC)
    {
        Animator animator = NPC.GetComponent<Animator>();
        if (moveCharacters)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (moveCharacters) beginmoving();
        if (spawnCharacters) checkNPCSpawn();
    }
}
