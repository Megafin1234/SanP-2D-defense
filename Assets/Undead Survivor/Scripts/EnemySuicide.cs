using System.Collections;
using UnityEngine;

public class EnemySuicide : EnemyBase
{
    public float explodeRange = 1.5f;               // 폭발 범위
    public float explodeDamage = 50f;               // 폭발 데미지
    public float attackCooldown = 1.5f;             // 공격 간격
    public GameObject ExplosionRangeIndicator;         // 범위 원 프리팹 (인스펙터에서 할당)

    private GameObject indicatorInstance;           // 생성된 원 이펙트 인스턴스
    float attackTimer;
    private bool exploded = false;
    public bool IsExploding()
    {
        return exploded;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        attackTimer = attackCooldown;
    }

    protected override void Act()
    {
        if (!isLive || GameManager.instance.player == null)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= explodeRange && attackTimer <= 0f)
        {
            // 상태 고정
            isLive = false;
            agent.enabled = false;
            rigid.linearVelocity = Vector2.zero;
            coll.enabled = false;

            if (ExplosionRangeIndicator)
            {
                indicatorInstance = Instantiate(ExplosionRangeIndicator, transform.position, Quaternion.identity);
                indicatorInstance.transform.localScale = Vector3.one * explodeRange * 2f;
            }

            anim.SetTrigger("AttackMagic");
            StartCoroutine(ExplodeAfterDelay(1.2f));
            attackTimer = attackCooldown;
        }

        attackTimer -= Time.deltaTime;
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EndSuicide();
    } 

    public void EndSuicide()
    {
        if (exploded) return;
        exploded = true;

        Debug.Log("💥 자폭 실행됨: " + gameObject.name);

        // 폭발 피해
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explodeRange);
        foreach (var t in targets)
        {
            if (t.CompareTag("Player"))
                t.GetComponent<Player>()?.TakeDamage(explodeDamage);
        }

        // 이펙트 제거
        if (indicatorInstance)
            Destroy(indicatorInstance);

        // 킬 카운트 딱 1번만 증가
        GameManager.instance.kill++;
        GameManager.instance.coin += (3 + GameManager.instance.DayCount * 2);
        GameManager.instance.GetExp();
        WaveSpawner.instance.currentWaveKillCount++;

        Debug.Log("☠️ 웨이브 킬 카운트 증가: " + WaveSpawner.instance.currentWaveKillCount);

        if (GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        Dead();
    }
}

