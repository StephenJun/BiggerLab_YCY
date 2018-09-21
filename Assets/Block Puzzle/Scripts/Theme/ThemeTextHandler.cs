using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ThemeTextHandler : MonoBehaviour {

	public List<Color> ThemeColors;
	Text text;

	void Awake()
	{
		text = GetComponent<Text> ();
		OnThemeChangedEvent (ThemeManager.instance.isDarkTheme);
	}

	void OnEnable()
	{
		ThemeManager.OnThemeChangedEvent += OnThemeChangedEvent;
		OnThemeChangedEvent (ThemeManager.instance.isDarkTheme);
	}

	void OnDisable()
	{
		ThemeManager.OnThemeChangedEvent -= OnThemeChangedEvent;
	}

	void OnThemeChangedEvent (bool isDarkTheme)
	{
		if (text != null && ThemeColors.Count >= 2) {
			text.color = (isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
		}
	}
}
