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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);
        dashWaiting = 0;
    }
    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
        //anim.SetInteger("Player", GameManager.instance.playerId);
    }
    void Update(){
        if(!GameManager.instance.isLive)
            return;
    }
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        Vector2 nextVec;
        if (dashTime>0) // 대시 중이면 대시 속도로 이동
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
        else{
            nextVec = inputVec * speed * Time.fixedDeltaTime;
        }
        dashWaiting -= Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void OnMove(InputValue value)
    {
        if (!GameManager.instance.isLive)
            return;
        inputVec = value.Get<Vector2>();
    }

    void OnAttack(){
        GameManager.instance.weapon.MouseFire();
    }

    void OnCatch(){
        GameManager.instance.weapon.TryCatch();
    }

    void OnDash(){
        if (dashWaiting<=0 && inputVec != Vector2.zero) // 움직이고 있을 때만 대시 가능
        {
            dashWaiting = dashCoolDown;
            dashTime = dashDuration;
            dashVec = inputVec.normalized * dashSpeed * Time.fixedDeltaTime;
            trailIntervalReal = trailInterval;
            trailTimer = 0;
        }
    }

    void CreateTrail()
    {
        GameObject trail = new GameObject("Trail"); // 잔상 오브젝트 생성
        SpriteRenderer trailSprite = trail.AddComponent<SpriteRenderer>(); // SpriteRenderer 추가
        trailSprite.sprite = spriter.sprite; // 현재 스프라이트 복사
        trailSprite.color = new Color(1f, 0.5f, 0.5f, 0.3f); // 색상 설정
        trail.transform.position = transform.position;
        trail.transform.rotation = transform.rotation;
        trail.transform.localScale = transform.localScale;
        trailSprite.flipX = spriter.flipX;

        Destroy(trail, trailLifetime + trailInterval*2 - trailIntervalReal*2); // 일정 시간 후 잔상 제거
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
        anim.SetFloat("Speed",inputVec.magnitude);
        
        if (inputVec.x != 0){
            spriter.flipX = inputVec.x < 0;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;
        if (collision.gameObject.CompareTag("Enemy")) {
            GameManager.instance.health -= Time.deltaTime * 10;
        }

        if(GameManager.instance.health <0){
            for (int index=2;index < transform.childCount;index++){
                transform.GetChild(index).gameObject.SetActive(false);
            }
            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }
    }
}