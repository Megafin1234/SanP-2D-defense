using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public skp_item_temp itemData;  // 드랍 아이템 정보
    public int quantity = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        bool success = InventoryManager.Instance != null && InventoryManager.Instance.AddItem(itemData, quantity);
        if (success)
        {
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("인벤토리에 공간이 없습니다.");
        }
    }
}
