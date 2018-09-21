using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RateButton : MonoBehaviour 
{
	//  The button to rate. Assigned from inspector.
	public Button btnRate;

	//Set to true if current store is amazon.
	public bool isAmazon = false;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		btnRate.onClick.AddListener(() => 
		                             {
			if (InputManager.instance.canInput ()) {
				AudioManager.instance.PlayButtonClickSound ();
			

				#if UNITY_ANDROID
				if(!isAmazon) {
					Application.OpenURL(Constants.PlayStoreURL);
				}
				else {
					Application.OpenURL(Constants.AmazonStoreURL);
				}
				#elif UNITY_IOS
					Application.OpenURL(Constants.AppStoreURL);
				#elif UNITY_EDITOR
				Application.OpenURL("http://www.epilexgames.com");
				#endif
			}
		});
	}
}
