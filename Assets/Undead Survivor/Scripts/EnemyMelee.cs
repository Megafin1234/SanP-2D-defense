using UnityEngine;

public class EnemyMelee : EnemyBase
{
    public float attackDelay = 1f;
    public float damage = 10f;

    float attackTimer;
    bool isTouchingPlayer;

    protected override void Act()
    {
        if (isTouchingPlayer && attackTimer <= 0f)
        {
            anim.SetTrigger("AttackMelee");
            GameManager.instance.health -= damage;
            attackTimer = attackDelay;
        }

        attackTimer -= Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLive) return;
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }
}
