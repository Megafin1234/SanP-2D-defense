using UnityEngine;

public class UIPageSwitcher : MonoBehaviour
{
    [Header("UI Panels 순서대로 넣기 (Inventory, PetInv, Dex, WeaponSwitch)")]
    public GameObject[] pages;

    private int currentIndex = 1; // 기본 펫인벤
    private bool isMenuOpen = false;

    void Update()
    {
        // ESC 또는 P로 펫인벤 열기/닫기
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isMenuOpen)
                CloseAll();
            else
                OpenPage(1);
        }

        // 단축키로 바로 진입 (이제는 isMenuOpen도 true 설정)
        if (Input.GetKeyDown(KeyCode.I)) OpenPage(0);
        if (Input.GetKeyDown(KeyCode.K)) OpenPage(2);
        if (Input.GetKeyDown(KeyCode.Tab)) OpenPage(3);

        // 메뉴 열려 있을 때만 전환 가능
        if (!isMenuOpen) return;

        // 좌우 페이지 전환
        if (Input.GetKeyDown(KeyCode.A))
            ChangePage(-1);
        else if (Input.GetKeyDown(KeyCode.D))
            ChangePage(1);
    }

    void ChangePage(int dir)
    {
        int newIndex = currentIndex + dir;
        if (newIndex < 0) newIndex = pages.Length - 1;
        else if (newIndex >= pages.Length) newIndex = 0;

        OpenPage(newIndex);
    }

    void OpenPage(int index)
    {
        // 다른 모든 페이지 끄고 해당 페이지만 켜기
        for (int i = 0; i < pages.Length; i++)
            pages[i].SetActive(i == index);

        currentIndex = index;
        isMenuOpen = true;      // 단축키로 진입해도 메뉴 상태로 인식
        Time.timeScale = 0f;    // 필요 시 주석처리 가능
    }

    void CloseAll()
    {
        foreach (var p in pages)
            p.SetActive(false);

        isMenuOpen = false;
        Time.timeScale = 1f;
    }
}
