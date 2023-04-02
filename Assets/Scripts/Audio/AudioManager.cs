using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] sounds;
    private float sound_fx_volume;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(this.gameObject);
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        this.SetSoundFxVolume(0.5f);
        this.SetMusicVolume(0.5f);
    }

    private void Start()
    {
        this.PlaySound("Music");
    }

    public void SetMusicVolume(float volume)
    {
        this.SetVolume("Music", volume);
    }

    public float GetMusicVolume()
    {
        return this.GetVolume("Music");
    }

    public void PlaySound(string sound_name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sound_name);
        if (s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("Sound: " + sound_name + " does not exist!");
        }
    }

    public void StopSound(string sound_name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sound_name);
        if (s != null)
        {
            s.source.Stop();
        }
        else
        {
            Debug.LogWarning("Sound: " + sound_name + " does not exist!");
        }
    }

    public void SetVolume(string sound_name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sound_name);
        if (s != null)
        {
            s.volume = volume;
            s.source.volume = volume;
        }
        else
        {
            Debug.LogWarning("Sound: " + sound_name + " does not exist!");
        }
    }

    public float GetVolume(string sound_name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == sound_name);
        if (s != null)
        {
            return s.volume;
        }
        else
        {
            Debug.LogWarning("Sound: " + sound_name + " does not exist!");
        }
        return 0f;
    }

    public void SetSoundFxVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            if (s.name != "Music")
            {
                s.volume = volume;
                s.source.volume = volume;
            }
        }
        this.sound_fx_volume = volume;
    }

    public float GetSoundFxVolume()
    {
        return this.sound_fx_volume;
    }
}
