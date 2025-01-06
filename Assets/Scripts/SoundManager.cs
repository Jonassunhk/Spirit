using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundManager : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {

    }

    public void PlaySound(string name)
    {
        GameObject sound = transform.Find(name).gameObject;
        if (sound != null )
        {
            sound.GetComponent<AudioSource>().Play();
        } else
        {
            Debug.Log("Sound doesn't exist");
        }
    }

    public void unmuteSound(string name)
    {
        GameObject sound = transform.Find(name).gameObject;
        if (sound != null)
        {
            sound.GetComponent<AudioSource>().mute = false;
        }
        else
        {
            Debug.Log("Sound doesn't exist");
        }
    }

    public void muteSound(string name)
    {
        GameObject sound = transform.Find(name).gameObject;
        if (sound != null)
        {
            sound.GetComponent<AudioSource>().mute = true;
        }
        else
        {
            Debug.Log("Sound doesn't exist");
        }
    }

    public void StopSound(string name)
    {
        GameObject sound = transform.Find(name).gameObject;
        if (sound != null)
        {
            sound.GetComponent<AudioSource>().Stop();
        }
        else
        {
            Debug.Log("Sound doesn't exist");
        }
    }


    public void CrossfadeTo(string source1, string source2, float fadeDuration)
    {
        GameObject sound = transform.Find(source1).gameObject;
        GameObject sound2 = transform.Find(source2).gameObject;
        StartCoroutine(Crossfade(sound.GetComponent<AudioSource>(), sound2.GetComponent<AudioSource>(),  fadeDuration));
    }

    private IEnumerator Crossfade(AudioSource currentSource, AudioSource nextSource, float fadeDuration)
    {
        nextSource.Play();

        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            currentSource.volume = Mathf.Lerp(1, 0, t);
            nextSource.volume = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 1;
        nextSource.volume = 1;

        // Swap sources
        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }
    

    // Call this function to start fading in the audio
    public void FadeIn(string name, float fadeDuration, float finalVolume)
    {
        AudioSource sound = transform.Find(name).gameObject.GetComponent<AudioSource>();
        StartCoroutine(FadeInCoroutine(sound, fadeDuration, finalVolume));
    }

    // Call this function to start fading out the audio
    public void FadeOut(string name, float fadeDuration)
    {
        AudioSource sound = transform.Find(name).gameObject.GetComponent<AudioSource>();
        StartCoroutine(FadeOutCoroutine(sound, fadeDuration));
    }

    private IEnumerator FadeInCoroutine(AudioSource audioSource, float fadeDuration, float finalVolume)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < finalVolume)
        {
            audioSource.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.volume = finalVolume; // Ensure volume is set to max at the end
    }

    private IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0.0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume to original level
    }


}
