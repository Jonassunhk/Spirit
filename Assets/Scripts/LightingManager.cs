using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    // Start is called before the first frame update
    ColorAdjustments colorAdjustments;
    ColorParameter color;

    private IEnumerator LerpColorsOverTime(Color startingColor, Color endingColor, float waitTime)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            color = new ColorParameter(Color.Lerp(startingColor, endingColor, (elapsedTime / waitTime)));
            colorAdjustments.colorFilter.SetValue(color);
            yield return null;
        }
    }

    IEnumerator LerpIntensity(int a,int b,int x) // a = start, b = end, x = time frame
    {
        float n = 0;  // lerped value
        for (float f = 0; f <= x; f += Time.deltaTime)
        {
            n = Mathf.Lerp(a, b, f / x); // passing in the start + end values, and using our elapsed time 'f' as a portion of the total time 'x'
            yield return null;
        }
    }

    private void Start()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out colorAdjustments);
       
    }

    public void onDeath()
    {
        Debug.Log("State switched to player death");
        StartCoroutine(LerpColorsOverTime(Color.white, Color.black, 3f));
        
    }

    public void onRespawn()
    {
        Debug.Log("State switched to player death");
        StartCoroutine(LerpColorsOverTime(Color.black, Color.white, 2f));

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
