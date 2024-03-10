using Sounds;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private AudioClip _clickSound;

    private void OnClick()
    {
        SoundManager.Instance.PlaySound(_clickSound, _audioMixerGroup);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }
}
