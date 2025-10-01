using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Scriptable Object/BuffSO")]
public class BuffSO : ScriptableObject
{
    public enum BuffType { Glove, Shoe }

    [Header("# Main Info")]
    public BuffType buffType;
    public int itemId;
    public string itemName;
    [TextArea] public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    // 기존 ItemData.damages[]과 동일한 개념(퍼센트 계열이면 0.1 => 10%)
    public float[] rates;
}