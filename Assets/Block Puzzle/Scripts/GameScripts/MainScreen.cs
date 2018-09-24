using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using System.Xml.Linq;
using System.Linq;
using System;

public class MainScreen : MonoBehaviour
{
	//int ClickedMode = 0;
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		GameController.instance.PushWindow (gameObject);
		//UM_AdManager.Init ();
		//UM_AdManager.ShowBanner (UM_AdManager.CreateAdBanner (TextAnchor.LowerCenter));
		//UM_AdManager.StartInterstitialAd ();
	
		#if UNITY_EDITOR
		//PlayerPrefs.DeleteAll ();
		#endif


	}

	void OnEnable()
	{
		Invoke ("EnableSettingsMenu", 0.1F);
	}

	void OnDisable()
	{
	}

	/// <summary>
	/// Raises the play classic button pressed event.
	/// </summary>
	public void OnPlayClassicButtonPressed (int level)
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			GamePlay.GamePlayMode = GameMode.classic;
			//if (PlayerPrefs.GetInt ("Classic_playedBefore", 0) == 0) 
			//{
			//	GameController.instance.SpawnUIScreen ("Classic_HelpIntro", true);
			//	PlayerPrefs.SetInt("Classic_playedBefore", 1);
			//}
			//else
			//{
				
			//}
            GameController.instance.SpawnUIScreen("GamePlay" + level, true);

            DisableSettingsMenu();
		}
	}


    public void OnPlayButtonPressed(GameObject levelPanel)
    {
        EGTween.MoveTo(levelPanel, EGTween.Hash("isLocal", true, "x", 0, "time", 1.0f));
    }

    void EnableSettingsMenu()
	{
		if (SettingsContent.instance.settings_main != null) {
			SettingsContent.instance.settings_main.gameObject.SetActive(true);
			SettingsContent.instance.transform.SetAsLastSibling();
		}
	}

	void DisableSettingsMenu() {
		if (SettingsContent.instance.settings_main != null) {
			SettingsContent.instance.settings_main.gameObject.SetActive(false);
		}
	}
}