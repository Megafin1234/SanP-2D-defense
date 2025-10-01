using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // 기존
    public ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    // 추가: BuffSO 지원
    public BuffSO buff;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;
    Button btn;

    void Awake()
    {
        var images = GetComponentsInChildren<Image>(true);
        // 아이콘이 두 번째 이미지가 아닐 수도 있어 안정화
        icon = images.Length > 1 ? images[1] : GetComponentInChildren<Image>(true);

        Text[] texts = GetComponentsInChildren<Text>(true);
        textLevel = texts.Length > 0 ? texts[0] : null;
        textName  = texts.Length > 1 ? texts[1] : null;
        textDesc  = texts.Length > 2 ? texts[2] : null;

        btn = GetComponent<Button>();

        // 이름/아이콘 세팅
        if (buff != null)
        {
            if (buff.itemIcon && icon) icon.sprite = buff.itemIcon;
            if (textName) textName.text = buff.itemName;
        }
        else
        {
            if (data != null && data.itemIcon && icon) icon.sprite = data.itemIcon;
            if (data != null && textName) textName.text = data.itemName;
        }
    }

    public int GetUpgradeId()
    {
        if (buff != null) return buff.itemId;   // BuffSO(Glove=300, Shoe=301)
        if (data != null) return data.itemId;   // 기존 ItemData
        return -1;
    }

    int GetMaxLen()
    {
        if (buff != null && buff.rates != null) return buff.rates.Length;
        if (data != null && data.damages != null) return data.damages.Length;
        return 0; // 힐/기타 등 레벨 개념 없음
    }

    void OnEnable()
    {
        if (btn == null) btn = GetComponent<Button>();

        // 0) 미세팅 가드: buff와 data가 모두 비어 있으면 표시/상호작용 막고 종료
        if (buff == null && data == null)
        {
            if (textLevel) textLevel.text = "Lv.-";
            if (textDesc)  textDesc.text  = "(설정되지 않은 카드)";
            if (btn) btn.interactable = false;
            return;
        }

        // 1) 전역 레벨부터 로드
        int id = GetUpgradeId();
        if (id >= 0) level = UpgradeProgress.GetLevel(id);

        // 2) 표기 레벨 업데이트
        if (textLevel) textLevel.text = "Lv." + (level + 1);

        // 3) 설명 구성
        if (buff != null)
        {
            float r = 0f;
            if (buff.rates != null && buff.rates.Length > 0)
            {
                int idx = Mathf.Clamp(level, 0, buff.rates.Length - 1);
                r = buff.rates[idx];
            }

            if (textDesc != null)
            {
                switch (buff.buffType)
                {
                    case BuffSO.BuffType.Glove:
                    case BuffSO.BuffType.Shoe:
                        textDesc.text = string.Format(buff.itemDesc, r * 100f);
                        break;
                    default:
                        textDesc.text = buff.itemDesc;
                        break;
                }
            }
        }
        else
        {
            // 기존 ItemData 카드 표시
            if (data == null)
            {
                if (textDesc != null) textDesc.text = "";
            }
            else
            {
                int lenD = (data.damages != null) ? data.damages.Length : 0;
                int lenC = (data.counts  != null) ? data.counts.Length  : 0;
                int idxD = lenD > 0 ? Mathf.Clamp(level, 0, lenD - 1) : 0;
                int idxC = lenC > 0 ? Mathf.Clamp(level, 0, lenC - 1) : 0;

                if (textDesc != null)
                {
                    switch (data.itemType)
                    {
                        case ItemData.ItemType.Melee:
                        case ItemData.ItemType.Range:
                            float dmgPct = (lenD > 0) ? data.damages[idxD] * 100f : 0f;
                            int   cntVal = (lenC > 0) ? data.counts[idxC] : 0;
                            textDesc.text = string.Format(data.itemDesc, dmgPct, cntVal);
                            break;
                        case ItemData.ItemType.Glove:
                        case ItemData.ItemType.Shoe:
                            float ratePct = (lenD > 0) ? data.damages[idxD] * 100f : 0f;
                            textDesc.text = string.Format(data.itemDesc, ratePct);
                            break;
                        default:
                            textDesc.text = string.Format(data.itemDesc);
                            break;
                    }
                }
            }
        }

        // 4) 버튼 상호작용 상태 동기화(최대 레벨이면 비활성)
        if (btn != null)
        {
            int maxLen = GetMaxLen(); // 0이면 항상 가능
            btn.interactable = (maxLen == 0) || (level < maxLen);
        }

        // 5) 다른 패널에서 이미 획득한 버프 재바인딩(gear 누락 방지)
        if (buff != null && level > 0 && gear == null)
        {
            gear = FindExistingGear(GetUpgradeId());
        }
    }

    public void OnClick()
    {
        // 0) 미세팅 가드: 런타임 클릭 시 안전막
        if (buff == null && data == null)
        {
            Debug.LogError($"[Item.OnClick] '{name}' has neither BuffSO nor ItemData assigned.");
            return;
        }

        if (buff != null)
        {
            int id = GetUpgradeId();
            // 다른 패널에서 이미 생성된 Gear 보정
            if (gear == null) gear = FindExistingGear(id);

            if (level == 0 && gear == null)
            {
                GameObject newGear = new GameObject("Gear" + id);
                gear = newGear.AddComponent<Gear>();
                gear.Init(buff);
            }
            else
            {
                int nextIdx = 0;
                if (buff.rates != null && buff.rates.Length > 0)
                    nextIdx = Mathf.Clamp(level, 0, buff.rates.Length - 1);

                // gear가 null이면(이례적) 한 번 더 보정 시도
                if (gear == null) gear = FindExistingGear(id);
                if (gear != null) gear.LevelUp(buff, nextIdx);
            }

            level++;
            UpgradeProgress.SetLevel(id, level);

            if (btn != null)
            {
                int maxLen = GetMaxLen();
                btn.interactable = (maxLen == 0) || (level < maxLen);
            }

            // 선택 후 현재 레벨업 패널 닫기
            GetComponentInParent<LevelUp>()?.Hide();
            return;
        }

        // 기존 ItemData 처리(원본 로직 유지 + 클램프/재바인딩 보강)
        if (data == null)
        {
            Debug.LogError($"[Item.OnClick] data is null on '{name}'. Did you forget to assign ItemData on this card?");
            return;
        }

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
                if (level == 0)
                {
                    // 항상 주무기 싱글톤 사용
                    weapon = GameManager.instance != null ? GameManager.instance.weapon : null;
                    if (weapon == null)
                    {
                        GameObject go = new GameObject("Weapon_" + data.itemId);
                        weapon = go.AddComponent<Weapon>();
                        // 주무기 등록
                        if (GameManager.instance != null)
                            GameManager.instance.weapon = weapon;
                    }
                    weapon.Init(data); // id=0(삽) 세팅
                }
                else
                {
                    if (weapon == null)
                        weapon = GameManager.instance != null ? GameManager.instance.weapon : null;

                    if (weapon == null)
                    {
                        GameObject go = new GameObject("Weapon_" + data.itemId);
                        weapon = go.AddComponent<Weapon>();
                        if (GameManager.instance != null)
                            GameManager.instance.weapon = weapon;
                        weapon.Init(data);
                    }

                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    int lenD = (data.damages != null) ? data.damages.Length : 0;
                    int lenC = (data.counts  != null) ? data.counts.Length  : 0;
                    int idxD = lenD > 0 ? Mathf.Clamp(level, 0, lenD - 1) : 0;
                    int idxC = lenC > 0 ? Mathf.Clamp(level, 0, lenC - 1) : 0;

                    nextDamage += data.baseDamage * (lenD > 0 ? data.damages[idxD] : 0f);
                    nextCount  += (lenC > 0 ? data.counts[idxC] : 0);

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    // 주무기 싱글톤 사용 + 방어적 생성
                    weapon = GameManager.instance != null ? GameManager.instance.weapon : null;
                    if (weapon == null)
                    {
                        GameObject go = new GameObject("Weapon_" + data.itemId);
                        weapon = go.AddComponent<Weapon>();
                        if (GameManager.instance != null)
                            GameManager.instance.weapon = weapon;
                    }
                    weapon.Init(data);
                }
                else
                {
                    if (weapon == null)
                    {
                        weapon = GameManager.instance != null ? GameManager.instance.weapon : null;
                        if (weapon == null)
                        {
                            GameObject go = new GameObject("Weapon_" + data.itemId);
                            weapon = go.AddComponent<Weapon>();
                            if (GameManager.instance != null)
                                GameManager.instance.weapon = weapon;
                            weapon.Init(data);
                        }
                    }

                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    int lenD = (data.damages != null) ? data.damages.Length : 0;
                    int lenC = (data.counts  != null) ? data.counts.Length  : 0;
                    int idxD = lenD > 0 ? Mathf.Clamp(level, 0, lenD - 1) : 0;
                    int idxC = lenC > 0 ? Mathf.Clamp(level, 0, lenC - 1) : 0;

                    nextDamage += data.baseDamage * (lenD > 0 ? data.damages[idxD] : 0f);
                    nextCount  += (lenC > 0 ? data.counts[idxC] : 0);

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
            {
                int id = GetUpgradeId();
                if (gear == null) gear = FindExistingGear(id);

                if (level == 0 && gear == null)
                {
                    GameObject newGear = new GameObject("Gear" + id);
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    int lenD = (data.damages != null) ? data.damages.Length : 0;
                    int idxD = lenD > 0 ? Mathf.Clamp(level, 0, lenD - 1) : 0;
                    float nextRate = (lenD > 0 ? data.damages[idxD] : 0f);
                    if (gear != null) gear.LevelUp(nextRate);
                }
                level++;
                break;
            }

            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                // level 증가/최대레벨 개념 없음(Heal은 항상 가능)
                break;
        }

        // 전역 진행도 반영
        UpgradeProgress.SetLevel(GetUpgradeId(), level);

        // 버튼 상호작용 갱신
        if (btn != null)
        {
            int maxLen = GetMaxLen();
            btn.interactable = (maxLen == 0) || (level < maxLen);
        }

        // 선택 후 현재 레벨업 패널 닫기
        GetComponentInParent<LevelUp>()?.Hide();
    }

    // 플레이어 하위에서 기존 Gear를 찾아 재바인딩
    private Gear FindExistingGear(int id)
    {
        var player = GameManager.instance != null ? GameManager.instance.player : null;
        if (player == null) return null;

        var gears = player.GetComponentsInChildren<Gear>(true);
        string targetName = "Gear" + id;
        foreach (var g in gears)
        {
            if (g != null && g.name == targetName)
                return g;
        }
        return null;
    }

    // 무기 재바인딩(스테이지 패널에서 GameManager.weapon이 비어있는 경우 대비)
    private Weapon FindExistingWeapon()
    {
        if (GameManager.instance != null && GameManager.instance.weapon != null)
            return GameManager.instance.weapon;

        var player = GameManager.instance != null ? GameManager.instance.player : null;
        if (player == null) return null;

        // 플레이어 하위 첫 번째 Weapon
        return player.GetComponentInChildren<Weapon>(true);
    }
}
