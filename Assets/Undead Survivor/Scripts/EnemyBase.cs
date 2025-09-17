using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Linq;

public abstract class EnemyBase : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public int spriteType;
    public int enemyType;
    public bool isInvincible = false;
    [SerializeField] private GameObject dropItemPrefab; // 드랍 프리팹 연결
    public EnemySO enemySO; // Init()에서 세팅됨
    public Sprite sprite;

    public bool isLive;

    public bool isPet;
    public bool isBoss;
    public bool canCaught = false;

    public int enemyIdx;//이걸로 적 소환, 펫 소환 다 관리함.
    public NavMeshAgent agent;
    public RuntimeAnimatorController[] animCon;
    public GameObject activeSpeedIcon;
    public GameObject activeAttackIcon;
    public interface IAttackable
{
    float GetAttackPower();
    void SetAttackPower(float value);
}
    protected Rigidbody2D rigid;
    protected Collider2D coll;
    protected Animator anim;
    protected SpriteRenderer spriter;
    public Rigidbody2D target;
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
        isPet=false;
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
        if (agent.enabled){
            agent.SetDestination(target.position);
        }
        spriter.flipX = target.position.x < transform.position.x;
        Act(); // 하위 클래스에서 공격 로직 구현
    }
    public virtual void Init(EnemySO myData)
    {
        anim.runtimeAnimatorController = myData.animCon;
        speed = myData.speed;
        maxHealth = myData.health;
        health = myData.health;
        enemySO = myData; //////////
        if (agent != null)
            agent.speed = speed;
        if (myData.isBoss)
        {
            transform.localScale *= 3f;
            spriter.sortingOrder = 1;
        }
        enemyIdx = myData.enemyIdx;
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
        if (data.isBoss)
        {
            transform.localScale *= 3f;
            spriter.sortingOrder = 1;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isInvincible || !isLive || isPet)
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
            Debug.Log($"{name} :: 사망 조건 진입");
            isLive = false;
            DropItem();
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.ResetTrigger("AttackMelee");
            anim.ResetTrigger("AttackMagic");
            anim.ResetTrigger("Hit");
            anim.SetBool("Dead", true);
            anim.Play("Dead", 0, 0f); //버퍼 죽음 애니메이션 문제로 강제재생. 없어도 되야함
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
        if (!GameManager.instance.canCatch || isBoss) return;//보스는 일단  포획 안되게 해뒀음. 이번 스테이지에서 이미 잡았어도 포획 안됨.
        
        if (agent != null)
            agent.enabled = false;

        if (TutorialManager.isTutorial || Random.Range(0, 5) >= 3)
        {
        GameManager.instance.GetPet(enemyIdx, transform);
        /*WaveSpawner.instance.currentWaveKillCount++;
        GameManager.instance.GetExp(); */
        Dead();
        }

        if (Random.Range(0, 5) >= 3)//포획 성공시
        {
            GameManager.instance.GetPet(enemyIdx, transform);
            WaveSpawner.instance.currentWaveKillCount++;
            GameManager.instance.GetExp();
            Dead();//나는 죽는다. 
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
        rigid.linearVelocity = Vector2.zero;
    }
    
    public virtual void DropItem()
    {
        if (enemySO == null || enemySO.possibleDrops == null || enemySO.possibleDrops.Count == 0)
            return;

        int dropIndex = Random.Range(0, enemySO.possibleDrops.Count);/////////드랍될 아이템의 정보를 enemySO 내부 배열 중에서 랜덤선택
        ItemSO itemToDrop = enemySO.possibleDrops[dropIndex]; 

        Vector3 dropPos = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), 0.2f, 0f);
        GameObject drop = Instantiate(dropItemPrefab, dropPos, Quaternion.identity);

        DroppedItem dropComp = drop.GetComponent<DroppedItem>();
        dropComp.itemData = itemToDrop;
        dropComp.quantity = 1;
        dropComp.sprite = enemySO.possibleDrops[dropIndex].icon; 
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
            float dmg = 1f;
            TakeDamage(dmg);
            StartCoroutine(KnockBack(GameManager.instance.player.transform.position));
            if (canCaught) Caught();
        }
    }
    public virtual void Dead()
    {
        gameObject.SetActive(false);
    }
    protected abstract void Act(); // 공격 등 고유 행동을 하위 클래스에서 구현
}
