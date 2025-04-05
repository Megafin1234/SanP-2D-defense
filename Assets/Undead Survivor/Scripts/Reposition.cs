using System.Collections.Generic;
using UnityEngine;

public enum MapType { 평원, 구릉지, 동굴, 사막, 몰라 }

public class Reposition : MonoBehaviour
{
    public static Reposition instance; // 추가

    Collider2D coll;

    public GameObject[] groundSet1;
    public GameObject[] groundSet2;
    public GameObject[] groundSet3;
    public GameObject[] groundSet4;
    public GameObject[] groundSet5;

    private GameObject[][] tilemapSets;

    public MapType currentMap = MapType.평원;
    private int currentSetIndex = 0;

    void Awake()
    {
        instance = this; // 싱글톤화
        tilemapSets = new GameObject[][] { groundSet1, groundSet2, groundSet3, groundSet4, groundSet5};
    }

    public void SetCurrentMap(MapType type)
    {
        currentMap = type;

        switch (type)
        {
            case MapType.평원:
                currentSetIndex = 0;
                break;
            case MapType.구릉지:
                currentSetIndex = 1;
                break;
            case MapType.동굴:
                currentSetIndex = 2;
                break;
            case MapType.사막:
                currentSetIndex = 3;
                break;
            case MapType.몰라:
                currentSetIndex = 3;
                break;
        }

        for (int i = 0; i < tilemapSets.Length; i++)
        {
            SetActiveTilemapSet(i, i == currentSetIndex);
        }
    }

    private void SetActiveTilemapSet(int setIndex, bool isActive = true)
    {
        foreach (GameObject tilemap in tilemapSets[setIndex])
        {
            tilemap.SetActive(isActive);
        }
    }

    /*
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
    } */
}
