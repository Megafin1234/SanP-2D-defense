using UnityEngine;
using UnityEngine.UI;
using System;

public class WeaponSlot : MonoBehaviour
{
    public ItemData data;

    [Header("UI Refs")]
    public Image icon;
    public Text  nameText;
    public GameObject selectedMark; // 체크/하이라이트용 (없으면 비워둬도 됨)
    public CanvasGroup cg;          // 소유X일 때 alpha 낮추기
    public Button button;

    Action<WeaponSlot> onClick;

    public void Setup(ItemData d, Action<WeaponSlot> onClick)
    {
        data = d;
        this.onClick = onClick;

        if (!button) button = GetComponent<Button>();
        if (!cg) cg = GetComponent<CanvasGroup>();

        if (icon && d && d.itemIcon) icon.sprite = d.itemIcon;
        if (nameText && d) nameText.text = d.itemName;

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.onClick?.Invoke(this));
        }
    }

    public void Refresh(bool owned, bool selected)
    {
        if (button) button.interactable = owned;
        if (cg) cg.alpha = owned ? 1f : 0.4f;
        if (selectedMark) selectedMark.SetActive(selected);
    }
}
