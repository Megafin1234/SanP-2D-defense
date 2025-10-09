using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterDexUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform contentParent;
    public MonsterDexDatabase database;
    public Button closeButton;

    private List<GameObject> activeSlots = new List<GameObject>();

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        // 기존 슬롯 삭제
        foreach (var obj in activeSlots)
            Destroy(obj);
        activeSlots.Clear();

        // 도감 데이터 순회
        foreach (var entry in database.entries)
        {
            Debug.Log($"생성 중: {entry.enemyData?.UnitName ?? "null"}");
            GameObject slot = Instantiate(slotPrefab, contentParent);
            activeSlots.Add(slot);

            // 슬롯 내부 컴포넌트 참조
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            Text nameText = slot.transform.Find("NameText").GetComponent<Text>();
            Image statusIcon = slot.transform.Find("StatusIcon").GetComponent<Image>();
            Image capturedIcon = slot.transform.Find("CapturedIcon").GetComponent<Image>();

            if (!entry.isDiscovered)
            {
                icon.color = Color.black;
                nameText.text = "???";
                statusIcon.enabled = false;
                capturedIcon.enabled = false;
                continue;
            }

            icon.sprite = entry.icon ?? entry.enemyData?.sprite;
            icon.color = Color.white;
            nameText.text = entry.enemyData?.UnitName ?? "Unknown";
            statusIcon.gameObject.SetActive(entry.isDefeated);
            capturedIcon.gameObject.SetActive(entry.isCaptured);
        }
    }

    void Start()
    {
        if (closeButton)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
