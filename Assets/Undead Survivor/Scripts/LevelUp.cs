using System.Data.Common;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    RectTransform rect;
    Item[] items;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }
    public void Show()
    {
        Next();
        rect.localScale = Vector3.one;
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBgm(true);
    }
    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBgm(false);


    }
    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next(){
        foreach (Item item in items){
            item.gameObject.SetActive(false);
        }
        int[] ran = new int[3];
        while (true){
            ran[0] = Random.Range(0,items.Length);
            ran[1] = Random.Range(0,items.Length);
            ran[2] = Random.Range(0,items.Length);
            if(ran[0]!= ran[1]&& ran[1]!= ran[2] && ran[0]!= ran[2])
                break;
        }
        for (int index =0;index<ran.Length; index++){
            Item ranItem = items[ran[index]];
            if(ranItem.level == ranItem.data.damages.Length){
                items[4].gameObject.SetActive(true);
            }
            else{
                ranItem.gameObject.SetActive(true);
            }

        }
    }
}
