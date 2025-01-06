using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeAdjuster : MonoBehaviour
{

    public Slider volumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(delegate { onSliderChanged(); });
    }

    void onSliderChanged()
    {
        AudioListener.volume = volumeSlider.value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
