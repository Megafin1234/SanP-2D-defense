using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;  
    public GameObject tutorialPanel;  

    [Header("Buttons")]
    public Button storyButton;        // 새 게임 (스토리 컷씬 시작)
    public Button storyEndButton;     // 튜토리얼 스킵 → 본편
    public Button goBackButton;       // 뒤로가기
    public Button nextCutsceneButton; // 컷씬 넘기기
    public Button tutorialButton;     // 튜토리얼 보기

    [Header("Cutscene UI")]
    public Image cutsceneImage;       
    public Text cutsceneText;         
    public Image cutsceneFadeImage;  

    [System.Serializable]
    public class StoryCutscene {
        public Sprite cutsceneImage;
        [TextArea(3, 5)]
        public string cutsceneText;
    }
    public StoryCutscene[] cutscenes;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private float typeSpeed = 0.1f; // 타자기 효과 속도 (고정)

    [Header("Effects")]
    public GameObject nightPhaseText; 
    public GameObject dayPhaseText;    
    public Image fadeImage;     
    public CanvasGroup fadeCanvasGroup;   
    public AnimationCurve fadeCurve;
    public Image nightEffect;
    public GameObject clickEffectA;
    public GameObject clickEffectB;
    public GameObject clickEffectC;
    public GameObject mapChoicePanel;          
    public Button[] mapChoiceButtons;       

    void Awake()
    {
        instance = this;
        fadeCanvasGroup.alpha = 0;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("CameFromTutorial", 0) == 1)
        {
            mainMenuPanel.SetActive(false); // 메인메뉴 자동 숨김
            PlayerPrefs.DeleteKey("CameFromTutorial"); // 플래그 제거
        }

        // 버튼 이벤트 연결
        storyButton.onClick.AddListener(ShowTutorial);
        storyEndButton.onClick.AddListener(GoToMainScene);
        goBackButton.onClick.AddListener(ShowMainMenu);
        nextCutsceneButton.onClick.AddListener(NextCutscene);
        tutorialButton.onClick.AddListener(GoToTutorialScene);


        nextCutsceneButton.gameObject.SetActive(false);
        storyEndButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);

        mapSpriteDict = new Dictionary<MapType, Sprite>();
        foreach (var data in mapSprites)
        {
            mapSpriteDict[data.mapType] = data.mapSprite;
        }
    }



    public void ShowTutorial()
    {
        clickEffectA.SetActive(true);
        StartCoroutine(TutDelay());
    }

    private IEnumerator TutDelay()
    {
        yield return new WaitForSeconds(0.5f); 
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);
        clickEffectA.SetActive(false);

        currentIndex = 0;
        ShowCutscene(currentIndex);
    }

    private void ShowCutscene(int index)
    {
        if (cutscenes.Length == 0) return;

        cutsceneImage.sprite = cutscenes[index].cutsceneImage;

        StopTyping();
        typingCoroutine = StartCoroutine(TypewriterEffect(cutscenes[index].cutsceneText));

        nextCutsceneButton.gameObject.SetActive(true);
        storyEndButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
    }

    private void NextCutscene()
    {
        currentIndex++;
        if (currentIndex >= cutscenes.Length)
        {
            nextCutsceneButton.gameObject.SetActive(false);
            storyEndButton.gameObject.SetActive(true);
            tutorialButton.gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(FadeTransition(1f, () => {
                cutsceneImage.sprite = cutscenes[currentIndex].cutsceneImage;
                StopTyping();
                typingCoroutine = StartCoroutine(TypewriterEffect(cutscenes[currentIndex].cutsceneText));
            }));
        }
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    IEnumerator TypewriterEffect(string fullText)
    {
        cutsceneText.text = "";
        foreach (char c in fullText)
        {
            cutsceneText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }


    IEnumerator FadeTransition(float duration, System.Action midAction)
    {
        // 1) 페이드 아웃 (0 → 1)
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, t / duration);
            cutsceneFadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        // 2) 중간 작업 (이미지/텍스트 교체)
        midAction?.Invoke();

        // 3) 페이드 인 (1 → 0)
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / duration);
            cutsceneFadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }



    public void GoToTutorialScene()
    {
        SceneManager.LoadScene("Tutorial");
        clickEffectC.SetActive(true);
        StartCoroutine(EndDelay());
    }

    public void GoToMainScene()
    {
        PlayerPrefs.SetInt("CameFromTutorial", 1); // 튜토리얼에서 돌아옴 표시
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
        clickEffectC.SetActive(true);
        StartCoroutine(EndDelay());
    }

    private IEnumerator EndDelay()
    {
        yield return new WaitForSeconds(0.5f); 
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        clickEffectC.SetActive(false);
    }

    public void ShowMainMenu()
    {
        clickEffectB.SetActive(true);
        StartCoroutine(BackDelay());
    }

    private IEnumerator BackDelay(){
        yield return new WaitForSeconds(0.5f); 
        mainMenuPanel.SetActive(true);
        tutorialPanel.SetActive(false);
        clickEffectB.SetActive(false);
    }


    public void FadeOut(System.Action callback)
    {
        StartCoroutine(Fade(0, 1, 4f, callback)); 
    }

    public void FadeIn(System.Action callback)
    {
        StartCoroutine(Fade(1, 0, 1f, callback)); 
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration, System.Action callback)
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
    public void HidePhaseText()
    {
        nightPhaseText.gameObject.SetActive(false);  
        dayPhaseText.gameObject.SetActive(false);
    }

    public void NightEffect()
    {
        nightEffect.gameObject.SetActive(true);
    }
    public void DayEffect()
    {
        nightEffect.gameObject.SetActive(false);
    }

    public void ShowMapChoices(List<MapType> mapOptions){
        mapChoicePanel.SetActive(true);

        for (int i = 0; i < mapChoiceButtons.Length; i++)
        {
            var mapType = mapOptions[i];
            mapChoiceButtons[i].GetComponentInChildren<Text>().text = GetDisplayName(mapType);
            Image mapImage = mapChoiceButtons[i].transform.Find("MapImage").GetComponent<Image>();
            if (mapSpriteDict.TryGetValue(mapType, out Sprite sprite))
            {
                mapImage.sprite = sprite;
                mapImage.enabled = true;
            }
            else
            {
                mapImage.enabled = false;
            }

            mapChoiceButtons[i].onClick.RemoveAllListeners();
            MapType localType = mapType; 
            mapChoiceButtons[i].onClick.AddListener(() => {
                OnMapSelected(localType);
            });
        }
    }

    private string GetDisplayName(MapType type){
        switch (type)
        {
            case MapType.심연의정원:
                return "심연의 정원";
            case MapType.수정해변:
                return "수정 해변";
            default:
                return type.ToString();
        }
    }

    void OnMapSelected(MapType selected)
    {
        mapChoicePanel.SetActive(false);

        if (Reposition.instance != null)
            Reposition.instance.SetCurrentMap(selected);
        GameManager.instance.Resume();
        GameManager.instance.StartDayPhase();
    }

    [System.Serializable]
    public class MapSpriteData{
        public MapType mapType;
        public Sprite mapSprite;
    }

    public MapSpriteData[] mapSprites; 
    private Dictionary<MapType, Sprite> mapSpriteDict;
}
