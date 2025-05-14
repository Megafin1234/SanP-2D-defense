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
        switch(id){
            // case 0: 
            //     //transform.Rotate(Vector3.back* speed* Time.deltaTime);
            //     break;
            default:
                timer += Time.deltaTime;

                if(timer>speed){
                    timer = 0f;
                    //Fire();
                    canFire = true;
                }

                catchTimer += Time.deltaTime;
                if(catchTimer>speed){
                    catchTimer=0f;
                    canCatch=true;
                }
                break;
        }
    }
    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        // if (id ==0)
        //     Batch();
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
    public void Init(ItemData data)
    {
        //Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;
        //Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int index=0; index < GameManager.instance.pool.prefabs.Length; index++){
            if(data.projectile == GameManager.instance.pool.prefabs[index]){
                prefabId = index;
                break;
            }
        }
        switch(id){
            case 0: 
                speed = 150*Character.WeaponSpeed;
                //Batch();
                break;
            case 1: //총
                speed = 0.5f* Character.WeaponRate;
                break;
            default:
                speed = 0.5f* Character.WeaponRate;
                break;
        }

        //Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);
        
        player.BroadcastMessage("ApplyGear",SendMessageOptions.DontRequireReceiver);
    }
    void Batch()
    {
        for (int index =0; index < count; index ++){
            Transform bullet;
            if (index < transform.childCount){
                bullet = transform.GetChild(index);
            }
            else {
                bullet= GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            
            

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec= Vector3.forward *360 * index / count ;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);  //-1 is infinity per
        }
    }
    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;
        
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;


        Transform bullet= GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);  //목표방향으로 회전하는 함수
        bullet.GetComponent<Bullet>().owner = Bullet.BulletOwner.Player;
        bullet.GetComponent<Bullet>().Init(damage, count,dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);

    }

    IEnumerator SlashShovel(){
        Transform bullet;
        Vector3 dir;
        if(transform.childCount>0){
            bullet = transform.GetChild(0);
            bullet.GetComponent<Bullet>().BulletActive(true);
        }
        else{
            bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.parent = transform;
        }
        bullet.localPosition = Vector3.zero;
        bullet.localRotation = Quaternion.identity;
        { //돌리기
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir = targetPos - transform.position;
        dir.z = 0;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.localRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
        Vector3 rotVec= Vector3.forward * (combo==0?310:50);
        bullet.Rotate(rotVec);
        bullet.Translate(bullet.up * 1f, Space.World);
        yield return new WaitForSeconds(0.05f);
        bullet.localPosition = Vector3.zero;
        //bullet.localRotation = Quaternion.identity;
        rotVec = Vector3.forward * (combo==0?100:260);
        // for(int i=0; i<4; i++){
        //     rotVec = Vector3.forward * (combo==0?10+(i*10):350-(i*10));
            bullet.Rotate(rotVec);
            bullet.localPosition = Vector3.zero;
            bullet.Translate(bullet.up * 1f, Space.World);
        //     yield return new WaitForSeconds(0.05f);
        // }

        {
            dir = dir.normalized;
            Transform slash     = GameManager.instance.pool.Get(7).transform;
            slash.position      = transform.position;
            slash.rotation      = Quaternion.FromToRotation(Vector3.up, dir);  //목표방향으로 회전하는 함수
            slash.localScale    = new Vector3(2.2f,2.2f,2.2f);
            slash.Translate(slash.up * 1.2f, Space.World);
            slash.GetComponent<Bullet>().owner = Bullet.BulletOwner.Player;
            slash.GetComponent<Bullet>().Init(damage, 0, dir, 0);
        }
        
        yield return new WaitForSeconds(0.25f);
        bullet.GetComponent<Bullet>().BulletActive(false);
        //bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
        combo++;
        combo%=2;
    }

    public void MouseFire(){
        if(!canFire) return;
        canFire = false;
        if(id==0){
            StartCoroutine(SlashShovel());
            return;
        }
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }

    public void TryCatch(){
        if(!canCatch) return;
        canCatch = false;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = targetPos - transform.position;
        dir.z = 0;
        dir = dir.normalized;

        //bullet이 아니라 그물을 던져야 한다. 
        Transform bullet = GameManager.instance.pool.Get(5).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<CatchTool>().Init(damage, count, dir);
    }
}
