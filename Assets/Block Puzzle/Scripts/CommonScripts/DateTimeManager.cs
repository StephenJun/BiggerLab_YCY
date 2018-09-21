using UnityEngine;
using System.Collections;
using System;

public class DateTimeManager : MonoBehaviour {

	public static DateTimeManager instance;

	[HideInInspector]	public int lastOpenedDays = 0;

	void Awake()
	{
		if (instance == null) {
			instance = this;
			return;
		}
		Destroy (gameObject);
	}

	void Start () {
	
		UpdateDateData ();
	}
	
	void UpdateDateData()
	{
		if (PlayerPrefs.GetString ("firstOpenedDate", string.Empty) == string.Empty) {
			PlayerPrefs.SetString ("firstOpenedDate", DateTime.Now.Date.ToString ());
		}
		//DateTime firstOpenDate = DateTime.Parse(PlayerPrefs.GetString("firstOpenedDate",DateTime.Now.Date.ToString()));
		DateTime lastOpenedDate = DateTime.Parse(PlayerPrefs.GetString("lastOpenedDate",DateTime.Now.Date.ToString()));
		DateTime currentDate = DateTime.Now.Date;
		lastOpenedDays = (currentDate - lastOpenedDate).Days;
		PlayerPrefs.SetString ("lastOpenedDate", DateTime.Now.Date.ToString ());
	}
}
