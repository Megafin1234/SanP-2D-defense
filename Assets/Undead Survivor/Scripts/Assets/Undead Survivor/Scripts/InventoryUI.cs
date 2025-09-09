using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
	public GameObject panelRoot;
	public Transform slotsParent;
	public GameObject slotPrefab;

	private readonly List<InventorySlotUI> activeSlots = new List<InventorySlotUI>();

	void Awake()
	{
		if (InventoryManager.Instance != null)
		{
			InventoryManager.Instance.OnInventoryChanged += Refresh;
		}
	}

	void OnEnable()
	{
		Refresh();
	}

	public void Refresh()
	{
		if (InventoryManager.Instance == null) return;
		for (int i = 0; i < activeSlots.Count; i++)
		{
			Destroy(activeSlots[i].gameObject);
		}
		activeSlots.Clear();

		for (int i = 0; i < InventoryManager.Instance.slots.Count; i++)
		{
			var go = Instantiate(slotPrefab, slotsParent);
			var ui = go.GetComponent<InventorySlotUI>();
			ui.Bind(i);
			activeSlots.Add(ui);
		}
	}
}


