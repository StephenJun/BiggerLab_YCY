using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public Text txtGameOverReason;

	GameMode mode;
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		GameController.instance.PushWindow (gameObject);
	}

	void OnEnable()
	{
	}
	
	void OnDisable()
	{
	}

	/// <summary>
	/// Set the score and best score.
	/// </summary>
	/// <param name="GamePlayMode">Game play mode.</param>
	/// <param name="score">Score.</param>
	/// <param name="bestScore">Best score.</param>
	public void setScore (GameMode GamePlayMode, string reason)
	{
		mode = GamePlayMode;
		txtGameOverReason.text = reason.ToString ();
		
		switch (GamePlayMode) {
		case GameMode.classic:
			break;
		}
	}

	/// <summary>
	/// This will navigate to home screen.
	/// </summary>
	public void OnHomeButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			GameController.instance.OnCloseButtonPressed ();
			Destroy (GameController.instance.WindowStack.Pop ());
			GameController.instance.SpawnUIScreen ("MainScreen", true);
		}
	}

	/// <summary>
	/// Put code here to sharing game, score etc on social network.
	/// </summary>
	public void OnShareButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			//UM_ShareUtility.ShareMedia ("Caption Goes Here", "Message Goes Here..");
		}
	}

	/// <summary>
	/// Put your code here to open the leaderboard.
	/// </summary>
	public void OnLeaderboardButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			//Debug.Log ("Leaderbord open stuff goes here..");

			switch (mode) {
			case GameMode.classic:
				break;
			//case GameMode.plus:
			//	break;
			//case GameMode.bomb:
			//	break;
			//case GameMode.hexa:
			//	break;
			}
		}
	}

	/// <summary>
	/// This will navigate to home screen.
	/// </summary>
	public void OnCloseButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			GameController.instance.OnCloseButtonPressed ();
		}
	}

	/// <summary>
	/// Will restart the game.
	/// </summary>
	public void OnRetryButtonPressed ()
	{
		if (InputManager.instance.canInput ()) {
			AudioManager.instance.PlayButtonClickSound ();
			BlockTrayManager.instance.ResetGame ();
			GameController.instance.RestartGamePlay ();
		}
	}
}
