using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public Text instructionText;

    private int step = 0;
    private bool moved, attacked, dashed;

    void Start()
    {
        ShowStep(); // 첫 메시지 표시
            }

    void Update()
    {
        switch (step)
        {
            case 0: // 이동
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    moved = true;
                    StartCoroutine (NextStep());
                }
                break;
            case 1: // 공격
                if (Input.GetMouseButtonDown(0))
                {
                    attacked = true;
                    StartCoroutine (NextStep());
                }
                break;
            case 2: // 대시
                if (Input.GetMouseButtonDown(1))
                {
                    dashed = true;
                    StartCoroutine (NextStep());
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
                instructionText.text = "튜토리얼이 끝났습니다!";
                instructionText.text += "\n[튜토리얼 종료] 버튼을 눌러 게임으로 돌아가세요.";
                break;
        }
    }

    IEnumerator NextStep()
    {
        instructionText.text = "잘하셨습니다!";
        yield return new WaitForSeconds(1f); 
        step++;
        ShowStep();
    }
}
