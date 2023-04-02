using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider music_volume_slider;
    [SerializeField] private Slider sfx_volume_slider;

    // Start is called before the first frame update
    void Start()
    {
        music_volume_slider.value = AudioManager.Instance.GetMusicVolume();
        music_volume_slider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });

        sfx_volume_slider.value = AudioManager.Instance.GetSoundFxVolume();
        sfx_volume_slider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
    }

    private void UpdateMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(music_volume_slider.value);
    }

    private void UpdateSFXVolume()
    {
        AudioManager.Instance.SetSoundFxVolume(sfx_volume_slider.value);
    }
}
