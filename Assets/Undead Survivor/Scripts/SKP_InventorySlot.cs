using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SKP_InventorySlot : MonoBehaviour
{
    public SKP_Item item;
    public int quantity;

    public bool IsEmpty => item == null;

    public void SetItem(SKP_Item newItem, int amount)
    {
        item = newItem;
        quantity = amount;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}