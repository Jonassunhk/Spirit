using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas canvas;
    public GameObject gameTitlePage;
    public GameObject storyboardPage;
    public TextMeshProUGUI storyboardText;
    public GameObject pitchBlackPanel;
    public GameObject settingsMenu;
    public GameObject startGamePanel;
    public GameObject endGamePanel;
    public GameObject[] startGameStoryline;
    public GameObject[] endGameStoryline;
    bool textAnimating = false;

    public IEnumerator showStartStoryline() // total time: 13 seconds
    {
        StartCoroutine(FadeToOpaque(0f));
        Debug.Log("Screen should be blacked out");
        startGamePanel.SetActive(true);
        for (int i = 0; i < startGameStoryline.Length; i++) // preset
        {
            startGameStoryline[i].SetActive(true);
            TextMeshProUGUI text = startGameStoryline[i].GetComponent<TextMeshProUGUI>();
            Color color = text.color;
            color.a = 0;
            text.color = color;
        }
        for (int i = 0; i < startGameStoryline.Length; i++)
        {
            StartCoroutine(showTransitionText(startGameStoryline[i].GetComponent<TextMeshProUGUI>(), 1f));
            yield return new WaitForSeconds(3f);
        }
        for (int i = 0; i < startGameStoryline.Length; i++) // preset
        {
            startGameStoryline[i].SetActive(false);
        }
        StartCoroutine(FadeToTransparent(1f));
        yield return null;
    }

    public IEnumerator showEndStoryline() // total time: 15 seconds
    {
        StartCoroutine(FadeToOpaque(2f));
        yield return new WaitForSeconds(2f);
        endGamePanel.SetActive(true);
        for (int i = 0; i < endGameStoryline.Length; i++) // preset
        {
            endGameStoryline[i].SetActive(true);
            TextMeshProUGUI text = endGameStoryline[i].GetComponent<TextMeshProUGUI>();
            Color color = text.color;
            color.a = 0;
            text.color = color;
        }
        for (int i = 0; i < endGameStoryline.Length; i++)
        {
            StartCoroutine(showTransitionText(endGameStoryline[i].GetComponent<TextMeshProUGUI>(), 1f));
            yield return new WaitForSeconds(3f);
        }
       
        yield return null;
    }
    public IEnumerator FadeToOpaque(float duration)
    {
        Color color = pitchBlackPanel.GetComponent<Image>().color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            pitchBlackPanel.GetComponent<Image>().color = color;
            yield return null;
        }

        // Ensure the panel is fully opaque
        color.a = 1f;
        pitchBlackPanel.GetComponent<Image>().color = color;
    }

    public IEnumerator showTransitionText (TextMeshProUGUI text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        // Ensure the panel is fully opaque
        color.a = 1f;
        text.color = color;
    }

    // Fade panel from opaque to transparent
    public IEnumerator FadeToTransparent(float duration)
    {
        Color color = pitchBlackPanel.GetComponent<Image>().color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            pitchBlackPanel.GetComponent<Image>().color = color;
            yield return null;
        }

        // Ensure the panel is fully transparent
        color.a = 0f;
        pitchBlackPanel.GetComponent<Image>().color = color;
    }

    IEnumerator LerpIntensity(int a, int b, int x) // a = start, b = end, x = time frame
    {
        float n = 0;  // lerped value
        for (float f = 0; f <= x; f += Time.deltaTime)
        {
            n = Mathf.Lerp(a, b, f / x); // passing in the start + end values, and using our elapsed time 'f' as a portion of the total time 'x'
            yield return null;
        }
    }



    IEnumerator LerpText(TextMeshProUGUI gameObject, string targetText, float time)
    {
        
        string currentText = "";
        float timePerChar = time / targetText.Length;

        for (int i = 0; i < targetText.Length; i++)
        {
            currentText += targetText[i];
            gameObject.text = currentText;
            if (i % 2 == 0)
            {
                yield return null;
            }
            
        }
        textAnimating = false;
    }

    public void onButtonClick()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        storyboardPage.SetActive(false);
        gameTitlePage.SetActive(false);
        settingsMenu.SetActive(false);
        startGamePanel.SetActive(false);
        endGamePanel.SetActive(false);
    }

    public void ShowGameTitle()
    {
        gameTitlePage.SetActive(true);
    }

    public void RemoveGameTitle()
    {
        gameTitlePage.SetActive(false);
    }

    public void onSettingButtonClicked()
    {
        gameTitlePage.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void onQuitButtonPressed()
    {
        Application.Quit();
    }

    public void onBackButtonPressed()
    {
        gameTitlePage.SetActive(true);
        settingsMenu.SetActive(false);
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
            StartCoroutine(LerpText(storyboardText, text, 1)); // 2 seconds each text
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
