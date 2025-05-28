using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SKP_Item
{
    public int id;
    public string name;
    public string desc;
    public List<string> attributes;
    public int maxStack;
    public SKP_Item(string name, string desc, int maxStack = 64)
    {
        this.name = name;
        this.desc = desc;
        this.attributes = new List<string>();
        this.maxStack = maxStack;
    }
    public override bool Equals(object obj)
    {
        if (obj is not SKP_Item other) return false;
        return id == other.id &&
               name == other.name &&
               desc == other.desc &&
               maxStack == other.maxStack &&
               attributes.SequenceEqual(other.attributes);
    }
}


public class SKP_Inv : MonoBehaviour
{
    public static SKP_Inv Instance;
    public SKP_InventorySlot mouseSlot = new SKP_InventorySlot();

    public GameObject slotPrefab;
    public Transform itemGroupTransform;
    public int slotCount = 20;
    public List<SKP_InventorySlot> slots = new List<SKP_InventorySlot>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        InitSlots();
        gameObject.SetActive(false);
    }
    void Update()
    {

    }

    void InitSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(slotPrefab, itemGroupTransform);
            slot.name = $"Slot {i}";
            slots.Add(new SKP_InventorySlot());
        }
    }
    public void AddSlots(int moreMoreSlot)
    {
        for (int i = 0; i < moreMoreSlot; i++)
        {
            GameObject slot = Instantiate(slotPrefab, itemGroupTransform);
            slot.name = $"Slot {i + slotCount}";
            slots.Add(new SKP_InventorySlot());
        }
        slotCount += moreMoreSlot;
    }

    public void InvToggle()
    {
        //Debug.Log("InvToggle");
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            OnInvClose();
        }
        else
        {
            gameObject.SetActive(true);
            OnInvOpen();
        }
    }

    public void InvOpen()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            OnInvOpen();
        }
    }

    public void InvClose()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            OnInvClose();
        }
    }

    void OnInvOpen()
    {

    }

    void OnInvClose()
    {

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
    
    public void RemoveItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
        {
            slots[slotIndex].Clear();
        }
    }
}
