using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    private NavMeshAgent navMeshAgent;
    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    NavMeshAgent agent;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
        coll = GetComponent<Collider2D>();
        navMeshAgent = GetComponent<NavMeshAgent>();

    
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;

        navMeshAgent.enabled = true;  
        navMeshAgent.speed = speed;   
    }

    void Update()
    {
        if (!GameManager.instance.isLive || !isLive) 
            return;

        navMeshAgent.SetDestination(target.position);
        spriter.flipX = target.position.x < transform.position.x;
    }
    void FixedUpdate() {
    if (!GameManager.instance.isLive || !isLive) {
        navMeshAgent.enabled = false; 
        return;
        }
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;

         // NavMeshAgent의 속도 동기화
    if (navMeshAgent != null) {
        navMeshAgent.speed = speed;
    }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive) return;
        
        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack());

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
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
        }
    }

    IEnumerator KnockBack()
    {
    if (agent != null) 
        agent.enabled = false; // 넉백 동안 NavMeshAgent 비활성화

    yield return new WaitForFixedUpdate(); // 물리 프레임 딜레이
    Vector3 playerPos = GameManager.instance.player.transform.position;
    Vector3 dirVec = transform.position - playerPos;
    rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

    yield return new WaitForSeconds(0.5f); // 넉백 지속 시간

    if (agent != null)
        agent.enabled = true; // 넉백이 끝난 후 NavMeshAgent 재활성화
    }


    void Dead()
    {
        gameObject.SetActive(false);
    }
}
