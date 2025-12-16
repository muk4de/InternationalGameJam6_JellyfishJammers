using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderManager : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    void Start()
    {
        bgmSlider.value = AudioManager.I.GetBgmVolume();
        sfxSlider.value = AudioManager.I.GetSfxVolume();
    }

    void Update()
    {
        
    }

    public void OnBgmSliderChanged(float volume)
    {
        AudioManager.I.SetBgmVolume(volume);
    }
    public void OnSfxSliderChanged(float volume)
    {
        AudioManager.I.SetSfxVolume(volume);
    }
}
