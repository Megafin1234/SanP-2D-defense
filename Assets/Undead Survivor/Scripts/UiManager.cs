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

    void Awake()
    {
        instance = this;
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
    fadeImage.gameObject.SetActive(true);
    fadeImage.CrossFadeAlpha(1, 1f, false);
    StartCoroutine(FadeComplete(callback));
}

    public void FadeIn(System.Action callback)
{
    fadeImage.CrossFadeAlpha(0, 1f, false);
    StartCoroutine(FadeComplete(callback));
    fadeImage.gameObject.SetActive(false);
}

    private IEnumerator FadeComplete(System.Action callback)
{
    yield return new WaitForSeconds(1f); // 페이드 완료 대기
    callback?.Invoke(); // 콜백 실행
}


    private void OnFadeOutComplete()
    {
        // 페이드 아웃이 끝난 후 처리할 작업
    }


    private void OnFadeInComplete()
    {
        // 페이드 인이 끝난 후 처리할 작업


    }

    public void ShowDayPhaseText()
    {
        dayPhaseText.gameObject.SetActive(true);
        Invoke("HidePhaseText", 2f); 
    }

    public void ShowNightPhaseText()
    {
        nightPhaseText.gameObject.SetActive(true);
        Invoke("HidePhaseText", 2f);  
    }

    private void HidePhaseText()
    {
        dayPhaseText.SetActive(false);
        nightPhaseText.SetActive(false);
    }
}

