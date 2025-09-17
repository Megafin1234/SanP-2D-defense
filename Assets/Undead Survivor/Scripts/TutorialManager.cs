using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    public Text instructionText;
    public Button endTutorialButton;

    [Header("Tutorial Enemies")]
    public GameObject captureEnemy; // 포획 대상 (씬에 미리 배치, 처음엔 비활성화)
    public GameObject targetEnemy;  // 펫으로 처치할 대상 (씬에 미리 배치, 처음엔 비활성화)

    private int step = 0;
    private bool isProcessingStep = false;

    public static bool isTutorial = false;

    void Start()
    {
        isTutorial = true;

        // 게임 자동 시작 (캐릭터 선택 건너뜀)
        GameManager.instance.GameStart(0);

        // 튜토리얼 몬스터 enemyIdx 강제 세팅
        if (captureEnemy != null)
        {
            EnemyBase eb = captureEnemy.GetComponent<EnemyBase>();
            if (eb != null)
            {
                eb.enemyIdx = 7; // 슬라임 전용 idx
                Debug.Log("[튜토리얼] captureEnemy idx 강제 세팅 = 7");
            }
            captureEnemy.SetActive(false);
        }

        if (targetEnemy != null)
        {
            EnemyBase eb = targetEnemy.GetComponent<EnemyBase>();
            if (eb != null)
            {
                eb.enemyIdx = 7; // 슬라임 전용 idx
                Debug.Log("[튜토리얼] targetEnemy idx 강제 세팅 = 7");
            }
            targetEnemy.SetActive(false);
        }

        ShowStep();
    }

    void Update()
    {
        if (isProcessingStep) return;

        switch (step)
        {
            case 0: // 이동
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                    StartCoroutine(NextStep());
                break;

            case 1: // 공격
                if (Input.GetMouseButtonDown(0))
                    StartCoroutine(NextStep());
                break;

            case 2: // 대시
                if (Input.GetMouseButtonDown(1))
                    StartCoroutine(NextStep());
                break;

            case 3: // 포획 성공 여부 확인
                if (captureEnemy != null && !captureEnemy.activeSelf)
                    StartCoroutine(NextStep());
                break;

            case 4: // 펫 소환 확인
                // 파티 리스트에 있는 펫 중 하나라도 활성화되면 달성
                if (GameManager.instance.party.Exists(p => p.activeSelf))
                    StartCoroutine(NextStep());
                break;

            case 5: // 펫으로 처치 성공 여부 확인
                if (targetEnemy != null && !targetEnemy.activeSelf)
                    StartCoroutine(NextStep());
                break;
        }
    }

    void ShowStep()
    {
        switch (step)
        {
            case 0:
                instructionText.text = "WASD 키로 이동해보세요!";
                break;
            case 1:
                instructionText.text = "마우스 왼쪽 버튼으로 공격해보세요!";
                break;
            case 2:
                instructionText.text = "마우스 오른쪽 버튼으로 대시해보세요!";
                break;
            case 3:
                instructionText.text = "이제 [R 키]로 힘을 개방해 몬스터를 포획해보세요!";
                if (captureEnemy != null) captureEnemy.SetActive(true);
                break;
            case 4:
                instructionText.text = "[P 키]로 펫 가방을 열고 펫을 소환해보세요!";
                break;
            case 5:
                instructionText.text = "이제 포획한 펫이 몬스터를 처치하길 기다리세요!";
                if (targetEnemy != null) targetEnemy.SetActive(true);
                break;
            case 6:
                instructionText.text = "튜토리얼이 끝났습니다!\n[튜토리얼 종료] 버튼을 눌러 게임을 시작하세요.";
                break;
        }
    }

    IEnumerator NextStep()
    {
        isProcessingStep = true;
        instructionText.text = "잘하셨습니다!";
        yield return new WaitForSeconds(1f);
        step++;
        ShowStep();
        isProcessingStep = false;
    }

    public void EndTutorial()
    {
        isTutorial = false;
        PlayerPrefs.SetInt("CameFromTutorial", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}
