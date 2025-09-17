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

        // 시작 시 적들은 꺼둠
        if (captureEnemy != null) captureEnemy.SetActive(false);
        if (targetEnemy != null) targetEnemy.SetActive(false);

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

            case 4: // 펫으로 처치 성공 여부 확인
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
                instructionText.text = "이제 [R 키]로 몬스터를 포획해보세요!";
                if (captureEnemy != null) captureEnemy.SetActive(true);
                break;
            case 4:
                instructionText.text = "잘했습니다! 이제 포획한 펫으로 몬스터를 처치해보세요!";
                if (targetEnemy != null) targetEnemy.SetActive(true);
                break;
            case 5:
                instructionText.text = "튜토리얼이 끝났습니다!\n[튜토리얼 종료] 버튼을 눌러 게임으로 돌아가세요.";
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
