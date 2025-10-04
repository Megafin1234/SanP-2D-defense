using UnityEngine;

[System.Serializable]
public class MonsterDexEntry
{
    [Header("기본 정보")]
    public EnemySO enemyData;
    public Sprite icon;
    [TextArea(2, 5)] public string description;
    public EnemySO.UnitType type;
    public bool isBoss;

    [Header("도감 상태")]
    public bool isDiscovered;  // 조우 여부
    public bool isDefeated;    // 처치 여부
    public bool isCaptured;    // 포획 여부
}
