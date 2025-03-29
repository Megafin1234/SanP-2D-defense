using System.Collections;
using UnityEngine;

public class EnemySuicide : EnemyBase
{
    public float explodeRange = 1.5f;         // 폭발 범위
    public float explodeDamage = 50f;         // 폭발 데미지
    public float attackCooldown = 1.5f;       // 공격 간격

    float attackTimer;

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
            // 폭발 시작 - 상태 고정
            isLive = false;
            agent.enabled = false;
            rigid.linearVelocity = Vector2.zero;
            coll.enabled = false;

            anim.SetTrigger("AttackMagic");
            StartCoroutine(ExplodeAfterDelay(0.3f)); // 애니메이션과 싱크 조정
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
        if (!gameObject.activeSelf) return;

        // 플레이어에게 범위 피해
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explodeRange);
        foreach (var t in targets)
        {
            if (t.CompareTag("Player"))
            {
                t.GetComponent<Player>()?.TakeDamage(explodeDamage);
            }
        }

        // 리소스 처리
        GameManager.instance.kill++;
        GameManager.instance.coin += (3 + GameManager.instance.DayCount * 2);
        GameManager.instance.GetExp();
        WaveSpawner.instance.currentWaveKillCount++;

        if (GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);

        Dead(); // 오브젝트 비활성화
    }
}
