using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ThemeSpriteHandler : MonoBehaviour {

	public List<Color> ThemeColors;
	Image image;

	void Awake()
	{
		image = GetComponent<Image> ();
		//OnThemeChangedEvent (ThemeManager.instance.isDarkTheme);
	}

	void OnEnable()
	{
		ThemeManager.OnThemeChangedEvent += OnThemeChangedEvent;
		bool isDarkTheme = PlayerPrefs.GetInt ("isDarkTheme", ((ThemeManager.instance.isDarkTheme == true ? 0 : 1))) == 0 ? true : false;
		OnThemeChangedEvent (isDarkTheme);
	}

	void OnDisable()
	{
		ThemeManager.OnThemeChangedEvent -= OnThemeChangedEvent;
	}

	void OnThemeChangedEvent (bool isDarkTheme)
	{
		image.color = (isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
	}
}
