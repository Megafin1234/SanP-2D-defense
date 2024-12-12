using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject interactionPanel; // 판넬 참조
    public Transform player; // 플레이어 참조
    public float interactionRadius = 3f; // NPC 반경

    private bool isPlayerNearby = false;

    void Update()
    {
        // 플레이어와 NPC 간의 거리 계산
        float distance = Vector3.Distance(player.position, transform.position);

        // 플레이어가 반경 안에 들어오면 판넬 활성화
        if (distance <= interactionRadius)
        {
            if (!isPlayerNearby)
            {
                ShowInteractionPanel();
                isPlayerNearby = true;
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                HideInteractionPanel();
                isPlayerNearby = false;
            }
        }
    }

    void ShowInteractionPanel()
    {
        interactionPanel.SetActive(true);
    }

    void HideInteractionPanel()
    {
        interactionPanel.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        // NPC 반경 시각화 (디버깅용)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
