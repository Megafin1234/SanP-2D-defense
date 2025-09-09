using UnityEngine;

public class ItemMagnet : MonoBehaviour
{
	public float attractRadius = 3f;
	public float pullSpeed = 10f;
	public LayerMask itemLayer;

	void FixedUpdate()
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attractRadius, itemLayer);
		for (int i = 0; i < hits.Length; i++)
		{
			var rb = hits[i].attachedRigidbody;
			if (rb != null)
			{
				Vector2 dir = (transform.position - hits[i].transform.position).normalized;
				rb.linearVelocity = dir * pullSpeed;
			}
			else
			{
				hits[i].transform.position = Vector2.MoveTowards(hits[i].transform.position, transform.position, pullSpeed * Time.fixedDeltaTime);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attractRadius);
	}
}


