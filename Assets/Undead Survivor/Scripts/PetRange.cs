using System.Linq;
using UnityEngine;

public class PetRange : EnemyBase, EnemyBase.IAttackable
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
    protected override void Update()
    {
        if (!GameManager.instance.isLive || !isLive)
            return;
        
        //target 탐색
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if(enemies.Length==0){
            target = null;
        }
        GameObject nearestEnemy = enemies.OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        if(nearestEnemy != null){
            target = nearestEnemy.GetComponent<Rigidbody2D>();
        }

        if (agent.enabled && target != null){
            agent.SetDestination(target.position);
        }
        else if (agent.enabled && Vector2.Distance(GameManager.instance.player.transform.position, transform.position) > 10){
            agent.SetDestination(GameManager.instance.player.transform.position);
        }

        if(target!=null)spriter.flipX = target.position.x < transform.position.x;

        Act(); // 하위 클래스에서 공격 로직 구현
    }

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
        isPet=true;
        speed = 2;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        return;
    }

    void Shoot()
    {
        if (target == null)
            return;

        //여기 재설정.
        Vector3 dir = (target.transform.position - transform.position).normalized;


        GameObject bulletObj = GameManager.instance.pool.Get(3);
        bulletObj.transform.position = transform.position;
        bulletObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.owner = Bullet.BulletOwner.Player;
        bullet.Init(damage, 0, dir); // 관통 없음

    }
}
