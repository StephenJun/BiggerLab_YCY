using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ThemeButtonHandler : MonoBehaviour {

    public List<Sprite> imageThemeSprites;
    public List<Sprite> highlightThemeSprites;
    Image image;
    Button button;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
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
        image.sprite = (isDarkTheme) ? imageThemeSprites[0] : imageThemeSprites[1];
        SpriteState newSpriteState = new SpriteState();
        newSpriteState.highlightedSprite = (isDarkTheme) ? highlightThemeSprites[0] : highlightThemeSprites[1]; 
        newSpriteState.pressedSprite = (isDarkTheme) ? highlightThemeSprites[0] : highlightThemeSprites[1];
        button.spriteState = newSpriteState;
    }
}
