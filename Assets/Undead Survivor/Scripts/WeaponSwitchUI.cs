using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitchUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;           // 무기 교체 패널 루트 (Show/Hide)
    public WeaponSlot[] slots;         // 슬롯들 (버튼+아이콘+선택표시)
    
    [Header("Data")]
    public ItemData[] weaponDatas;     // 슬롯 순서대로 연결 (Melee, Range, ...)

    [Header("Behavior")]
    public KeyCode toggleKey = KeyCode.Tab; // 탭으로 열고 닫기
    public bool autoCloseOnSelect = true;   // 선택 시 자동 닫기
    public bool pauseOnOpen = false;        // 열 때 일시정지 할지

    void Start()
    {
        Build();
        Refresh();
        Hide();
    }

    void OnEnable() => Refresh();

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();

        if (panel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q)) Cycle(-1);
            if (Input.GetKeyDown(KeyCode.E)) Cycle(+1);
            if (Input.GetKeyDown(KeyCode.Escape)) Hide();
        }
    }

    void Build()
    {
        for (int i = 0; i < slots.Length && i < weaponDatas.Length; i++)
        {
            var d = weaponDatas[i];
            if (d == null || slots[i] == null) continue;
            slots[i].Setup(d, OnSlotClicked);
        }
    }

    public void Refresh()
    {
        int currentId = GameManager.instance?.weapon?.id ?? -1;

        for (int i = 0; i < slots.Length && i < weaponDatas.Length; i++)
        {
            var slot = slots[i];
            var d = weaponDatas[i];
            if (slot == null || d == null) continue;

            bool owned = UpgradeProgress.GetLevel(d.itemId) > 0;
            bool selected = (currentId == d.itemId);
            slot.Refresh(owned, selected);
        }
    }

    void OnSlotClicked(WeaponSlot slot)
    {
        if (slot == null || slot.data == null) return;

        bool owned = UpgradeProgress.GetLevel(slot.data.itemId) > 0;
        if (!owned) return;

        WeaponSwitchService.SwitchTo(slot.data);
        Refresh();
        if (autoCloseOnSelect) Hide();
    }

    void Cycle(int dir)
    {
        int n = weaponDatas != null ? weaponDatas.Length : 0;
        if (n == 0) return;

        int currentId = GameManager.instance?.weapon?.id ?? -1;
        int start = 0;
        for (int i = 0; i < n; i++)
        {
            if (weaponDatas[i] != null && weaponDatas[i].itemId == currentId)
            {
                start = i;
                break;
            }
        }

        for (int step = 1; step <= n; step++)
        {
            int idx = (start + dir * step + n * 100) % n;
            var d = weaponDatas[idx];
            if (d == null) continue;

            if (UpgradeProgress.GetLevel(d.itemId) > 0)
            {
                WeaponSwitchService.SwitchTo(d);
                Refresh();
                break;
            }
        }
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        Refresh();
        if (pauseOnOpen) Time.timeScale = 0f;
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
        if (pauseOnOpen) Time.timeScale = 1f;
    }

    public void Toggle()
    {
        if (panel == null) return;
        if (panel.activeSelf) Hide(); else Show();
    }
}
