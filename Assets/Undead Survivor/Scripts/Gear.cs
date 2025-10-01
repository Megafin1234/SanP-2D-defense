using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }

    // BuffSO 오버로드
    public void Init(BuffSO so)
    {
        name = "Gear" + so.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        switch (so.buffType)
        {
            case BuffSO.BuffType.Glove:
                type = ItemData.ItemType.Glove;
                break;
            case BuffSO.BuffType.Shoe:
                type = ItemData.ItemType.Shoe;
                break;
            default:
                type = ItemData.ItemType.Glove; // 기본값
                break;
        }

        rate = (so.rates != null && so.rates.Length > 0) ? so.rates[0] : 0f;
        ApplyGear();
    }

    public void LevelUp(float nextRate)
    {
        rate = nextRate;
        ApplyGear();
    }

    // BuffSO 기반 레벨업
    public void LevelUp(BuffSO so, int nextIndex)
    {
        float next = 0f;
        if (so != null && so.rates != null && so.rates.Length > 0)
        {
            int idx = Mathf.Clamp(nextIndex, 0, so.rates.Length - 1);
            next = so.rates[idx];
        }
        rate = next;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    // 변경: 모든 무기에 동일 공식(쿨다운 감소) 적용
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            float baseDelay = 0.5f * Character.WeaponRate;
            weapon.speed = baseDelay * (1f - rate);
        }
    }

    void SpeedUp()
    {
        float baseMove = 3f * Character.Speed;
        GameManager.instance.player.speed = baseMove + baseMove * rate;
    }
}
