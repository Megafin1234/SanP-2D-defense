using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner instance;
    public Transform[] spawnPoint; 
    public WaveSpawnData[] waveSpawnData;
    public int[] monstersPerWave; 
    public int monsterSize;
    public Button nextWaveButton; 
    public Text waveDirectionText; 
    public Reposition reposition; 
    public int currentWave = 0;
    private bool isSpawning = false; 
    public int currentSpawnIndex; 
    public int currentWaveKillCount = 0; 
    public MapWaveFilter[] mapWaveFilters;  
    public BossEntry[] bossWaveFilters; 
    private Dictionary<MapType, List<int>> mapWaveFilterDict;

    private Dictionary<MapType, WaveSpawnData> bossSpawnDataDict; // 실행 시 참조용
    private bool bossSpawned = false;


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
        monsterSize = 17;

        mapWaveFilterDict = new Dictionary<MapType, List<int>>();
        foreach (var filter in mapWaveFilters){
            mapWaveFilterDict[filter.mapType] = filter.allowedSpawnIndices;
        }
        bossSpawnDataDict = new Dictionary<MapType, WaveSpawnData>();
        foreach (var filter in bossWaveFilters)
        {
            if (!bossSpawnDataDict.ContainsKey(filter.mapType))
                bossSpawnDataDict.Add(filter.mapType, filter.bossData);
}
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
        bossSpawned = false;
        
        List<int> allowedIndices = GetAllowedEnemyForCurrentMap();
            /*if (allowedIndices == null || allowedIndices.Count == 0){
        Debug.LogWarning($"[WaveSpawner] No valid spawn indices for wave {waveIndex} in map {Reposition.instance.currentMap}");
            yield break;
        }*/

        for (int i = 0; i < monstersPerWave[waveIndex]; i++)
        {
            int dataIndex = allowedIndices[Random.Range(0, allowedIndices.Count)];
            WaveSpawnData spawnData = waveSpawnData[dataIndex];
            Spawn(spawnData, dataIndex);
            yield return new WaitForSeconds(spawnData.spawnTime); 
        }

        if ((waveIndex + 1) % 4 == 0 && !bossSpawned)
        {
            yield return new WaitForSeconds(2f); // 약간 지연 효과
            SpawnBoss(); 
        }

        int totalExpectedKills = monstersPerWave[waveIndex] + (bossSpawned ? 1 : 0);
        yield return new WaitUntil(() => currentWaveKillCount >= totalExpectedKills);

        isSpawning = false;
        currentWave++;
        GameManager.instance.NightToDay();
    }
    private List<int> GetAllowedEnemyForCurrentMap()
    {
        MapType map = Reposition.instance.currentMap;
        if (mapWaveFilterDict.TryGetValue(map, out var list))
        {
            return list;
        }
        return new List<int>();
    }

    void Spawn(WaveSpawnData data,int Idx)
    {
        int prefabIndex = GetEnemyPrefabIndex(data.type);
        GameObject enemy = GameManager.instance.pool.Get(prefabIndex);

        Vector3 spawnPosition = spawnPoint[currentSpawnIndex].position;
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        enemy.transform.position = spawnPosition + offset;

        enemy.GetComponent<EnemyBase>().Init(data.ToSpawnData());
        enemy.GetComponent<EnemyBase>().enemyIdx = Idx;
    }

    void SpawnBoss()
    {
        MapType currentMap = Reposition.instance.currentMap;

        if (!bossSpawnDataDict.TryGetValue(currentMap, out WaveSpawnData bossData))
        {
            return;
        }

        int bossPrefabIndex = GetEnemyPrefabIndex(bossData.type); // 기존 EnemyType 사용
        GameObject boss = GameManager.instance.pool.Get(bossPrefabIndex);

        Vector3 spawnPosition = spawnPoint[currentSpawnIndex].position;
        boss.transform.position = spawnPosition;

        SpawnData spawnStruct = bossData.ToSpawnData();
        //spawnStruct.isBoss = true; 

        boss.GetComponent<EnemyBase>().Init(spawnStruct);

        bossSpawned = true;
    }

    public void NtoDbuttons() //밤> 낮 전환시 버튼이 안나오는 오류 해결 위해 gamemanager>startDayPhaze에서 아래 코드들을 통제
    {
        if (currentWave < monstersPerWave.Length)
        {
            nextWaveButton.gameObject.SetActive(true);
            waveDirectionText.gameObject.SetActive(false);
        }
    }

    int GetEnemyPrefabIndex(EnemyType type)
    {
    return 10 + (int)type; // Enemy 프리팹은 10번대부터 배치함 풀매니저 123번에 불렛이 자리하고 있음
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
    public EnemyType type; 
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    public bool isBoss; 


    public SpawnData ToSpawnData()
    {
        return new SpawnData
        {
            spawnTime = this.spawnTime,
            spriteType = this.spriteType,
            health = this.health,
            speed = this.speed,
            isBoss = this.isBoss
        };
    }
}
public enum EnemyType
{
    Melee,
    Ranged,
    Suicide,
    Buffer,
    Boss
}
[System.Serializable]
public class MapWaveFilter
{
    public MapType mapType;
    public List<int> allowedSpawnIndices; 
}
[System.Serializable]
public class BossEntry {
    public MapType mapType;      
    public WaveSpawnData bossData;  
}