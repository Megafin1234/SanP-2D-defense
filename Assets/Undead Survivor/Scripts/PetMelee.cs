///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PetMelee : EnemyBase, EnemyBase.IAttackable
{
    public float attackDelay = 1f;
    public float damage = 10f;
    private float attackTimer;
    public float GetAttackPower() => damage;
    public void SetAttackPower(float value) => damage = value;
    public bool returnToPlayer = false;
    protected override void Update()
    {
        if (!GameManager.instance.isLive || !isLive)
            return;
        // 타겟이 없거나 죽었으면 새로운 적을 찾음
        if (target == null || !target.gameObject.activeSelf)
        {
            FindNearestEnemy();
        }
        // 이동 로직
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
        float distPlayer = Vector2.Distance(GameManager.instance.player.transform.position, transform.position);
        if(distPlayer > 3 && returnToPlayer){
            agent.SetDestination(GameManager.instance.player.transform.position);
            spriter.flipX = GameManager.instance.player.transform.position.x < transform.position.x;
            if(distPlayer<=3)returnToPlayer=false;
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
        // 방향 전환
        if (target != null && !returnToPlayer)
        {
            spriter.flipX = target.position.x < transform.position.x;
        }
        // 공격 로직 실행
        Act();
        if (attackTimer > 0) attackTimer -= Time.deltaTime;
    }
    void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            target = null;
            return;         
        }
        GameObject nearestEnemy = enemies
            .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
        if (nearestEnemy != null)
        {
            target = nearestEnemy.GetComponent<Rigidbody2D>();
        }
    }
    protected override void Act()
    {
        if (target == null) return;

        if (attackTimer <= 0f && Vector2.Distance(transform.position, target.position) <= 1.5f)
        {
            EnemyBase enemy = target.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                attackTimer = attackDelay;
            }
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        isPet = true;
        speed = 2;
        target = null;
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)//근접펫은 이속 3
        {
            agent.speed = 3.5f;
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && attackTimer <= 0f)
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                attackTimer = attackDelay;
            }
        }
    }
}