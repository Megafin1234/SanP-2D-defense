using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy")]

public class EnemySO : ScriptableObject
{
    public enum UnitType//어느계열 유닛인지에 대한 enum리스트
    {
        Melee,
        Ranged,
        Buff,
        Utility,
        Boss
    }

    [Header("Unit Information")]//유닛을 소환할 때 기본 프리팹에 덮어씌울 정보들.
    public int enemyIdx;
    public UnitType type;
    public string UnitName;
    public int level;
    public float speed;
    public float health;
    public float maxHealth;
    public float attackDelay = 1f;
    public float damage = 10f;
    public bool isBoss;
    private float attackTimer;
    public Sprite sprite;
    public RuntimeAnimatorController animCon;//애니메이션컨트롤러

    public List<ItemSO> possibleDrops; /////////////////////////////////

    [Header("Passive Skills")]
    public List<Skill> passiveSkills = new List<Skill>();//델리게이트 리스트로 스킬 구현할 것.
    public List<int> passiveSkillRequiredLevels = new List<int>();//스킬별 요구레벨(습득조건)

    //액티브스킬 관련 정보들
    [Header("Active Skills")]
    public List<Skill> activeSkills = new List<Skill>();//델리게이트 리스트로 구현한 액티브스킬목록
    public List<int> activeSkillRequiredLevels = new List<int>();//액티브스킬별 요구레벨

    //에너미는 시너지가 없음
}