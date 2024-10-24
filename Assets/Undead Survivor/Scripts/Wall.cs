using UnityEngine;

public class Wall : MonoBehaviour
{
    public float health = 100f;
    public float damagePerHit = 10f; // 에너미가 벽을 부술 때 감소하는 체력

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 벽의 체력을 감소시키고 체크
            health -= damagePerHit;
            if (health <= 0)
            {
                DestroyWall();
            }
        }
    }

    void DestroyWall()
    {
        // 벽을 파괴하는 로직
        gameObject.SetActive(false);
        // 또는 Destroy(gameObject);
    }
}
