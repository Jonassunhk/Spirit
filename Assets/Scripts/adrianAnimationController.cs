using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adrianAnimationController : MonoBehaviour
{
    Vector3 floatPos = new Vector3(235.1f, -30.46f, -255.59f); // initial position
    Vector3 groundPos = new Vector3(239f, -54.96f, -255.59f);
    public CharacterController characterController;
    public Animator animator;

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("here");
        changePos(floatPos);
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void changePos(Vector3 pos)
    {
        characterController.enabled = false;
        characterController.transform.position = pos;
        characterController.enabled = true;
    }

    public void changeToGroundPos()
    {
        changePos(groundPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
