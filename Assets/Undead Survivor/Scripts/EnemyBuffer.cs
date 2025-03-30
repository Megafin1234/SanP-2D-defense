using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffer : EnemyBase
{
    [Header("버프 설정")]
    public float buffRange = 4f;
    public float buffInterval = 5f;
    public float buffDuration = 3f;
    public float speedMultiplier = 1.5f;
    public float damageMultiplier = 1.5f;
    public bool enableSpeedBuff = true;
    public bool enableAttackBuff = true;
    public bool enableHealBuff = true;
    public bool enableInvincibilityBuff = false;

    public float healAmount = 5f;


    float buffTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        buffTimer = buffInterval;
    }

    protected override void Act()
    {
        if (!isLive) return;

        buffTimer -= Time.deltaTime;

        if (buffTimer <= 0f)
        {
            ApplyBuffToAllies();
            buffTimer = buffInterval;
        }
    }

    void ApplyBuffToAllies()
    {
        Collider2D[] allies = Physics2D.OverlapCircleAll(transform.position, buffRange);

        bool didBuff = false;

        foreach (Collider2D ally in allies)
        {
            if (ally.gameObject == this.gameObject) continue;

            EnemyBase enemy = ally.GetComponent<EnemyBase>();
            if (enemy != null && enemy.isLive)
            {
                StartCoroutine(ApplyBuff(enemy));
                didBuff = true;
            }
        }

        if (didBuff)
            anim.SetTrigger("AttackMagic"); 
    }


    IEnumerator ApplyBuff(EnemyBase enemy)
    {
        float originalSpeed = enemy.agent.speed;
        float originalDamage = 0f;
        bool hasAttackable = false;

        // 공격력 버프
        if (enableAttackBuff && enemy is IAttackable attackable)
        {
            originalDamage = attackable.GetAttackPower();
            attackable.SetAttackPower(originalDamage * damageMultiplier);
            hasAttackable = true;
        }

        // 속도 버프
        if (enableSpeedBuff)
            enemy.agent.speed = originalSpeed * speedMultiplier;

        // 힐 버프
        if (enableHealBuff && enemy.health < enemy.maxHealth)
            enemy.health = Mathf.Min(enemy.maxHealth, enemy.health + healAmount);

        // 무적 버프 시작
        if (enableInvincibilityBuff)
            enemy.isInvincible = true;

        yield return new WaitForSeconds(buffDuration);

        // 버프끝끝
        if (enemy != null && enemy.isLive)
        {
            if (enableSpeedBuff)
                enemy.agent.speed = originalSpeed;

            if (hasAttackable)
                (enemy as IAttackable).SetAttackPower(originalDamage);

            if (enableInvincibilityBuff)
                enemy.isInvincible = false;
        }
    }

}
