using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SKP_InventorySlot
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

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int slotCount = 20;
    public List<SKP_InventorySlot> slots = new List<SKP_InventorySlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new SKP_InventorySlot());
        }
    }

    public void AddSlots(int moreMoreSlot){
        slotCount += moreMoreSlot;
        for(int i=0; i<moreMoreSlot; i++){
            slots.Add(new SKP_InventorySlot());
        }
    }

    public bool PlaceItemInSlot(int slotIndex, SKP_Item item, int quantity)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogWarning("잘못된 슬롯 인덱스");
            return false;
        }

        var slot = slots[slotIndex];

        if (!slot.IsEmpty)
        {
            Debug.Log("이 슬롯은 이미 사용 중입니다.");
            return false;
        }

        slot.SetItem(item, quantity);
        return true;
    }

    // 슬롯 아이템 제거
    public void RemoveItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            slots[slotIndex].Clear();
        }
    }
}
