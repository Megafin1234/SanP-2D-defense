using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager Instance;

	[System.Serializable]
	public class InventoryStack
	{
		public ItemSO itemData;
		public int quantity;
	}

	public const int MaxPerSlot = 999;
	public int maxSlots = 30;
	public List<InventoryStack> slots = new List<InventoryStack>();

	public System.Action OnInventoryChanged;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public bool AddItem(ItemSO item, int amount)
	{
		if (amount <= 0 || item == null)
			return false;

		int remaining = amount;

		// Fill existing stacks of the same item up to MaxPerSlot
		for (int i = 0; i < slots.Count && remaining > 0; i++)
		{
			var stack = slots[i];
			if (stack.itemData == item && stack.quantity < MaxPerSlot)
			{
				int canAdd = Mathf.Min(MaxPerSlot - stack.quantity, remaining);
				stack.quantity += canAdd;
				remaining -= canAdd;
			}
		}

		// Create new stacks if needed
		while (remaining > 0)
		{
			if (slots.Count >= maxSlots)
			{
				OnInventoryChanged?.Invoke();
				return false; // no space
			}
			int toCreate = Mathf.Min(MaxPerSlot, remaining);
			slots.Add(new InventoryStack { itemData = item, quantity = toCreate });
			remaining -= toCreate;
		}

		OnInventoryChanged?.Invoke();
		return true;
	}

	public bool RemoveQuantityAt(int slotIndex, int amount)
	{
		if (slotIndex < 0 || slotIndex >= slots.Count || amount <= 0)
			return false;
		var stack = slots[slotIndex];
		stack.quantity -= amount;
		if (stack.quantity <= 0)
		{
			slots.RemoveAt(slotIndex);
		}
		OnInventoryChanged?.Invoke();
		return true;
	}

	public void UseAt(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= slots.Count)
			return;
		var stack = slots[slotIndex];
		// 골드는 사용 불가. Consumable만 사용 가능이라고 가정
		bool canUse = stack.itemData.itemType == ItemType.Consumable;
		if (!canUse)
			return;
		// 간단히 1개 소모로 처리. 실제 효과는 추후 확장
		RemoveQuantityAt(slotIndex, 1);
	}
}


