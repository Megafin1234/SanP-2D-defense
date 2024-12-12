using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterClass", menuName = "Character Class")]
public class ClassSO : ScriptableObject
{
    public enum ClassType//어느계열 직업인지에 대한 enum리스트
    {
        Combat,
        Construction,
        ResourceCollection,
        Buffer,
        Debuffer,
        Utility
    }
    

    //패시브스킬 관련 정보들
    [Header("Class Information")]
    public ClassType classType;//직업계열
    public string className;//직업명

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