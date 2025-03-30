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

    public float healAmount = 50f;
    [Header("버프 아이콘")]
    public GameObject buffHealIconPrefab;
    public GameObject buffSpeedIconPrefab;
    public GameObject buffAttackIconPrefab;

    HashSet<EnemyBase> buffingTargets = new HashSet<EnemyBase>();


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

        if (enemy != null && enemy.isLive && !buffingTargets.Contains(enemy))
        {
            StartCoroutine(ApplyBuff(enemy));
            buffingTargets.Add(enemy); // 중복 방지 위해
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
        if (enableAttackBuff && enemy is IAttackable attackable){
            originalDamage = attackable.GetAttackPower();
            attackable.SetAttackPower(originalDamage * damageMultiplier);
            hasAttackable = true;
                if (buffAttackIconPrefab && enemy.activeAttackIcon == null){
                    enemy.activeAttackIcon = Instantiate(buffAttackIconPrefab, enemy.transform);
                    enemy.activeAttackIcon.transform.localPosition = new Vector3(0.4f, 1.5f, 0); // 오른쪽 위
                }
        }

        // 속도 버프
        if (enableSpeedBuff)
            enemy.agent.speed = originalSpeed * speedMultiplier;
                if (buffSpeedIconPrefab && enemy.activeSpeedIcon == null){
                    enemy.activeSpeedIcon = Instantiate(buffSpeedIconPrefab, enemy.transform);
                    enemy.activeSpeedIcon.transform.localPosition = new Vector3(-0.4f, 1.5f, 0); // 왼쪽 위
                }

        // 힐 버프
        if (enableHealBuff && enemy.health < enemy.maxHealth)
            enemy.health = Mathf.Min(enemy.maxHealth, enemy.health + healAmount);
                if (buffHealIconPrefab){
                    GameObject healIcon = Instantiate(buffHealIconPrefab, enemy.transform);
                    healIcon.transform.localPosition = new Vector3(0, 1.5f, 0); // 정중앙
                    Destroy(healIcon, 1.0f); // 1초 후 사라짐
                }

        // 무적 버프 시작
        if (enableInvincibilityBuff)
            enemy.isInvincible = true;

        yield return new WaitForSeconds(buffDuration);

        // 버프끝
        if (enemy != null && enemy.isLive)
        {
            if (enableSpeedBuff)
                enemy.agent.speed = originalSpeed;
                        if (enemy.activeSpeedIcon != null)
                        {
                            Destroy(enemy.activeSpeedIcon);
                            enemy.activeSpeedIcon = null;
                        }

            if (hasAttackable)
                (enemy as IAttackable).SetAttackPower(originalDamage);
                        if (enemy.activeAttackIcon != null)
                        {
                            Destroy(enemy.activeAttackIcon);
                            enemy.activeAttackIcon = null;
                        }

            if (enableInvincibilityBuff)
                enemy.isInvincible = false;
        }
        buffingTargets.Remove(enemy);
    }

}
