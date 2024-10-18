using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float attackRange = 5f;
    public float attackRate = 1f; 
    public GameObject bulletPrefab; 
    private Transform target;

    void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        DetectTarget();
    }

    void DetectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                target = hit.transform;
                return;
            }
        }
        target = null;
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (target != null)
            {
                Attack();
            }
            yield return new WaitForSeconds(attackRate);
        }
    }

void Attack()
{
    if (target == null) return; // 타겟이 없으면 리턴

    GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    Vector2 direction = (target.position - transform.position).normalized;
    
    bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction); //총알회전
    bullet.GetComponent<Bullet>().Init(10f, 1, direction);
    AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 발사 사운드
}
}
