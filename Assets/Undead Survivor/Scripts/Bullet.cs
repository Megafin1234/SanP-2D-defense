using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    public enum BulletOwner
    {
        Player,
        Enemy,
        Pet
    }

    public BulletOwner owner = BulletOwner.Player; // 기본은 플레이어 탄환
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir, float customSpeed = -1f)  //탄속은 bulletComp.Init(rangedDamage, 0, dir, 6f); 이걸 다른 스크립트에 추가해 조절하면됨
    {
        this.damage = damage;
        this.per = per;

        float finalSpeed = (customSpeed > 0) ? customSpeed : 15f;

        if (per >= 0)
            rigid.linearVelocity = dir * finalSpeed;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == BulletOwner.Player && collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (per != -100)
                {
                    per--;
                    if (per < 0)
                    {
                        rigid.linearVelocity = Vector2.zero;
                        gameObject.SetActive(false);
                    }
                }
            }
        }
        else if (owner == BulletOwner.Enemy && collision.CompareTag("Pet")){
            gameObject.SetActive(false);
        }
        else if (owner == BulletOwner.Enemy && collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                gameObject.SetActive(false); // 단발
            }
        }

    }




    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;

        gameObject.SetActive(false);
    }
}
