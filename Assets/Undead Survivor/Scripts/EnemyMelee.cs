using UnityEngine;

public class EnemyMelee : EnemyBase, EnemyBase.IAttackable
{
    public float attackDelay = 1f;
    public float damage = 10f;
    public float GetAttackPower(){
        return damage;
    }
    public void SetAttackPower(float value){
        damage = value;
    }

    float attackTimer;
    bool isTouchingPlayer;

    protected override void Act()
    {
        if (isTouchingPlayer && attackTimer <= 0f)
        {
            Debug.Log(" Melee 공격 발동됨");
            anim.SetTrigger("AttackMelee");
            GameManager.instance.player.TakeDamage(damage);
            attackTimer = attackDelay;
        }

        attackTimer -= Time.deltaTime;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        canCaught=true;
        enemyIdx=0;
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
