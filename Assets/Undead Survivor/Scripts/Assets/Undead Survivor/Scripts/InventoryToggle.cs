using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			if (GameManager.instance.inventoryPanel != null && GameManager.instance.inventoryPanel.activeSelf)
			{
				GameManager.instance.CloseInventory();
			}
			else
			{
				GameManager.instance.OpenInventory();
			}
		}
	}
}


