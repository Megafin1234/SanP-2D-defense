using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MonsterDexDatabase", menuName = "MonsterDex/Database")]
public class MonsterDexDatabase : ScriptableObject
{
    public List<MonsterDexEntry> entries = new List<MonsterDexEntry>();

    public MonsterDexEntry GetEntry(EnemySO enemy)
    {
        return entries.Find(e => e.enemyData == enemy);
    }
}
