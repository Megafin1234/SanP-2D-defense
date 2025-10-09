using UnityEngine;

public class MonsterDexManager : MonoBehaviour
{
    public static MonsterDexManager Instance;
    private MonsterDexDatabase db;

    private static bool isDexInitialized = false;  // 게임 세션 내 1회만 초기화

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            db = MonsterDexDatabase.Instance;

            // ✅ 첫 게임 시작 시에만 초기화
            if (!isDexInitialized)
            {
                ResetDex();
                isDexInitialized = true;
            }

            DontDestroyOnLoad(gameObject); // 씬 이동 시 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetDex()
    {
        foreach (var e in db.entries)
        {
            e.isDiscovered = false;
            e.isDefeated = false;
            e.isCaptured = false;
        }
        Debug.Log("🧹 도감 초기화 완료 (게임 첫 시작 시 1회)");
    }

    public void RegisterDiscovery(EnemySO enemy) => db.RegisterDiscovery(enemy);
    public void RegisterDefeat(EnemySO enemy) => db.RegisterDefeat(enemy);
    public void RegisterCapture(EnemySO enemy) => db.RegisterCapture(enemy);
}
