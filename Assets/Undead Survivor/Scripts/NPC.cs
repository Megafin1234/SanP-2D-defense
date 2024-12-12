using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject interactionPanel; 
    public Transform player;
    public float interactionRadius = 2f; 

    private bool isPlayerNearby = false;

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
