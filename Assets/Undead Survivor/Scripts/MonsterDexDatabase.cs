using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MonsterDex/Database")]
public class MonsterDexDatabase : ScriptableObject
{
    private static MonsterDexDatabase _instance;
    public static MonsterDexDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<MonsterDexDatabase>("MonsterDexDatabase"); // 경로 조정 가능
            return _instance;
        }
    }

    [System.Serializable]
    public class MonsterEntry
    {
        public EnemySO enemyData;
        public Sprite icon;
        public bool isDiscovered;
        public bool isDefeated;
        public bool isCaptured;
    }

    public List<MonsterEntry> entries = new List<MonsterEntry>();

    public MonsterEntry GetEntry(EnemySO enemy)
    {
        foreach (var entry in entries)
            if (entry.enemyData == enemy)
                return entry;
        return null;
    }

    public void RegisterDiscovery(EnemySO enemy)
    {
        var e = GetEntry(enemy);
        if (e != null && !e.isDiscovered) e.isDiscovered = true;
    }

    public void RegisterDefeat(EnemySO enemy)
    {
        var e = GetEntry(enemy);
        if (e != null)
        {
            e.isDiscovered = true;
            e.isDefeated = true;
        }
    }

    public void RegisterCapture(EnemySO enemy)
    {
        var e = GetEntry(enemy);
        if (e != null)
        {
            e.isDiscovered = true;
            e.isCaptured = true;
        }
    }
}
