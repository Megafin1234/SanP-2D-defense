using UnityEngine;
using UnityEngine.UI;

public class UISettingPanel : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Button closeButton;

    void Start()
    {
        // 초기값
        if (AudioManager.instance != null)
        {
            bgmSlider.value = AudioManager.instance.GetBgmVolume();
            sfxSlider.value = AudioManager.instance.GetSfxVolume();
        }

        // 값 변경 시 반영
        bgmSlider.onValueChanged.AddListener(OnBgmChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        closeButton.onClick.AddListener(ClosePanel);
    }

    void OnBgmChanged(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetBgmVolume(value);
    }

    void OnSfxChanged(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetSfxVolume(value);
    }

    void ClosePanel()
    {
        gameObject.SetActive(false);

        // // 🔹 UIPageSwitcher 상태 동기화 (닫힘 처리)
        // var switcher = FindObjectOfType<UIPageSwitcher>();
        // if (switcher != null)
        //     switcher.CloseAll();
    }
}
