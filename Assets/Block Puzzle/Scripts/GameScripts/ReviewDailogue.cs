using UnityEngine;
using System.Collections;

public class ReviewDailogue : MonoBehaviour 
{
	/// <summary>
	/// Raises the close button pressed event.
	/// </summary>
	public void OnYesButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();

			#if UNITY_ANDROID
			if(Constants.isAmazon) {
				Application.OpenURL(Constants.AmazonStoreURL);
			} else {	
				Application.OpenURL(Constants.PlayStoreURL);
			}
			#elif UNITY_IOS
			Application.OpenURL(Constants.AppStoreURL);
			#elif UNITY_EDITOR
			Application.OpenURL("http://www.epilexgames.com");
			#endif

			PlayerPrefs.SetInt ("HasReviewedApp", 1);
			GameController.instance.OnCloseButtonPressed ();

		}
	}

	/// <summary>
	/// Raises the ok button pressed event.
	/// </summary>
	public void OnNextTimeButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			GameController.instance.OnCloseButtonPressed ();
		}
	}
}
