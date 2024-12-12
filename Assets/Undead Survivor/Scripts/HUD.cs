using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill,DayCount,NightCount, DayTime, NightTime, Health, DashCooldown}
    public InfoType type;

    Text myText;
    Slider mySlider;
    public Button nextWaveButton;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }
    void LateUpdate()
    {
        switch (type){
            case InfoType.Exp:
                float curExp = GameManager.instance.exp;
                float maxExp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level,GameManager.instance.nextExp.Length-1) ];
                mySlider.value = curExp/maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.DayCount:
                myText.text = string.Format("{0:F0}일차 낮", GameManager.instance.DayCount);
                break;
            case InfoType.NightCount:
                myText.text = string.Format("{0:F0}일차 밤", GameManager.instance.NightCount);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.DayTime:
                float remainDayTime = GameManager.instance.dayPhaseTimer;
                int min = Mathf.FloorToInt(remainDayTime/60);
                int sec = Mathf.FloorToInt(remainDayTime%60);
                if(min<0 || sec<0){
                    myText.text = string.Format("--: --");
                }
                else{
                    myText.text = string.Format("{0:D2}: {1:D2}", min, sec);
                }    
                break;
            case InfoType.NightTime:
                float remainNightTime = GameManager.instance.nightPhaseTimer;
                int nmin = Mathf.FloorToInt(remainNightTime/60);
                int nsec = Mathf.FloorToInt(remainNightTime%60);
                if(nmin<0 || nsec<0){
                    myText.text = string.Format("--: --");
                }
                else{
                    myText.text = string.Format("{0:D2}: {1:D2}", nmin, nsec);
                }     
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                mySlider.value = curHealth/maxHealth;
                break;
            case InfoType.DashCooldown:
                float curCool = GameManager.instance.player.dashWaiting;
                float maxCool = GameManager.instance.player.dashCoolDown;
                mySlider.value = (maxCool-curCool)/maxCool;
                Transform child = transform.Find("DashCoolTXT");
                myText = child.GetComponent<Text>();
                if(curCool<=0){
                    myText.text = "";
                }
                else{
                    myText.text = string.Format("{0:F1}s", curCool);
                }
                break;          
        }
    }
}
