using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;

    public GameObject[] pets;

    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length]; //배열초기화

        for (int index=0;index<pools.Length;index++){
            pools[index] = new List<GameObject>(); //배열안의 리스트 초기화
        }

        Debug.Log(pools.Length);
    }

    public GameObject Get(int index){
        GameObject select = null;
        foreach(GameObject item in pools[index]){
            if (!item.activeSelf){
                select = item;
                select.SetActive(true);
                break;
            }
        }
        if (!select){
            select = Instantiate(prefabs[index],transform);
            pools[index].Add(select);
        }
        return select;
    }
}
