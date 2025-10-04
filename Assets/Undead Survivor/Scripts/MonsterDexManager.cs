using UnityEngine;

public class MonsterDexManager : MonoBehaviour
{
    public static MonsterDexManager Instance;
    public MonsterDexDatabase database;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterDiscovery(EnemySO enemy)
    {
        var entry = database.GetEntry(enemy);
        if (entry != null)
            entry.isDiscovered = true;
    }

    public void RegisterDefeat(EnemySO enemy)
    {
        var entry = database.GetEntry(enemy);
        if (entry != null)
        {
            entry.isDiscovered = true;
            entry.isDefeated = true;
        }
    }

    public void RegisterCapture(EnemySO enemy)
    {
        var entry = database.GetEntry(enemy);
        if (entry != null)
        {
            entry.isDiscovered = true;
            entry.isCaptured = true;
        }
    }
}
