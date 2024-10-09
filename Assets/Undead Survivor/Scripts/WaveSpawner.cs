using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    public Transform[] spawnPoint; // 적이 생성될 위치
    public WaveSpawnData[] waveSpawnData; // 웨이브에 대한 SpawnData
    public float waveInterval; // 웨이브 간의 간격
    public int[] monstersPerWave; // 각 웨이브마다 생성할 적의 수

    private int currentWave; // 현재 웨이브 인덱스
    private float timer; // 생성 타이머

    void Awake() {
    spawnPoint = GetComponentsInChildren<Transform>();
}


    void Start()
    {
        currentWave = 0; // 웨이브 초기화
        StartCoroutine(SpawnWaves()); // 웨이브 생성 시작
    }

    IEnumerator SpawnWaves()
    {
        Debug.Log("Starting SpawnWaves Coroutine");
        while (currentWave < monstersPerWave.Length)
        {
            for (int i = 0; i < monstersPerWave[currentWave]; i++)
            {
                Spawn(currentWave); // 현재 웨이브에 맞는 적 생성
                yield return new WaitForSeconds(waveSpawnData[currentWave].spawnTime); // 각 적의 생성 간격
            }

            currentWave++; // 다음 웨이브로 이동
            yield return new WaitForSeconds(waveInterval); // 웨이브 간 대기 시간
        }
    }

    void Spawn(int waveIndex)
    {
        GameObject enemy = GameManager.instance.pool.Get(0); // PoolManager에서 적 가져오기
        enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position; // 랜덤 생성 위치
        enemy.GetComponent<Enemy>().Init(waveSpawnData[waveIndex].ToSpawnData()); // 적 초기화
    }
}

[System.Serializable]
public class WaveSpawnData
{
    public float spawnTime; // 적의 생성 간격
    public int spriteType; // 적의 스프라이트 타입
    public int health; // 적의 체력
    public float speed; // 적의 이동 속도

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
