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

    //     void Next(){
    //         foreach (Item item in items){
    //             item.gameObject.SetActive(false);
    //         }
    //         int[] ran = new int[3];
    //         while (true){
    //             ran[0] = Random.Range(0,items.Length);
    //             ran[1] = Random.Range(0,items.Length);
    //             ran[2] = Random.Range(0,items.Length);
    //             if(ran[0]!= ran[1]&& ran[1]!= ran[2] && ran[0]!= ran[2])
    //                 break;
    //         }
    //         for (int index =0;index<ran.Length; index++){
    //             Item ranItem = items[ran[index]];
    //             if(ranItem.level == ranItem.data.damages.Length){
    //                 items[4].gameObject.SetActive(true);
    //             }
    //             else{
    //                 ranItem.gameObject.SetActive(true);
    //             }

    //         }
    //     }

    void Next()
    {
        // 0) 모두 끄고 시작 (안전)
        foreach (Item item in items)
            item.gameObject.SetActive(false);

        int total = items.Length;        // 보통 5
        int fallbackIndex = 4;           // 대체(힐) 카드가 5번째임을 전제로 함
        int need = 3;

        // 1) 5개 전체(대체 포함)에서 중복 없이 3장 뽑기
        System.Collections.Generic.HashSet<int> chosen = new System.Collections.Generic.HashSet<int>();
        while (chosen.Count < need)
        {
            int idx = Random.Range(0, total);
            chosen.Add(idx);
        }

        // 2) 최종 표시 목록 구성 (맥스면 대체/다른 후보로 교체)
        var final = new System.Collections.Generic.List<int>(need);
        foreach (int idx in chosen)
        {
            int slot = idx;

            // (a) 맥스인 카드는 대체 카드로 우선 교체 시도
            if (slot != fallbackIndex && IsMaxed(items[slot]))
            {
                // 대체 카드가 이미 포함되어 있지 않으면 대체로 교체
                if (!final.Contains(fallbackIndex) && !chosen.Contains(fallbackIndex))
                {
                    slot = fallbackIndex;
                }
                else
                {
                    // (b) 대체가 이미 포함되어 있다면, 다른 '비-맥스' 카드로 교체 시도
                    for (int i = 0; i < total; i++)
                    {
                        if (i == slot) continue;
                        if (final.Contains(i)) continue;
                        if (!IsMaxed(items[i]))
                        {
                            slot = i;
                            break; // 첫 비-맥스 후보로 교체하고 종료
                        }
                    }
                    // (모두 맥스면 slot 그대로 유지)
                }
            }

            // 중복 방지
            if (!final.Contains(slot))
                final.Add(slot);
        }

        // 3) 만약(중복 조정 등으로) 3장 미만이면 남는 아무 카드로 채움
        for (int i = 0; final.Count < need && i < total; i++)
        {
            if (!final.Contains(i))
                final.Add(i);
        }

        // 4) 최종 활성화
        foreach (int idx in final)
            items[idx].gameObject.SetActive(true);
    }


    // --- 헬퍼: BuffSO/ItemData 모두 대응한 최대레벨 체크 ---
    int GetMaxLen(Item it)
    {
        if (it == null) return 0;
        // BuffSO 우선
        if (it.buff != null && it.buff.rates != null && it.buff.rates.Length > 0)
            return it.buff.rates.Length;

        // 기존 ItemData
        if (it.data != null && it.data.damages != null && it.data.damages.Length > 0)
            return it.data.damages.Length;

        // Heal(대체)처럼 레벨 개념이 없으면 0 -> 항상 사용 가능
        return 0;
    }

    bool IsMaxed(Item it)
    {
        int len = GetMaxLen(it);
        if (len <= 0) return false;
        int current = UpgradeProgress.GetLevel(it.GetUpgradeId());  // [변경: it.level → 전역]
        return current >= len;
    }
}
