using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ThemeBGMHandler : MonoBehaviour {

    public AudioSource[] audios;

    void Awake()
    {
    }

    void OnEnable()
    {
        ThemeManager.OnThemeChangedEvent += OnThemeChangedEvent;
        bool isDarkTheme = PlayerPrefs.GetInt("isDarkTheme", ((ThemeManager.instance.isDarkTheme == true ? 0 : 1))) == 0 ? true : false;
        OnThemeChangedEvent(isDarkTheme);
    }

    void OnDisable()
    {
        ThemeManager.OnThemeChangedEvent -= OnThemeChangedEvent;
    }

    void OnThemeChangedEvent(bool isDarkTheme)
    {
        if (isDarkTheme)
        {
            audios[0].volume = 0;
            audios[1].volume = 1;
        }
        else
        {
            audios[0].volume = 1;
            audios[1].volume = 0;
        }
    }
}
