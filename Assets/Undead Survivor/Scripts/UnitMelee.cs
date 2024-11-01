using UnityEngine;

public class UnitMelee : MonoBehaviour
{
    public float attackRange = 1f; // 공격 범위
    public float attackCooldown = 2f; // 공격 쿨타임
    public float speed = 5f; // 이동 속도
    public int damage = 10; // 공격 데미지
    public Transform target; // 타겟
    private Rigidbody2D rb; // Rigidbody2D 컴포넌트
    private Scanner scanner; // Scanner 스크립트 참조
    private float attackTimer;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        scanner = GetComponent<Scanner>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 타겟을 스캐너에서 가져오기
        if (target == null)
        {
            target = scanner.nearestTarget;
        }

        if (target != null)
        {
            MoveTowardsTarget(); // 타겟에게 접근
            anim.SetBool("isMoving", true); // 이동 중임을 애니메이터에 알림

            // 공격 범위에 들어오면 공격 실행
            if (Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                Attack(); // 공격
                anim.SetBool("isMoving", false); // 이동 중이 아님을 애니메이터에 알림
            }      
        }
        else
        {
            anim.SetBool("isMoving", false); // 타겟이 없을 경우 이동 중이 아님
        }
    }

    void MoveTowardsTarget()
    {
        // 타겟 방향 계산
        Vector2 direction = (target.position - transform.position).normalized;
        // Rigidbody2D를 사용하여 이동
        rb.linearVelocity = direction * speed; 
    }

    void Attack()
    {
        if (attackTimer <= 0f) // 공격 쿨타임 체크
        {
            Vector2 direction = (target.position - transform.position).normalized;

            // 방향에 따라 애니메이션 클립 선택
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x)) // 위 또는 아래
            {
                if (direction.y > 0) // 위쪽
                {
                    anim.SetTrigger("Attack_Up");
                }
                else // 아래쪽
                {
                    anim.SetTrigger("Attack_Down");
                }
            }
            else // 좌우
            {
                if (direction.x > 0) // 오른쪽
                {
                    anim.SetTrigger("Attack_Front");
                }
                else // 왼쪽
                {
                    anim.SetTrigger("Attack_Front"); // 왼쪽도 오른쪽 애니메이션을 재사용
                    transform.localScale = new Vector3(-1, 1, 1); // 좌우 반전
                }
            }

            target.GetComponent<Enemy>().TakeDamage(damage); // 적에게 피해 입히기
            attackTimer = attackCooldown; // 쿨타임 초기화
            anim.SetBool("isMoving", false);
        }
        else
        {
            attackTimer -= Time.deltaTime; // 쿨타임 감소
            anim.SetTrigger("Attack_Complete");
        }
    }
    //change Eric
}
