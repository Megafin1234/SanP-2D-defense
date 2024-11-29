using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject mainMenuPanel;  
    public GameObject tutorialPanel;  
    public Button storyButton;    
    public Button storyEndButton;  
    public Button goBackButton;    
     public GameObject nightPhaseText;  // 밤 페이즈 텍스트 UI
    public GameObject dayPhaseText;    // 낮 페이즈 텍스트 UI
    public Image fadeImage;            // 페이드 아웃/인 이미지  

    void Start()
    {
        nightPhaseText.SetActive(false);
        dayPhaseText.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        ShowMainMenu(); 
        storyButton.onClick.AddListener(ShowTutorial);
        storyEndButton.onClick.AddListener(TutorialEnd);
        goBackButton.onClick.AddListener(ShowMainMenu);
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
        Invoke("OnFadeOutComplete", 1f);
    }

    public void FadeIn()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.CrossFadeAlpha(0, 1f, false); 
        Invoke("OnFadeInComplete", 1f);
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

