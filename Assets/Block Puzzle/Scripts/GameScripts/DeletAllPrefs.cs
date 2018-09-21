using UnityEngine;
using System.Collections;

public class DeletAllPrefs : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll ();
	}
}
