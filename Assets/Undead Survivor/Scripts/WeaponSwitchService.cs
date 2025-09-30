using UnityEngine;

public static class WeaponSwitchService
{
    public static void SwitchTo(ItemData data)
    {
        if (data == null) return;

        var gm = GameManager.instance;
        var weapon = gm != null ? gm.weapon : null;

        // 주무기 보장
        if (weapon == null)
        {
            GameObject go = new GameObject("Weapon_" + data.itemId);
            weapon = go.AddComponent<Weapon>();
            if (gm != null) gm.weapon = weapon;
        }

        // 무기 초기화
        weapon.Init(data);

        // 저장된 레벨만큼 다시 적용 (damage는 절대치 치환, count는 누적)
        int level = UpgradeProgress.GetLevel(data.itemId);
        ApplyUpgrades(weapon, data, level);
    }

    static void ApplyUpgrades(Weapon weapon, ItemData data, int level)
    {
        if (weapon == null || data == null || level <= 0) return;

        int lenD = (data.damages != null) ? data.damages.Length : 0;
        int lenC = (data.counts  != null) ? data.counts.Length  : 0;

        // 과거에 레벨업 버튼을 누른 횟수만큼 재적용
        for (int i = 0; i < level; i++)
        {
            int idxD = lenD > 0 ? Mathf.Clamp(i, 0, lenD - 1) : 0;
            int idxC = lenC > 0 ? Mathf.Clamp(i, 0, lenC - 1) : 0;

            float dmg = data.baseDamage + data.baseDamage * (lenD > 0 ? data.damages[idxD] : 0f);
            int   cnt = (lenC > 0 ? data.counts[idxC] : 0);

            weapon.LevelUp(dmg, cnt);
        }
    }
}
