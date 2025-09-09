using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
	public Image icon;
	public Text quantityText;
	public Button useButton;
	public Button dropButton;

	private int slotIndex;

	public void Bind(int index)
	{
		slotIndex = index;
		var stack = InventoryManager.Instance.slots[index];
		icon.sprite = stack.itemData.icon;
		quantityText.text = stack.quantity.ToString();
		bool canUse = stack.itemData.itemType == ItemType.Consumable;
		useButton.interactable = canUse;
		useButton.onClick.RemoveAllListeners();
		useButton.onClick.AddListener(OnClickUse);
		dropButton.onClick.RemoveAllListeners();
		dropButton.onClick.AddListener(OnClickDrop);
	}

	void OnClickUse()
	{
		InventoryManager.Instance.UseAt(slotIndex);
	}

	void OnClickDrop()
	{
		// 삭제만 하고 월드에 드랍하지 않음
		InventoryManager.Instance.RemoveQuantityAt(slotIndex, InventoryManager.Instance.slots[slotIndex].quantity);
	}
}


