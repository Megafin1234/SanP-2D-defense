using UnityEngine;

public class UnitMelee : MonoBehaviour
{
    public float attackRange = 1f; 
    public float attackCooldown = 2f; 
    public float speed = 5f;
    public int damage = 10; 
    public Transform target; 
    private Rigidbody2D rb; 
    private Scanner scanner; 
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
        if (target == null)
        {
            target = scanner.nearestTarget;
        }

        if (target != null)
        {
            MoveTowardsTarget(); 
            anim.SetBool("isMoving", true); 

            if (Vector2.Distance(transform.position, target.position) <= attackRange)
            {
                Attack(); 
                anim.SetBool("isMoving", false); 
            }      
        }
        else
        {
            anim.SetBool("isMoving", false); 
        }
    }

    void MoveTowardsTarget()
    {
      
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed; 
    }

    void Attack()
    {
        if (attackTimer <= 0f) 
        {
            Vector2 direction = (target.position - transform.position).normalized;

          
            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x)) 
            {
                if (direction.y > 0) 
                {
                    anim.SetTrigger("Attack_Up");
                }
                else 
                {
                    anim.SetTrigger("Attack_Down");
                }
            }
            else
            {
                if (direction.x > 0) 
                {
                    anim.SetTrigger("Attack_Front");
                }
                else 
                {
                    anim.SetTrigger("Attack_Front"); 
                    transform.localScale = new Vector3(-1, 1, 1); // 좌우 반전
                }
            }

            target.GetComponent<Enemy>().TakeDamage(damage); 
            attackTimer = attackCooldown; 
            anim.SetBool("isMoving", false);
        }
        else
        {
            attackTimer -= Time.deltaTime; 
            anim.SetTrigger("Attack_Complete");
        }
    } //main changes
}
