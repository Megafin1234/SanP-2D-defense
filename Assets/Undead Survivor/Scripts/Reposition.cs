
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;
    public GameObject[] groundSet1;
    public GameObject[] groundSet2;
    public GameObject[] groundSet3;
    private GameObject[][] tilemapSets;
    private int currentSetIndex = 0;

    void Awake()
    {
        tilemapSets = new GameObject[][] { groundSet1, groundSet2, groundSet3 }; // 배열에 세트 추가
        coll = GetComponent<Collider2D>();
        SetActiveTilemapSet(currentSetIndex);
        SetActiveTilemapSet(currentSetIndex + 1, false);
        SetActiveTilemapSet(currentSetIndex + 2, false);
    }
    public void ToggleTilemapLayers()
    {
        SetActiveTilemapSet(currentSetIndex, false);
        currentSetIndex = (currentSetIndex + 1) % tilemapSets.Length;
        SetActiveTilemapSet(currentSetIndex, true);
    }

    private void SetActiveTilemapSet(int setIndex, bool isActive = true)
    {
        foreach (GameObject tilemap in tilemapSets[setIndex])
        {
            tilemap.SetActive(isActive);
        }
    }
    private void OnTriggerExit2D(Collider2D collision){ 
        if (!collision.CompareTag("Area"))
            return;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        switch (transform.tag){
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX <0 ? -1 : 1;
                float dirY = diffY <0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);
                if(diffX>diffY){
                    transform.Translate(Vector3.right*dirX*40);
                }
                else if(diffX<diffY){
                    transform.Translate(Vector3.up*dirY*40);
                }
                break;
            case "GroundA":
                diffX = playerPos.x - myPos.x;
                diffY = playerPos.y - myPos.y;
                dirX = diffX <0 ? -1 : 1;
                dirY = diffY <0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);
                if(diffX>diffY){
                    transform.Translate(Vector3.right*dirX*40);
                }
                else if(diffX<diffY){
                    transform.Translate(Vector3.up*dirY*40);
                }
                break;
            case "GroundB":
                diffX = playerPos.x - myPos.x;
                diffY = playerPos.y - myPos.y;
                dirX = diffX <0 ? -1 : 1;
                dirY = diffY <0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);
                if(diffX>diffY){
                    transform.Translate(Vector3.right*dirX*40);
                }
                else if(diffX<diffY){
                    transform.Translate(Vector3.up*dirY*40);
                }
                break;
            case "Enemy":
                if (coll.enabled){
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3,3), Random.Range(-3,3),0);
                    transform.Translate(ran + dist * 2);

                }
                break;
        }
    }
}
