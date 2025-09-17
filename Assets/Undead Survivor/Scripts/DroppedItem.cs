using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemSO itemData;  // 드랍 아이템 정보
    public int quantity = 1;
    public Sprite sprite;    // 드랍 아이템의 스프라이트
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 스프라이트가 설정되어 있으면 적용
        if (sprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

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
