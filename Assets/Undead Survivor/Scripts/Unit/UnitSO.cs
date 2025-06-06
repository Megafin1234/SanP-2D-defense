using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewUnit", menuName = "Unit")]

public class UnitSO : ScriptableObject
{
    public enum UnitType//어느계열 유닛인지에 대한 enum리스트
    {
        Melee,
        Ranged,
        Buff,
        Utility
    }

    [Header("Unit Information")]//유닛을 소환할 때 기본 프리팹에 덮어씌울 정보들.
    public UnitType type;
    public string UnitName;
    public int level;
    public float speed;
    public float health;
    public float maxHealth;
    public float attackDelay = 1f;
    public float damage = 10f;
    private float attackTimer;
    public Sprite sprite;
    public RuntimeAnimatorController overrideController;//애니메이션컨트롤러

    [Header("Passive Skills")]
    public List<Skill> passiveSkills = new List<Skill>();//델리게이트 리스트로 스킬 구현할 것.
    public List<int> passiveSkillRequiredLevels = new List<int>();//스킬별 요구레벨(습득조건)

    //액티브스킬 관련 정보들
    [Header("Active Skills")]
    public List<Skill> activeSkills = new List<Skill>();//델리게이트 리스트로 구현한 액티브스킬목록
    public List<int> activeSkillRequiredLevels = new List<int>();//액티브스킬별 요구레벨

    //롤체시너지같은 시스템
    [Header("Synergies")]
    public List<string> synergies = new List<string>();//시너지 시스템은 나중에 다른식으로 보완해서 구현할 것. 일단 스트링으로 구색만 맞추기.
}

[System.Serializable]
public class Skill
{
    public string skillName; //스킬이름
    public int requiredLevel; //요구레벨..? 여기서 처리할지 so자체에 리스트로 넣을지 고민중.
    int skillType;
}
