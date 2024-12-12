using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;
    public Transform[] spawnPoint; 
    public WaveSpawnData[] waveSpawnData;
    public int[] monstersPerWave; 
    public Button nextWaveButton; 
    public Text waveDirectionText; 
    public Reposition reposition; 
    private int currentWave = 0;
    private bool isSpawning = false; 
    private int currentSpawnIndex; 
    public int currentWaveKillCount = 0; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
        spawnPoint = GetComponentsInChildren<Transform>();
        waveDirectionText.gameObject.SetActive(false);    
    }

    public void WaveStartButton(){
        GameManager.instance.DayToNight();
    }

    public void StartWave()
    {

        if (!isSpawning && currentWave < monstersPerWave.Length)
        {
            currentSpawnIndex = Random.Range(1, spawnPoint.Length); 
            currentWaveKillCount = 0; 
            UpdateWaveDirectionText(currentSpawnIndex); 
            StartCoroutine(SpawnWave(currentWave));
            nextWaveButton.gameObject.SetActive(false); 
            waveDirectionText.gameObject.SetActive(true); 
        }
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        isSpawning = true;

        for (int i = 0; i < monstersPerWave[waveIndex]; i++)
        {
            Spawn(waveIndex);
            yield return new WaitForSeconds(waveSpawnData[waveIndex].spawnTime); 
        }

    
        yield return new WaitUntil(() => currentWaveKillCount >= monstersPerWave[waveIndex]);

        isSpawning = false;
        currentWave++;
        GameManager.instance.NightToDay();
        if (currentWave % 1 == 0)
        {
            reposition.ToggleTilemapLayers(); 
        }
        if (currentWave < monstersPerWave.Length)
        {
            nextWaveButton.gameObject.SetActive(true); 
            waveDirectionText.gameObject.SetActive(false); 
        }
    }

    void Spawn(int waveIndex)
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        Vector3 spawnPosition = spawnPoint[currentSpawnIndex].position;
        float randomOffsetX = Random.Range(-1f, 1f); 
        float randomOffsetY = Random.Range(-1f, 1f); 
        enemy.transform.position = spawnPosition + new Vector3(randomOffsetX, randomOffsetY, 0);
        enemy.GetComponent<Enemy>().Init(waveSpawnData[waveIndex].ToSpawnData());
    }

    void UpdateWaveDirectionText(int spawnIndex)
    {
        string direction = "";

        switch (spawnIndex)
        {
            case 1:
                direction = "북쪽에서 땅울림이 느껴집니다...";
                break;
            case 2:
                direction = "동쪽에서 무언가가 몰려옵니다...";
                break;
            case 3:
                direction = "서쪽 하늘이 어두워집니다...";
                break;
            case 4:
                direction = "남쪽에서 비명소리가 들립니다...";
                break;
            default:
                direction = "Unknown";
                break;
        }

        waveDirectionText.text = $"{direction}";
    }
}



[System.Serializable]
public class WaveSpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;

    public SpawnData ToSpawnData()
    {
        return new SpawnData
        {
            spawnTime = this.spawnTime,
            spriteType = this.spriteType,
            health = this.health,
            speed = this.speed
        };
    }
}
