using UnityEngine;

public enum ItemType { Material, Consumable, Equipment }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    public int id;
    public string itemName;
    public string description;
    public Sprite icon;
    public ItemType itemType;
    public int maxStack = 1;
}
