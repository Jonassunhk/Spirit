using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class startGameManager : MonoBehaviour
{

    public GameObject adrianCharacter;
    public UIManager uiManager;
    public CameraManager cameraManager;
    public DissolveAnimation dissolveAnimation;
    public GameObject lightBeam;
    public GameObject spotLight;
    public AnimationMovementController playerController;
    public GameObject player;

    private bool isListening = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator startGameStartScene()
    {

        StartCoroutine(uiManager.FadeToOpaque(0f));
        yield return new WaitForSeconds(2);
        lightBeam.GetComponent<VolumetricLightBeam>().enabled = false;
        spotLight.GetComponent<Light>().enabled = false;
        uiManager.ShowGameTitle();
        playerController.canUpdate = false;
        player.SetActive(false);
        StartCoroutine(uiManager.FadeToTransparent(1f));
        yield return new WaitForSeconds(2);
        isListening = true;
    }

    IEnumerator startCutScene()
    {
        uiManager.RemoveGameTitle();
        StartCoroutine(uiManager.showStartStoryline());
        yield return new WaitForSeconds(13);
        lightBeam.GetComponent<VolumetricLightBeam>().enabled = true;
        spotLight.GetComponent<Light>().enabled = true;
        adrianCharacter.GetComponent<Animator>().Play("FallingFlat");
        yield return new WaitForSeconds(2);
        player.SetActive(true);
        dissolveAnimation.dissolve(true, 1f, 0.4f);
        yield return new WaitForSeconds(4);
        playerController.canUpdate = true;
        uiManager.showStoryboard("WASD to move and SPACE to jump");
        yield return new WaitForSeconds(10);
        uiManager.removeStoryboard();
    }

    public void onButtonPressed()
    {
        Debug.Log("Button Pressed");
        StartCoroutine(startCutScene());
    }

        // Update is called once per frame
}
