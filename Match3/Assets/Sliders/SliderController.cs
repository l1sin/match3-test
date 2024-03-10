using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _slider;
    [SerializeField] private SliderType _sliderType;

    private enum SliderType
    {
        SFX,
        Music
    }

    public void LoadSlider()
    {
        switch (_sliderType)
        {
            case SliderType.SFX:
                float sFXVolume = SaveManager.Instance.CurrentProgress.SFXVolume;
                _slider.value = sFXVolume;
                break;
            case SliderType.Music:
                float sFXMusic = SaveManager.Instance.CurrentProgress.MusicVolume;
                _slider.value = sFXMusic;
                break;
            default: break;
        }
    }

    public void SetSliderValue()
    {
        switch (_sliderType)
        {
            case SliderType.SFX:
                SetSFXFromSlider();
                break;
            case SliderType.Music:
                SetMusicFromSlider();
                break;
            default: break;
        }
    }

    public void SetSFXFromSlider()
    {
        if (_slider.value < 0.005) _slider.value = 0.0001f;
        float newValue = Mathf.Log10(_slider.value) * 20f;
        _audioMixer.SetFloat("VolumeSFX", newValue);
        SaveManager.Instance.CurrentProgress.SFXVolume = _slider.value;
    }

    public void SetMusicFromSlider()
    {
        if (_slider.value < 0.005) _slider.value = 0.0001f;
        float newValue = Mathf.Log10(_slider.value) * 20f;
        _audioMixer.SetFloat("VolumeMusic", newValue);
        SaveManager.Instance.CurrentProgress.MusicVolume = _slider.value;
    }
}
