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
    public GameObject monsterDexPanel;

    [Header("Buttons")]
    public Button storyButton;        // 새 게임 (스토리 컷씬 시작)
    public Button storyEndButton;     // 튜토리얼 스킵 → 본편(GoToMainScene 연결됨)
    public Button goBackButton;       // 뒤로가기
    public Button nextCutsceneButton; // 컷씬 넘기기
    public Button tutorialButton;     // 튜토리얼 보기

    // ▼ (추가) 선택지 버튼 (없어도 동작하도록 널가드/폴백 처리)
    public Button choiceAButton;
    public Button choiceBButton;

    [Header("Cutscene UI")]
    public Image cutsceneImage;
    public Text cutsceneText;
    public Image cutsceneFadeImage;

    [System.Serializable]
    public class StoryCutscene {
        public Sprite cutsceneImage;
        [TextArea(3, 5)] public string cutsceneText;
        public bool isChoiceStep; // ▼ (추가) 이 컷씬에서 선택지를 노출할지
    }

    [Header("Cutscenes")]
    public StoryCutscene[] cutscenes;          // 기존 컷씬 배열
    // ▼ (추가) 선택 분기용 배열들 (없어도 동작하도록 폴백 처리)
    public StoryCutscene[] branchCutscenesA;
    public StoryCutscene[] branchCutscenesB;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private float typeSpeed = 0.07f; // 타자기 효과 속도 (고정)

    // ▼ (추가) 현재 재생 중인 컷씬 배열(초기엔 cutscenes를 사용)
    private StoryCutscene[] currentCutscenes;

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
            mainMenuPanel.SetActive(false); // 메인메뉴 자동 숨김(기존 로직 유지)
            PlayerPrefs.DeleteKey("CameFromTutorial");
        }

        // 버튼 이벤트 연결 (기존 유지)
        storyButton.onClick.AddListener(ShowTutorial);
        storyEndButton.onClick.AddListener(GoToMainScene);
        goBackButton.onClick.AddListener(ShowMainMenu);
        nextCutsceneButton.onClick.AddListener(NextCutscene);

        nextCutsceneButton.gameObject.SetActive(false);
        storyEndButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);

        if (choiceAButton) choiceAButton.gameObject.SetActive(false);
        if (choiceBButton) choiceBButton.gameObject.SetActive(false);

        mapSpriteDict = new Dictionary<MapType, Sprite>();
        foreach (var data in mapSprites)
            mapSpriteDict[data.mapType] = data.mapSprite;
    }

    //튜토리얼 종료 후: 씬 전환 대신 컷씬 이어붙이기 시작
    public void StartPostTutorialCutscene()
    {
        mainMenuPanel.SetActive(false);
        tutorialPanel.SetActive(true);

        currentCutscenes = cutscenes; // 기존 배열 재사용
        currentIndex = 0;

        ShowCutscene(currentIndex);
    }

    // 기존: 메인에서 스토리 시작
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

        currentCutscenes = cutscenes; // 초기에는 기본 배열을 재생
        currentIndex = 0;
        ShowCutscene(currentIndex);
    }

    private void ShowCutscene(int index)
    {
        Debug.Log($"[UIManager] ShowCutscene index={index}");
        // 안전장치: currentCutscenes가 비어있으면 cutscenes로 폴백
        var list = (currentCutscenes != null && currentCutscenes.Length > 0) ? currentCutscenes : cutscenes;
        if (list == null || list.Length == 0)
        {
            Debug.LogWarning("[UIManager] currentCutscenes is empty!");
            return;
        }

        var sc = list[index];

        // 이미지 + 텍스트를 항상 동시에 세팅 (원래 동작 복원)
        cutsceneImage.sprite = sc.cutsceneImage;

        StopTyping();
        typingCoroutine = StartCoroutine(TypewriterEffect(sc.cutsceneText));

        // 선택 컷씬이면 선택 버튼을, 아니면 Next 버튼을
        bool canShowChoices = sc.isChoiceStep && choiceAButton != null && choiceBButton != null;

        if (canShowChoices)
        {
            nextCutsceneButton.gameObject.SetActive(false);

            choiceAButton.gameObject.SetActive(true);
            choiceBButton.gameObject.SetActive(true);

            choiceAButton.onClick.RemoveAllListeners();
            choiceBButton.onClick.RemoveAllListeners();

            choiceAButton.onClick.AddListener(() => OnChoiceSelected(true));
            choiceBButton.onClick.AddListener(() => OnChoiceSelected(false));
        }
        else
        {
            // 버튼 널이면 자동 폴백: Next로 진행
            nextCutsceneButton.gameObject.SetActive(true);
            if (storyEndButton) storyEndButton.gameObject.SetActive(false);
            if (tutorialButton) tutorialButton.gameObject.SetActive(false);

            if (choiceAButton) choiceAButton.gameObject.SetActive(false);
            if (choiceBButton) choiceBButton.gameObject.SetActive(false);
        }
    }

    private void NextCutscene()
    {
        var list = (currentCutscenes != null && currentCutscenes.Length > 0) ? currentCutscenes : cutscenes;

        currentIndex++;
        if (list == null || currentIndex >= list.Length)
        {
            // 끝: 기존 종료(본편) / 튜토 버튼 노출 (원래 동작 유지)
            nextCutsceneButton.gameObject.SetActive(false);
            if (storyEndButton) storyEndButton.gameObject.SetActive(true);
            if (tutorialButton) tutorialButton.gameObject.SetActive(true);
        }
        else
        {
            // 페이드 전환 중간에 "이미지+텍스트"를 동시에 세팅 (원래 스타일 유지)
            StartCoroutine(FadeTransition(1f, () =>
            {
                ShowCutscene(currentIndex);
            }));
        }
    }

    private void OnChoiceSelected(bool isA)
    {
        if (choiceAButton) choiceAButton.gameObject.SetActive(false);
        if (choiceBButton) choiceBButton.gameObject.SetActive(false);

        // 분기 배열 결정
        var branch = isA ? branchCutscenesA : branchCutscenesB;

        // 안전장치: 분기 배열이 비었으면 즉시 종료 버튼들 노출(원래 흐름 유지)
        if (branch == null || branch.Length == 0)
        {
            ShowEndControls();
            return;
        }

        currentCutscenes = branch;
        currentIndex = 0;
        ShowCutscene(currentIndex);
    }

    private void ShowEndControls()
    {
        nextCutsceneButton.gameObject.SetActive(false);
        if (storyEndButton) storyEndButton.gameObject.SetActive(true); // GoToMainScene 호출되는 기존 버튼
        if (tutorialButton) tutorialButton.gameObject.SetActive(true);
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

    private IEnumerator BackDelay()
    {
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
            fadeCanvasGroup.blocksRaycasts = true;
        else
            fadeCanvasGroup.blocksRaycasts = false;

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

    public void ShowMapChoices(List<MapType> mapOptions)
    {
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

    private string GetDisplayName(MapType type)
    {
        switch (type)
        {
            case MapType.심연의정원: return "심연의 정원";
            case MapType.수정해변:   return "수정 해변";
            default:                 return type.ToString();
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
    public class MapSpriteData {
        public MapType mapType;
        public Sprite mapSprite;
    }

    public MapSpriteData[] mapSprites;
    private Dictionary<MapType, Sprite> mapSpriteDict;  
}
