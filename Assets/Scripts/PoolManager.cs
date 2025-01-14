using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PoolManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator topWaterAnimator, bottomWaterAnimator, mechanismAnimator, underwaterGateAnimator, adrianAnimator, playerAnimator;
    public GameObject waterBox;
    bool interactable1 = false;
    bool pressed1 = false;
    bool interactable2 = false;
    bool pressed2 = false;
    bool interactable3 = false;
    bool pressed3 = false;
    public bool waterDescended = false;
    public CameraManager cameraManager;
    public UIManager uiManager;
    public AnimationMovementController playerController;
    public adrianAnimationController adrianAnimationController;
    public GameObject caustics;
    public GameObject poolSun;
    public GameObject adrianPointLight;
    public soundManager soundManager;


    void Start()
    {
        caustics.SetActive(true);
        poolSun.GetComponent<Light>().enabled = false;
        adrianPointLight.GetComponent<Light>().enabled = false;
        cameraManager.resetToDefaultPriority();
       // StartCoroutine(waterCutScene());
    }

    public void onPlayerInRange()
    {
        interactable1 = true;
    }

    public void onPlayerOutOfRange()
    {
        interactable1 = false;
    }

    public void onPlayerInRange2()
    {
        interactable2 = true;
    }

    public void onPlayerOutOfRange2()
    {
        interactable2 = false;
    }

    public void onPlayerInRange3()
    {
        interactable3 = true;
    }

    public void onPlayerOutOfRange3()
    {
        interactable3 = false;
    }

    IEnumerator waterCutScene()
    {
        waterDescended = true;
        // change caustics effect, add sun, and change adrian animation
        caustics.SetActive(false);
        poolSun.GetComponent<Light>().enabled = true;
        adrianPointLight.GetComponent<Light>().enabled = true;
        adrianAnimationController.changeToGroundPos();
        adrianAnimator.Play("StandingUp");
        adrianAnimator.speed = 0;
        cameraManager.setCameraPriority("Pool Camera", 10);
        yield return new WaitForSeconds(2);
        soundManager.FadeIn("waterLower", 5f, 0.25f);
        topWaterAnimator.SetTrigger("descend");
        bottomWaterAnimator.SetTrigger("descend");
        yield return new WaitForSeconds(5);
        soundManager.FadeOut("waterLower", 1f);
        yield return new WaitForSeconds(1f);
        waterBox.SetActive(false);
        playerController.exitWater();
        playerController.resetAnimation();
        cameraManager.setCameraPriority("Pool Camera", 3);
        cameraManager.setCameraPriority("Underwater Gate Camera", 30);
        yield return new WaitForSeconds(3);
        underwaterGateAnimator.SetTrigger("Activate");
        soundManager.PlaySound("metalDoorOpening");
        yield return new WaitForSeconds(5);
        cameraManager.setCameraPriority("AdrianSceneCamera", 40);
        yield return new WaitForSeconds(4);
        soundManager.FadeIn("endSceneMusic", 3f, 0.8f);
        cameraManager.resetToDefaultPriority();
        cameraManager.setCameraPriority("Final Scene Follow Camera", 6);
        yield return null;
    }

    IEnumerator finalCutScene()
    {
        playerController.canUpdate = false;
        cameraManager.setCameraPriority("AdrianSceneCamera", 60);
        playerAnimator.SetTrigger("Kneel");
        yield return new WaitForSeconds(8);
        soundManager.PlaySound("deathSound");
        playerController.dissolveAnimation.dissolve(false, 0.4f, 1f);
        yield return new WaitForSeconds(4);
        adrianAnimator.speed = 1;
        yield return new WaitForSeconds(15);
        StartCoroutine(uiManager.showEndStoryline());
        yield return new WaitForSeconds(20);
        yield return null;
    }


        IEnumerator mechanismCutScene()
    {
        cameraManager.setCameraPriority("Mechanism Camera", 10);
        yield return new WaitForSeconds(2);
        soundManager.PlaySound("machineSound");
        mechanismAnimator.SetBool("Activated", true);
        yield return new WaitForSeconds(4);
        cameraManager.setCameraPriority("Mechanism Camera", 3);
    }

        public void onButtonPress()
    {
        if (!pressed1 && interactable1)
        {
            pressed1 = true;
            soundManager.PlaySound("buttonClick");
            uiManager.removeStoryboard();
            StartCoroutine(waterCutScene());
        } else if (!pressed2 && interactable2)
        {
            pressed2 = true;
            soundManager.PlaySound("buttonClick");
            uiManager.removeStoryboard();
            StartCoroutine(mechanismCutScene());
        } else if (!pressed3 && interactable3 && waterDescended)
        {
            pressed3 = true;
            uiManager.removeStoryboard();
            StartCoroutine(finalCutScene());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
