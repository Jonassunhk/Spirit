using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class lightFlashScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectorScreen;
    float elapsed = 0f;
    public float period = 10f;
    public bool lightOn = false;
    Material[] mats;


    void Start()
    {
        mats = projectorScreen.GetComponent<Renderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime; 
        if (elapsed >= period)
        {
            elapsed = elapsed % period;
            if (lightOn)
            {
                lightOn = false;
                gameObject.GetComponent<Light>().enabled = false;
                mats[0].SetColor("_EmissionColor", Color.white);
            } else
            {
                lightOn = true;
                gameObject.GetComponent<Light>().enabled = true;
                mats[0].SetColor("_EmissionColor", new Color(255,21,22));
            }
            
        }
    }
}
