using UnityEngine;
using System.Collections;

public class EnemyBossVoid : EnemyBase, EnemyBase.IAttackable
{
    public float meleeDamage = 15f;
    public float rangedDamage = 5f;

    public float attackDelay = 2f;

    private float meleeTimer;
    private bool isTouchingPlayer;
    private bool phase2Activated = false;


    private float bulletCycleTimer;
    private float[] patternDelays = new float[] { 1.5f, 4f, 2f,5f }; 
    private int bulletPatternIndex = 0;
    


    public float GetAttackPower() => meleeDamage;
    public void SetAttackPower(float value) => meleeDamage = value;

    public virtual void Init(SpawnData data)
    {
        spriteType = data.spriteType;
        anim.runtimeAnimatorController = animCon[spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        if (agent != null)
            agent.speed = speed;
        if (data.isBoss)
        {
            transform.localScale *= 3f;
            spriter.sortingOrder = 1;
        }
    }
    protected override void Act()
    {
        if (!isLive) return;

        // 체력 50% 이하일 때 페이즈 2 진입
        if (!phase2Activated && health <= maxHealth * 0.5f)
        {
            phase2Activated = true;
            EnterPhase2();
        }

        // 근접 공격
        if (isTouchingPlayer && meleeTimer <= 0f)
        {
            anim.SetTrigger("AttackMelee");
            GameManager.instance.player.TakeDamage(meleeDamage);
            meleeTimer = attackDelay;
        }


        if (bulletCycleTimer <= 0f)
        {
            switch (bulletPatternIndex)
            {
                case 0:
                    FireBurstPattern(); break;
                case 1:
                    StartCoroutine(FireWhirlPattern()); ; break;
                case 2:
                    StartCoroutine(FireMultiShotPattern()); break;
                case 3:
                    StartCoroutine(FireWhirlPattern_Powered()); ; break;
            }

            bulletCycleTimer = patternDelays[bulletPatternIndex];
            bulletPatternIndex = Random.Range(0, 3);
        }

        meleeTimer -= Time.deltaTime;
        bulletCycleTimer -= Time.deltaTime;
    }

    void EnterPhase2()
    {
        Debug.Log("보스 페이즈 2 진입");
        meleeDamage += 20f;
        rangedDamage += 5f;
        StartCoroutine(FireWhirlPattern_Powered());
        bulletPatternIndex = Random.Range(0, 4);
    }
    void FireBurstPattern()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            (Vector3.up + Vector3.right).normalized,
            (Vector3.up + Vector3.left).normalized,
            (Vector3.down + Vector3.right).normalized,
            (Vector3.down + Vector3.left).normalized,
        };

        foreach (var dir in directions)
            FireBullet(dir,8f);
    }
    IEnumerator FireWhirlPattern()
    {
        float angleStep = 360f / 8f; 
        float angleOffset = 0f;

        for (int i = 0; i < 7; i++) 
        {
            float angle = angleOffset;

            for (int j = 0; j < 8; j++)
            {
                float rad = angle * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

                GameObject bullet = GameManager.instance.pool.Get(6);
                bullet.transform.position = transform.position;
                bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                Bullet bulletComp = bullet.GetComponent<Bullet>();
                bulletComp.owner = Bullet.BulletOwner.Enemy;
                bulletComp.Init(rangedDamage, 0, dir,6f);

                angle += angleStep;
            }

            angleOffset -= 10f; // 반시계 방향 회전
            yield return new WaitForSeconds(0.03f); // 탄 간 간격
        }
    }
    IEnumerator FireWhirlPattern_Powered()   //바로 위 코드 수치만 조절한한 복사본임
{
    float angleStep = 360f / 8f; 
    float angleOffset = 0f;

    for (int i = 0; i < 15; i++) 
    {
        float angle = angleOffset;

        for (int j = 0; j < 8; j++)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

            GameObject bullet = GameManager.instance.pool.Get(3);
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

            Bullet bulletComp = bullet.GetComponent<Bullet>();
            bulletComp.owner = Bullet.BulletOwner.Enemy;
            bulletComp.Init(rangedDamage + 1f, 0, dir, 7f);

            angle += angleStep;
        }

        angleOffset -= 10f;
        yield return new WaitForSeconds(0.03f); 
    }
}


    IEnumerator FireMultiShotPattern()
    {
        for (int i = 0; i < 3; i++)
        {
            FireBurstPattern();
            yield return new WaitForSeconds(0.2f);
        }
    }

    // 공통 불렛 발사
    void FireBullet(Vector3 dir,float speed = 10f)
    {
        GameObject bullet = GameManager.instance.pool.Get(7);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        Bullet bulletComp = bullet.GetComponent<Bullet>();
        bulletComp.owner = Bullet.BulletOwner.Enemy;
        bulletComp.Init(rangedDamage, 0, dir, speed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLive) return;
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        canCaught = false;
        enemyIdx = 99;
        meleeTimer = 0f;
        bulletCycleTimer = 2f; 
    }
}
