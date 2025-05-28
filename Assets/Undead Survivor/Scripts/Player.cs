using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCoolDown;
    public float trailInterval = 0.05f; // 잔상 생성 간격
    public float trailLifetime = 0.3f; // 잔상 지속 시간
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;
    public float dragCoefficient;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    float dashTime;
    public float dashWaiting;
    float trailTimer;
    float trailIntervalReal;
    Vector2 dashVec;

    public List<GameObject> party;

    public delegate void Skill(int level);
    List<Skill> skills = new List<Skill>(); //얘가 스킬 목록임

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        dashWaiting = 0;

        skills.Add(skillTest); //이렇게 추가 사사삭 해서 스킬쓰기
    }
    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];

    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
    }
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        Vector2 nextVec;
        if (dashTime > 0) // 대시 중이면 대시 속도로 이동
        {
            Vector2 dragForce = -dashVec.normalized * dragCoefficient * dashVec.magnitude * Time.fixedDeltaTime;
            dashVec += dragForce;
            nextVec = dashVec;
            dashTime -= Time.fixedDeltaTime;
            trailTimer -= Time.fixedDeltaTime;
            if (trailTimer <= 0)
            {
                CreateTrail();
                trailTimer = trailIntervalReal;
                trailIntervalReal += 0.01f;
            }
        }
        else
        {
            nextVec = inputVec * speed * Time.fixedDeltaTime;
        }
        dashWaiting -= Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    
    void OnInv()
    {
        //Debug.Log("OnInv");
        GameManager.instance.inventoryScript.InvToggle();
    }

    void useSkill(int skillID, int level)
    {
        if (skillID >= 0 && skillID < skills.Count)
        {
            skills[skillID](level);  // 선택한 함수 실행
        }
        else
        {
            Debug.Log("스킬 실행 실패: 잘못된 인덱스(ID)");
        }
    }

    void OnQSkill()
    {
        useSkill(GameManager.instance.equipSkillIDs[0], GameManager.instance.equipSkillLvls[0]);
    }
    void OnESkill()
    {
        useSkill(GameManager.instance.equipSkillIDs[1], GameManager.instance.equipSkillLvls[1]);
    }
    void OnXSkill()
    {
        useSkill(GameManager.instance.equipSkillIDs[2], GameManager.instance.equipSkillLvls[2]);
    }
    void OnCSkill()
    {
        useSkill(GameManager.instance.equipSkillIDs[3], GameManager.instance.equipSkillLvls[3]);
    }
    void OnMove(InputValue value)
    {
        // if (!GameManager.instance.isLive) //이거 있으면 일시정지했다가 풀릴때 무빙 이상하게쳐서 지워용
        //     return;
        inputVec = value.Get<Vector2>();
    }

    void OnAttack()
    {
        GameManager.instance.weapon.MouseFire();
    }

    void OnCatch()
    {
        GameManager.instance.weapon.TryCatch();
    }

    void OnDash()
    {
        if (dashWaiting <= 0) // 이제 마우스로 대시하니까 안움직여도 대시 가능 ㅋㅋ
        {
            dashWaiting = dashCoolDown;
            dashTime = dashDuration;
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//마우스 위치 챙기기
            Vector2 dashDir = (mouseWorldPos - (Vector2)transform.position).normalized; //대시할 방향 잡기
            dashVec = dashDir * dashSpeed * Time.fixedDeltaTime;                        //삐쓩
            spriter.flipX = dashDir.x < 0;
            //dashVec = inputVec.normalized * dashSpeed * Time.fixedDeltaTime;
            trailIntervalReal = trailInterval;
            trailTimer = 0;
        }
    }

    IEnumerator FadeAndDestroy(SpriteRenderer renderer, GameObject obj)
    {
        float elapsed = 0f;
        float duration = trailLifetime; // 사라지는 데 걸리는 시간
        Color startColor = renderer.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            renderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(obj); // 완전히 사라지면 삭제
    }

    void CreateTrail()
    {
        GameObject trail = new GameObject("Trail"); // 잔상 오브젝트 생성
        SpriteRenderer trailSprite = trail.AddComponent<SpriteRenderer>(); // SpriteRenderer 추가
        trailSprite.sprite = spriter.sprite; // 현재 스프라이트 복사
        trailSprite.color = new Color(1f, 0.5f, 0.5f, 1f); // 색상 설정
        trail.transform.position = transform.position;
        trail.transform.rotation = transform.rotation;
        trail.transform.localScale = transform.localScale;
        trailSprite.flipX = spriter.flipX;

        StartCoroutine(FadeAndDestroy(trailSprite, trail));
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0 && dashTime <= 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!GameManager.instance.isLive)
            return;

        GameManager.instance.health -= amount;

        if (GameManager.instance.health < 0)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.instance.health -= Time.deltaTime * 10;
        }

        if (GameManager.instance.health < 0)
        {
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }

    void skillTest(int level)
    {
        Debug.Log($"스킬 실행, 레벨:{level}");
    }
}