using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    public float levelTime;
    int level;
    float timer;

    void Awake() {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.dayPhaseTimer / spawnData.Length;
    }

    /*void Update(){
        if (!GameManager.instance.isLive)
            return;
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime),spawnData.Length -1); 
        if(timer > spawnData[level].spawnTime){
            timer =0f;   
            Spawn();
        } 
    }*/
    void Spawn()//어디에서도 참조하고 있지 않은 함수 - > 지워도 되는거?
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        //enemy.GetComponent<EnemyBase>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    internal bool isBoss;
}