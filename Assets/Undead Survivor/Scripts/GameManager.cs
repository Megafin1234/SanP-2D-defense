using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    [Header("#HUD References")]
    public GameObject dayTimerHUD; 
    public GameObject nightTimerHUD; 

    public GameObject dayCountHUD; 
    public GameObject nightCountHUD; 

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

    public List<Enemy> party;

    private bool isDayPhase = true;

    void Awake(){
        instance = this;
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
        if (!isLive)
            return;

        if(DayCount > 3){
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
        merchant.SetActive(false);//유닛상인 비활성화
    }

    public void NightToDay()
    {
        isDayPhase = true;
        isLive = false;
        dayPhaseTimer = dayPhaseDuration;
        
        UIManager.instance.ShowDayPhaseText();

        UIManager.instance.FadeOut(() =>
        {
            UIManager.instance.FadeIn(() =>
            {
                isLive = true;
                DayCount++;
                StartDayPhase();
            });
        });
        merchant.SetActive(true);//유닛상인 활성화
    }

    private void StartDayPhase()
    {
        UIManager.instance.DayEffect();
        ActivateDayTimer();
        stageLevelUp.Show();
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
    nightTimerHUD.SetActive(true); 
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
