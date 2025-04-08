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

        if (target != null)
        {
            agent.SetDestination(target.position);
        }
        else if (Vector2.Distance(GameManager.instance.player.transform.position, transform.position) > 10)
        {
            agent.SetDestination(GameManager.instance.player.transform.position);
        }

        // 방향 전환
        if (target != null)
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

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// using System.Linq;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.AI;

// public class PetMelee : EnemyBase, EnemyBase.IAttackable
// {
//     public float attackDelay = 1f;
//     public float damage = 10f;
//     public float GetAttackPower(){
//         return damage;
//     }
//     public void SetAttackPower(float value){
//         damage = value;
//     }

//     float attackTimer;
//     bool isTouchingPlayer;
//     bool isTouchingEnemy;
//     void FindNearestEnemy()
//     {
//         GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

//         if (enemies.Length == 0) {
//             target = null;
//             return;
//         }

//         GameObject nearestEnemy = enemies
//             .OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude)
//             .FirstOrDefault();

//         if (nearestEnemy != null) {
//             target = nearestEnemy.GetComponent<Rigidbody2D>();
//         }
//     }

//     protected override void Update()
//     {
//         if (!GameManager.instance.isLive || !isLive)
//             return;

//         // target이 없거나, 타겟이 비활성화된 경우 다시 찾음
//         if (target == null || !target.gameObject.activeSelf) {
//             FindNearestEnemy();
//         }

//         // 이동 처리
//         if (agent != null && !agent.enabled) {
//             agent.enabled = true;
//         }

//         if (target != null) {
//             agent.SetDestination(target.position);
//         } else if (Vector2.Distance(GameManager.instance.player.transform.position, transform.position) > 10) {
//             agent.SetDestination(GameManager.instance.player.transform.position);
//         }

//         // 방향 반전
//         if (target != null) {
//             spriter.flipX = target.position.x < transform.position.x;
//         }

//         // 공격 처리
//         Act();

//         if (attackTimer > 0) attackTimer -= Time.deltaTime;
//     }
//     // protected override void Update()
//     // {
//     //     if (!GameManager.instance.isLive || !isLive)
//     //         return;
//     //     if(target == null || !target.gameObject.activeSelf){
//     //         //target 탐색
//     //         GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
//     //         if(enemies.Length==0){
//     //             target = null;
//     //         }
//     //         GameObject nearestEnemy = enemies.OrderBy(enemy => (enemy.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
//     //         if(nearestEnemy != null){
//     //             target = nearestEnemy.GetComponent<Rigidbody2D>();
//     //         }
//     //     }
        

//     //     if (agent != null && !agent.enabled) {
//     //         agent.enabled = true;
//     //     }

//     //     if (target != null) {
//     //         agent.SetDestination(target.position);
//     //     } else if (Vector2.Distance(GameManager.instance.player.transform.position, transform.position) > 10) {
//     //         agent.SetDestination(GameManager.instance.player.transform.position);
//     //     }

//     //     spriter.flipX = target.position.x < transform.position.x;

//     //     Act(); // 하위 클래스에서 공격 로직 구현

//     //     if(attackTimer>0)attackTimer -= Time.deltaTime;
//     // }

//     protected override void Act()
//     {
        
//     }

//     protected override void OnEnable()
//     {
        
//         base.OnEnable();
//         if (agent != null)
//         {
//             agent.enabled = true;
//             agent.speed = speed;
//         }
//         isPet=true;
//         speed = 2;
//         target = null;
//     }

//     protected override void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.CompareTag("Enemy") && attackTimer <= 0f){
//             EnemyBase enemy = collision.GetComponent<EnemyBase>();
        
//             if (enemy != null) // Null 체크
//             {
//                 enemy.TakeDamage(damage);
//                 attackTimer = attackDelay;
//             }
//         }
//         return;
//     }

// }
