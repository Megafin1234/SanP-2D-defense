using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PetRange : EnemyBase, EnemyBase.IAttackable
{
    public float attackRange = 5f;
    public float attackDelay = 2f;
    public float bulletSpeed = 12f;
    public float damage = 8f;

    public bool returnToPlayer = false;

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

        float distPlayer = Vector2.Distance(GameManager.instance.player.transform.position, transform.position);

        if(distPlayer > 5 && returnToPlayer){
            agent.SetDestination(GameManager.instance.player.transform.position);
            spriter.flipX = GameManager.instance.player.transform.position.x < transform.position.x;
            if(distPlayer<=5)returnToPlayer=false;
        }
        else if (target != null)
        {
            if(distPlayer < 10)
            {
                // 타겟 추적 시 타겟 방향으로 이동하고, 스프라이트도 타겟을 바라보도록 설정
                agent.SetDestination(target.position);
                spriter.flipX = target.position.x < transform.position.x;
            }
            else 
            {
                // 플레이어와의 거리가 10 이상이면 플레이어에게 돌아가도록 함
                returnToPlayer = true;
                agent.SetDestination(GameManager.instance.player.transform.position);
                // 이때는 플레이어를 바라보도록
                spriter.flipX = GameManager.instance.player.transform.position.x < transform.position.x;
            }
        }
        else if (distPlayer > 5)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
            spriter.flipX = GameManager.instance.player.transform.position.x < transform.position.x;
        }

        if(target!=null && !returnToPlayer)spriter.flipX = target.position.x < transform.position.x;

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
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)//근k532
        // \
        // 4323+2+3262666666666666666666+2접펫은 이속 3
        {
            agent.speed = 2.5f;
        }
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
