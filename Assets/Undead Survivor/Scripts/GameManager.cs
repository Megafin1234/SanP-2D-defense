using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SKP_HUD inventoryScript;
    [Header("#Game Control")]
    public bool isLive;
    public float dayPhaseTimer;  
    public float nightPhaseTimer; 
    public int DayCount = 1;
    public int NightCount;
    public float dayPhaseDuration = 180f; 
    public float nightPhaseDuration = 120f; 
    
    
    [Header("#Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int coin;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 360, 450, 600};
    public int[] equipSkillIDs = { 0, 0, 0, 0 }; // Q, E, X, C
    public int[] equipSkillLvls = { 0, 1, 2, 3 }; // Q, E, X, C

    [Header("#HUD References")]
    public GameObject dayTimerHUD; 
    public GameObject nightTimerHUD; 

    public GameObject dayCountHUD; 
    public GameObject nightCountHUD;
    public GameObject inventoryHUD;

    [Header("#Game Object")]

    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUp;
    public GameObject UnitShopPanel;
    public LevelUp stageLevelUp;
    public Result uiResult;
    public GameObject enemyCleaner;
    public Weapon weapon;
    public GameObject merchant;
    public GameObject merchantInteraction;
    public List<GameObject> pets;
    public List<GameObject> party;

    public List<UnitSO> unitSOList;

    public Texture2D crosshair;
    public Image crosshair2;
    public Vector2 hotspot = new Vector2(16, 16);

    private bool isDayPhase = true;

    void Awake(){
        instance = this;
        Cursor.SetCursor(crosshair, hotspot, CursorMode.Auto); 
        inventoryScript = inventoryHUD.GetComponent<SKP_HUD>();
    }
    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;
        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);
        Resume();
        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        dayPhaseTimer = dayPhaseDuration;
        DayCount = 1;
        NightCount = 0;
    }
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        isLive =false;
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.lose();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);

    }

    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine());
    }
    IEnumerator GameVictoryRoutine()
    {
        isLive =false;
        enemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop();
        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);

    }
    public void GameRetry()

    {
        SceneManager.LoadScene(0);

    }
    void Update(){
        crosshair2.transform.position = Input.mousePosition;
        if (!isLive)
            return;
        if (SceneManager.GetActiveScene().name == "Tutorial")
            return; 

        if(WaveSpawner.instance.currentWave >= WaveSpawner.instance.monstersPerWave.Length){
            GameVictory();
        } 
        if (isDayPhase)
        {
            dayPhaseTimer -= Time.deltaTime;
            if (dayPhaseTimer <= 0)
            {
                DayToNight();
            }
        }
        else
        {
            nightPhaseTimer -= Time.deltaTime;
            if (nightPhaseTimer <= 0)
            {
                GameRetry();
            }
        }

    }

    public void DayToNight()
    {
        isDayPhase = false;
        nightPhaseTimer = nightPhaseDuration;
        NightCount++;
        isLive = false;
        
        player.inputVec = Vector2.zero;
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        UIManager.instance.ShowNightPhaseText();

        UIManager.instance.FadeOut(() =>
        {
            UIManager.instance.NightEffect();
            UIManager.instance.FadeIn(() =>
            {
                
                isLive = true;
                StartNightPhase();
            });
        });
        //merchant.SetActive(false);//유닛상인 비활성화
    }

    public void NightToDay()
    {
        isDayPhase = true;
        isLive = false;
        dayPhaseTimer = dayPhaseDuration;

        player.inputVec = Vector2.zero;
        player.GetComponent<Animator>().SetFloat("Speed", 0f);
        player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        
        UIManager.instance.ShowDayPhaseText();

        UIManager.instance.FadeOut(() =>
        {
            UIManager.instance.FadeIn(() =>
            {
                DayCount++;
                if (WaveSpawner.instance.currentWave > 0 && WaveSpawner.instance.currentWave % 4 == 0)
                {
                    List<MapType> allMaps = System.Enum.GetValues(typeof(MapType)).Cast<MapType>().ToList();//맵타입을 가져와 배열로 전환
                    allMaps.Remove(Reposition.instance.currentMap);
                    List<MapType> options = allMaps.OrderBy(x => Random.value).Take(2).ToList();

                    UIManager.instance.ShowMapChoices(options);
                    return; // 선택 후에 Resume
                }
                isLive = true;
                
                StartDayPhase();
            });
        });
        //merchant.SetActive(true);//유닛상인 활성화
    }

    public void StartDayPhase()
    {
        UIManager.instance.DayEffect();
        ActivateDayTimer();
        stageLevelUp.Show();
        WaveSpawner.instance.NtoDbuttons();
    }

    private void StartNightPhase()
    {
        ActivateNightTimer();
        WaveSpawner.instance.StartWave();
    }

public void ActivateDayTimer()
{
    dayTimerHUD.SetActive(true);   
    nightTimerHUD.SetActive(false); 
    dayCountHUD.SetActive(true);
    nightCountHUD.SetActive(false);
}

public void ActivateNightTimer()
{
    dayTimerHUD.SetActive(false);  
    nightTimerHUD.SetActive(true);// 
    dayCountHUD.SetActive(false);
    nightCountHUD.SetActive(true);
}

    public void GetExp()
    {   
        if (!isLive)
            return;
        exp++;

        if(exp == nextExp[Mathf.Min(level,nextExp.Length-1)]){
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void GetPet(int type, int idx, Transform originTransform){
        idx = 0;
        GameObject newPet = Instantiate(pets[type]); // 부모 없이 생성
        newPet.transform.SetParent(player.transform); // 부모를 플레이어로 설정
        newPet.transform.position = originTransform.position; // 위치를 originTransform과 동일하게 설정
        party.Add(newPet);
        newPet.name = unitSOList[idx].UnitName;
        //newPet의 데이터를 SO 리스트의 idx번 데이터에서 전부 가져와 덮어씌우기
        newPet.GetComponent<EnemyBase>().name = unitSOList[idx].UnitName;
        //레벨 설정
        newPet.GetComponent<EnemyBase>().speed = unitSOList[idx].speed;
        newPet.GetComponent<EnemyBase>().health = unitSOList[idx].health;
        //newPet.GetComponent<EnemyBase>().Sprite = unitSOList[idx].sprite;
        //newPet.GetComponent<EnemyBase>().speed = unitSOList[idx].speed;
        newPet.SetActive(true);
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
