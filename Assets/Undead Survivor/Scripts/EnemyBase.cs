using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int spriteType;
    public bool isInvincible = false;

    public bool isLive;
    public NavMeshAgent agent;
    public RuntimeAnimatorController[] animCon;

    public interface IAttackable
{
    float GetAttackPower();
    void SetAttackPower(float value);
}

    protected Rigidbody2D rigid;
    protected Collider2D coll;
    protected Animator anim;
    protected SpriteRenderer spriter;

    protected Rigidbody2D target;

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void OnEnable()
    {
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;

        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = speed;
        }
    }

    protected virtual void Update()
    {
        if (!GameManager.instance.isLive || !isLive)
            return;

        if (agent.enabled)
            agent.SetDestination(target.position);

        spriter.flipX = target.position.x < transform.position.x;

        Act(); // 하위 클래스에서 공격 로직 구현
    }

    public virtual void Init(SpawnData data)
    {
        spriteType = data.spriteType;
        anim.runtimeAnimatorController = animCon[spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;

        if (agent != null)
            agent.speed = speed;
    }

public virtual void TakeDamage(float damage)
{
    if (isInvincible || !isLive)
        return;

    // 자폭형은 TakeDamage로 죽지 않도록 막음
    if (this is EnemySuicide suicideEnemy && suicideEnemy.IsExploding())
        return;

    health -= damage;

    if (health > 0)
    {
        anim.SetTrigger("Hit");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
    }
    else
    {
        isLive = false;
        coll.enabled = false;
        rigid.simulated = false;
        spriter.sortingOrder = 1;
        anim.SetBool("Dead", true);

        GameManager.instance.kill++;
        GameManager.instance.coin += (3 + GameManager.instance.DayCount * 2);
        GameManager.instance.GetExp();
        WaveSpawner.instance.currentWaveKillCount++;

        if (GameManager.instance.isLive)
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
    }
}


    public void Caught()
    {
        if (agent != null)
            agent.enabled = false;

        if (Random.Range(0, 5) >= 3)
        {
            GameManager.instance.party.Add(this);
        }

        if (agent != null)
            agent.enabled = true;
    }



    protected IEnumerator KnockBack(Vector3 source)
    {
        if (agent != null)
            agent.enabled = false;

        Vector3 dir = transform.position - source;
        rigid.AddForce(dir.normalized * 2, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        if (agent != null)
            agent.enabled = true;
    }

protected virtual void OnTriggerEnter2D(Collider2D collision)
{
    if (!isLive) return;

    if (collision.CompareTag("Bullet"))
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        
        if (bullet != null && bullet.owner == Bullet.BulletOwner.Player)
        {
            TakeDamage(bullet.damage);
            StartCoroutine(KnockBack(GameManager.instance.player.transform.position));
        }
    }

    if (collision.CompareTag("Catch"))
    {
        float dmg = collision.GetComponent<CatchTool>().damage;
        TakeDamage(dmg);
        StartCoroutine(KnockBack(GameManager.instance.player.transform.position));
        Caught();
    }
}


    public virtual void Dead()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Act(); // 공격 등 고유 행동을 하위 클래스에서 구현
}
