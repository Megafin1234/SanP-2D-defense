using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject mainMenuPanel;  
    public GameObject tutorialPanel;  
    public Button storyButton;    
    public Button storyEndButton;  
    public Button goBackButton;    
     public GameObject nightPhaseText; 
    public GameObject dayPhaseText;    
    public Image fadeImage;     
    public CanvasGroup fadeCanvasGroup;   
    public AnimationCurve fadeCurve;

    void Awake()
    {
        instance = this;
        fadeCanvasGroup.alpha = 0;
    }

    public void GameStart()
    {
        storyButton.onClick.AddListener(ShowTutorial);
        storyEndButton.onClick.AddListener(TutorialEnd);
        goBackButton.onClick.AddListener(ShowMainMenu);
        nightPhaseText.gameObject.SetActive(false);
        dayPhaseText.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);  
    } 


    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        tutorialPanel.SetActive(false);
    }

    public void ShowTutorial()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
    }

    public void TutorialEnd()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    public void FadeOut(System.Action callback)
    {
        StartCoroutine(Fade(0, 1, 2f, callback)); 
    }

    public void FadeIn(System.Action callback)
    {
        StartCoroutine(Fade(1, 0, 2f, callback)); 
    }



    private System.Collections.IEnumerator Fade(float startAlpha, float endAlpha, float duration, System.Action callback)
    {
        float fadeTime = 0;

        while (fadeTime < duration)
        {
            fadeTime += Time.deltaTime;
            float t = fadeTime / duration;
            float curveValue = fadeCurve.Evaluate(t);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveValue); 
            yield return null;
        }

        fadeCanvasGroup.alpha = endAlpha; 

        if (endAlpha == 0)
        {
            fadeCanvasGroup.blocksRaycasts = true; 
        }
        else
        {
            fadeCanvasGroup.blocksRaycasts = false; 
        }

        callback?.Invoke(); 
    } 



    public void ShowDayPhaseText()
    {
        dayPhaseText.gameObject.SetActive(true);
        Invoke("HidePhaseText", 5f); 
    }

    public void ShowNightPhaseText()
    {
        nightPhaseText.gameObject.SetActive(true);
        Invoke("HidePhaseText", 5f);  
    }

    private void HidePhaseText()
    {
        dayPhaseText.SetActive(false);
        nightPhaseText.SetActive(false);
    }
}

