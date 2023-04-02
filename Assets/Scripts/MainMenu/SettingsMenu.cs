using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider volume_slider;
    
    // Start is called before the first frame update
    void Start()
    {
        volume_slider.value = AudioManager.Instance.GetMusicVolume();
        volume_slider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
    }

    private void UpdateMusicVolume()
    {
        AudioManager.Instance.SetMusicVolume(volume_slider.value);
    }
}
