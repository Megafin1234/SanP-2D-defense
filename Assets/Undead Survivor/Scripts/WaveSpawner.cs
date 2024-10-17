using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoint; // 적 생성 위치들
    public WaveSpawnData[] waveSpawnData; // 웨이브마다 사용할 스폰 데이터
    public int[] monstersPerWave; // 각 웨이브에서 생성할 몬스터 수
    public Button nextWaveButton; // 웨이브 시작 버튼
    public Text waveDirectionText; // 웨이브 방향 표시 텍스트
    public Reposition reposition; // Reposition 스크립트 참조
    private int currentWave = 0; // 현재 웨이브 인덱스
    private bool isSpawning = false; // 웨이브가 진행 중인지 확인
    private int currentSpawnIndex; // 현재 웨이브에서 사용할 스폰 포인트 인덱스


    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        
    waveDirectionText.gameObject.SetActive(false);    
    }

    public void StartWave()
    {
        if (!isSpawning && currentWave < monstersPerWave.Length)
        {
            currentSpawnIndex = Random.Range(1, spawnPoint.Length); 
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
            yield return new WaitForSeconds(waveSpawnData[waveIndex].spawnTime); // 각 적의 생성 간격
        }

        isSpawning = false;
        currentWave++;
        if (currentWave % 3 == 0)
        {
            reposition.ToggleTilemapLayers(); // 타일맵 레이어 전환
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
