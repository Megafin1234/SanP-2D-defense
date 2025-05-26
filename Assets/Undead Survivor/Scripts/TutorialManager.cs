using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Text instructionText;
    public Button endTutorialButton;
    public Button nextWaveButton; // 튜토리얼에서 버튼 클릭여부 체크용용
    private bool waveStarted = false;

    private int step = 0;
    private bool moved, attacked, dashed;
    private bool isProcessingStep = false;
    void Start()
    {
        ShowStep(); // 첫 메시지 표시
    }

    void Update()
    {
        if (isProcessingStep) return;
        switch (step)
        {
            case 0: // 이동
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    moved = true;
                    StartCoroutine(NextStep());
                }
                break;
            case 1: 
                if (Input.GetMouseButtonDown(0))
                {
                    attacked = true;
                    StartCoroutine(NextStep());
                }
                break;
            case 2: 
                if (Input.GetMouseButtonDown(1))
                {
                    dashed = true;
                    StartCoroutine(NextStep());
                }
                break;
            case 3:
                if (step == 3 && waveStarted)
                {
                    StartCoroutine(NextStep());
                }
                break;
            case 4:
                if (GameManager.instance.kill >= 1)
                {
                    StartCoroutine(NextStep());
                }
                break;
            case 5:
                if (GameManager.instance.kill >= 5)
                {
                    StartCoroutine(NextStep());
                }
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
                instructionText.text = "다음 웨이브 시작 버튼을 눌러보세요!";
                nextWaveButton.gameObject.SetActive(true);//사실 이미 액티브긴함
                break;
            case 4:
                instructionText.text = "몬스터를 처치해보세요!";
                break;
            case 5:
                instructionText.text = "웨이브를 모두 클리어해보세요!";
                break;
            case 6:
                instructionText.text = "튜토리얼이 끝났습니다!";
                instructionText.text += "\n[튜토리얼 종료] 버튼을 눌러 게임으로 돌아가세요.";
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
        PlayerPrefs.SetInt("CameFromTutorial", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
    public void OnClick_NextWave()
    {
        waveStarted = true;
        nextWaveButton.gameObject.SetActive(false); 
    }
}
