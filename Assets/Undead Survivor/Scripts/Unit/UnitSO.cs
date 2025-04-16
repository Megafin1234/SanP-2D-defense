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

    [Header("Class Information")]
    public string UnitName;
    public RuntimeAnimatorController overrideController;//애니메이션컨트롤러

}
