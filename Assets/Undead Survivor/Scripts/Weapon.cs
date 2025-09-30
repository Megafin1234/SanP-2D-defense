using System;
using System.Data;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    // 의미 통일: speed = 공격 간격(쿨다운, 초)
    public float speed;
    public float catchCool;
    float timer;
    float catchTimer;

    Player player;
    bool canFire;
    bool canCatch;
    int combo=0;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 쿨다운 타이머 공통 처리
        timer += Time.deltaTime;
        if (timer > speed)
        {
            timer = 0f;
            canFire = true;
        }

        // 캐치 쿨다운도 동일 간격 사용 (필요 시 분리 가능)
        catchTimer += Time.deltaTime;
        if (catchTimer > speed)
        {
            catchTimer = 0f;
            canCatch = true;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        // if (id == 0)
        //     Batch();
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count  = data.baseCount + Character.Count;

        // projectile -> prefabId 매핑
        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        // speed(공격 간격) 통일
        // 근접/원거리 모두 쿨다운 개념으로 0.5 * WeaponRate
        switch (id)
        {
            default:
                speed = 0.5f * Character.WeaponRate;
                break;
        }

        // Hand Set: 선택된 무기만 활성화
        if (player != null && player.hands != null && player.hands.Length > 0)
        {
            for (int i = 0; i < player.hands.Length; i++)
                player.hands[i].gameObject.SetActive(false);

            // Melee=0, Range=1 전제로 인덱싱 (ItemData.ItemType과 일치)
            int handIndex = (int)data.itemType;
            if (handIndex >= 0 && handIndex < player.hands.Length)
            {
                Hand hand = player.hands[handIndex];
                if (hand != null)
                {
                    hand.spriter.sprite = data.hand;
                    hand.gameObject.SetActive(true);
                }
            }
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().owner = Bullet.BulletOwner.Player;
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

    public void MouseFire()
    {
        if (!canFire) return;
        canFire = false;

        if (id == 0)
        {
            // 근접: 마우스 방향 휘두르기
            StartCoroutine(SlashShovel());
            return;
        }

        // 원거리: 마우스 타겟 발사
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

    IEnumerator SlashShovel()
    {
        Transform bullet;
        Vector3 dir;
        if (transform.childCount > 0)
        {
            bullet = transform.GetChild(0);
            bullet.GetComponent<Bullet>().BulletActive(true);
        }
        else
        {
            bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
        }
        bullet.localPosition = Vector3.zero;
        bullet.localRotation = Quaternion.identity;
        { // 돌리기
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dir = targetPos - transform.position;
            dir.z = 0;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        Vector3 rotVec = Vector3.forward * (combo == 0 ? 310 : 50);
        bullet.Rotate(rotVec);
        bullet.Translate(bullet.up * 1f, Space.World);
        yield return new WaitForSeconds(0.05f);
        bullet.localPosition = Vector3.zero;

        rotVec = Vector3.forward * (combo == 0 ? 100 : 260);
        bullet.Rotate(rotVec);
        bullet.localPosition = Vector3.zero;
        bullet.Translate(bullet.up * 1f, Space.World);

        {
            dir = dir.normalized;
            Transform slash = GameManager.instance.pool.Get(7).transform;
            slash.position = transform.position;
            slash.rotation = Quaternion.FromToRotation(Vector3.up, dir);
            slash.localScale = new Vector3(2.2f, 2.2f, 2.2f);
            slash.Translate(slash.up * 1.2f, Space.World);
            slash.GetComponent<Bullet>().owner = Bullet.BulletOwner.Player;
            slash.GetComponent<Bullet>().Init(damage, 0, dir, 0);
        }

        yield return new WaitForSeconds(0.25f);
        bullet.GetComponent<Bullet>().BulletActive(false);
        combo++;
        combo %= 2;
    }

    public void TryCatch()
    {
        if (!canCatch) return;
        canCatch = false;

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir = dir.normalized;

        // bullet이 아니라 그물을 던짐
        Transform bullet = GameManager.instance.pool.Get(5).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<CatchTool>().Init(damage, count, dir);
    }
}
