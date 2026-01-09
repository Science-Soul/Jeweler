using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource SFX;

    public GameObject musicSwitchOff;
    public GameObject musicSwitchOn;
    
    public GameObject sfxSwitchOff;
    public GameObject sfxSwitchOn;

    private int musicOn = 1;
    private int sfxOn = 1;

    private void Awake()
    {
        musicOn = PlayerPrefs.GetInt("musicOn", 1); // Хитрая система сохранения звуков
        sfxOn = PlayerPrefs.GetInt("sfxOn", 1);

        if (musicOn == 1)
        {
            MusicOn();
        }
        else
        {
            MusicOff();
        }

        if (sfxOn == 1)
        {
            SFXOn();
        }
        else
        {
            SFXOff();
        }
    }

    public void MusicOff()
    {
        if (music != null)
        {
            music.mute = true;
            musicOn = 0;
            musicSwitchOff.SetActive(true);
            musicSwitchOn.SetActive(false);
            SaveMusic();
        }
    }

    public void MusicOn()
    {
        if (music != null)
        {
            music.mute = false;
            musicOn = 1;
            musicSwitchOff.SetActive(false);
            musicSwitchOn.SetActive(true);
            SaveMusic();
        }
    }

    public void SFXOff()
    {
        if (SFX != null)
        {
            SFX.mute = true;
            sfxOn = 0;
            sfxSwitchOff.SetActive(true);
            sfxSwitchOn.SetActive(false);
            SaveSFX();
        }
    }

    public void SFXOn()
    {
        if (SFX != null)
        {
            SFX.mute = false;
            sfxOn = 1;
            sfxSwitchOff.SetActive(false);
            sfxSwitchOn.SetActive(true);
            SaveSFX();
        }
    }

    void SaveMusic()
    {
        PlayerPrefs.SetInt("musicOn", musicOn);
    }

    void SaveSFX()
    {
        PlayerPrefs.SetInt("sfxOn", sfxOn);
    }
}
