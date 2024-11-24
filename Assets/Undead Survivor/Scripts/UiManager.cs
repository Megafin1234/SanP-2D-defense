using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuPanel;  
    public GameObject tutorialPanel;  
    public Button storyButton;    
    public Button storyEndButton;  
    public Button goBackButton;      

    void Start()
    {
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
}
