using UnityEngine;
using UnityEngine.UI;

public class InventoryFixedUI : MonoBehaviour
{
	[Tooltip("12개 슬롯 버튼(아이콘 이미지와 수량 텍스트가 자식에 있어야 함)")]
	public Button[] slotButtons = new Button[12];

	[Tooltip("빈 슬롯일 때 아이콘을 숨길지 여부")]
	public bool hideEmptyIcon = true;

	void OnEnable()
	{
		Subscribe();
		Refresh();
	}

	void OnDisable()
	{
		Unsubscribe();
	}

	void Subscribe()
	{
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.OnInventoryChanged -= Refresh;
			InventoryManager.Instance.OnInventoryChanged += Refresh;
		}
	}

	void Unsubscribe()
	{
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.OnInventoryChanged -= Refresh;
		}
	}

	public void Refresh()
	{
		if (slotButtons == null || slotButtons.Length == 0)
			return;
		var inv = InventoryManager.Instance;
		for (int i = 0; i < slotButtons.Length; i++)
		{
			var btn = slotButtons[i];
			if (btn == null) continue;
			Image icon = btn.GetComponentInChildren<Image>(true);
			Text qty = btn.GetComponentInChildren<Text>(true);

			if (inv != null && i < inv.slots.Count)
			{
				var stack = inv.slots[i];
				if (icon != null)
				{
					icon.sprite = stack.itemData.icon;
					icon.enabled = true;
				}
				if (qty != null)
				{
					qty.text = stack.quantity.ToString();
				}
			}
			else
			{
				if (icon != null)
				{
					icon.enabled = !hideEmptyIcon;
					if (!hideEmptyIcon) icon.sprite = null;
				}
				if (qty != null)
				{
					qty.text = "";
				}
			}
		}
	}
}


