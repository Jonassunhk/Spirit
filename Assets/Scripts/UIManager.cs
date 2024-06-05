using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject gameTitlePage;
    public GameObject storyboardPage;
    public TextMeshProUGUI storyboardText;
    bool textAnimating = false;

    IEnumerator LerpIntensity(int a, int b, int x) // a = start, b = end, x = time frame
    {
        float n = 0;  // lerped value
        for (float f = 0; f <= x; f += Time.deltaTime)
        {
            n = Mathf.Lerp(a, b, f / x); // passing in the start + end values, and using our elapsed time 'f' as a portion of the total time 'x'
            yield return null;
        }
    }

    IEnumerator LerpStoryboardText(string targetText, float time)
    {
        
        string currentText = "";
        float timePerChar = time / targetText.Length;

        for (int i = 0; i < targetText.Length; i++)
        {
            currentText += targetText[i];
            storyboardText.text = currentText;
            if (i % 2 == 0)
            {
                yield return null;
            }
            
        }
        textAnimating = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        storyboardPage.SetActive(false);
        gameTitlePage.SetActive(false);
    }

    public void ShowGameTitle()
    {
        gameTitlePage.SetActive(true);
    }

    public void RemoveGameTitle()
    {
        gameTitlePage.SetActive(false);
    }

    public void removeStoryboard()
    {
        storyboardPage.SetActive(false);
    }
    
    public void showStoryboard(string text)
    {
        storyboardPage.SetActive(true);
        if (!textAnimating)
        {
            textAnimating = true;
            StartCoroutine(LerpStoryboardText(text, 1)); // 2 seconds each text
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
