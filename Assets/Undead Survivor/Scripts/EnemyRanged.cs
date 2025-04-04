using UnityEngine;

public class EnemyRanged : EnemyBase, EnemyBase.IAttackable
{
    public float attackRange = 5f;
    public float attackDelay = 2f;
    public float bulletSpeed = 12f;
    public float damage = 8f;
    public float GetAttackPower(){
        return damage;
    }
    public void SetAttackPower(float value){
        damage = value;
    }

    float attackTimer;

    protected override void Act()
    {
        if (!isLive) return;

        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= attackRange && attackTimer <= 0f)
        {
            anim.SetTrigger("AttackMagic");
            Shoot();
            attackTimer = attackDelay;
        }

        attackTimer -= Time.deltaTime;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        canCaught=true;
        enemyIdx=1;
    }

    void Shoot()
    {
        if (target == null)
            return;

        Vector3 dir = (GameManager.instance.player.transform.position - transform.position).normalized;


        GameObject bulletObj = GameManager.instance.pool.Get(3);
        bulletObj.transform.position = transform.position;
        bulletObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.owner = Bullet.BulletOwner.Enemy;
        bullet.Init(damage, 0, dir); // 관통 없음

    }

}

