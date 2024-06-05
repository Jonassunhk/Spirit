using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public CinemachineVirtualCamera[] gameCameras;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setCameraPriority(string cameraName, int priority)
    {
        for (int i = 0; i <  gameCameras.Length; i++) { 
            if (gameCameras[i].name == cameraName)
            {
                Debug.Log("Setting " + cameraName + " to priority " + priority);
                gameCameras[i].Priority = priority;
                break;
            }
        }
    }

    public void resetToDefaultPriority()
    {
        for (int i = 0; i < gameCameras.Length; i++)
        {
            gameCameras[i].Priority = 3;
            if (gameCameras[i].Name == "Follow Camera")
            {
                gameCameras[i].Priority = 5;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
