using UnityEngine;
using System.Collections;

public class CaptureScreenshot : MonoBehaviour {

	int captureIndex = 0;
	void Start()
	{
		captureIndex = PlayerPrefs.GetInt ("captureIndex", 0);
	}

	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.C)) {
			//Application.CaptureScreenshot (Application.dataPath +  "/IMG-" + System.DateTime.Now.Date.ToString () + "-" + System.DateTime.Now.TimeOfDay.ToString () + ".png", 2);
			ScreenCapture.CaptureScreenshot(Application.dataPath + "/IMG" + captureIndex.ToString()+".PNG",2);
			captureIndex++;
			PlayerPrefs.SetInt ("captureIndex", captureIndex);
		}
	}
}
