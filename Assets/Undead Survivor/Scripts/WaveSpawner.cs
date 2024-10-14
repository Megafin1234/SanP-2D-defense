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
        
    waveDirectionText.gameObject.SetActive(false); // 텍스트 초기 비활성화    
    }

    public void StartWave()
    {
        if (!isSpawning && currentWave < monstersPerWave.Length)
        {
            currentSpawnIndex = Random.Range(1, spawnPoint.Length); //방향랜덤선택
            UpdateWaveDirectionText(currentSpawnIndex); // 방향 텍스트 업데이트
            StartCoroutine(SpawnWave(currentWave));
            nextWaveButton.gameObject.SetActive(false); // 웨이브 중 버튼 비활성화
            waveDirectionText.gameObject.SetActive(true); // 방향 텍스트 활성화
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
        if (currentWave % 2 == 0)
        {
            reposition.ToggleTilemapLayers(); // 타일맵 레이어 전환
        }
        if (currentWave < monstersPerWave.Length)
        {
            nextWaveButton.gameObject.SetActive(true); // 웨이브 종료 후 버튼 활성화
            waveDirectionText.gameObject.SetActive(false); // 웨이브 종료 후 방향 텍스트 비활성화
        }
    }

    void Spawn(int waveIndex)
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[currentSpawnIndex].position;
        enemy.GetComponent<Enemy>().Init(waveSpawnData[waveIndex].ToSpawnData());
    }
     void UpdateWaveDirectionText(int spawnIndex)
    {
        string direction = "";

        // spawnIndex에 따라 방향 텍스트 설정
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
