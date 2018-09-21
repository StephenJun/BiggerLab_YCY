using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameVictory : MonoBehaviour {

    GameMode mode;
    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        GameController.instance.PushWindow(gameObject);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    /// <summary>
    /// This will navigate to home screen.
    /// </summary>
    public void OnHomeButtonPressed()
    {
        if (InputManager.instance.canInput())
        {
            AudioManager.instance.PlayButtonClickSound();
            GameController.instance.OnCloseButtonPressed();
            Destroy(GameController.instance.WindowStack.Pop());
            GameController.instance.SpawnUIScreen("MainScreen", true);
        }
    }

    /// <summary>
    /// Put code here to sharing game, score etc on social network.
    /// </summary>
    public void OnShareButtonPressed()
    {
        if (InputManager.instance.canInput())
        {
            AudioManager.instance.PlayButtonClickSound();
            //UM_ShareUtility.ShareMedia ("Caption Goes Here", "Message Goes Here..");
        }
    }

    /// <summary>
    /// Put your code here to open the leaderboard.
    /// </summary>
    public void OnLeaderboardButtonPressed()
    {
        if (InputManager.instance.canInput())
        {
            AudioManager.instance.PlayButtonClickSound();
            //Debug.Log ("Leaderbord open stuff goes here..");

            switch (mode)
            {
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
    public void OnCloseButtonPressed()
    {
        if (InputManager.instance.canInput())
        {
            AudioManager.instance.PlayButtonClickSound();
            GameController.instance.OnCloseButtonPressed();
        }
    }

    /// <summary>
    /// Will restart the game.
    /// </summary>
    public void OnNextLevelButtonPressed()
    {
        if (InputManager.instance.canInput())
        {
            AudioManager.instance.PlayButtonClickSound();
            BlockTrayManager.instance.ResetGame();
            GameController.instance.OpenNextLevel();
            //BlockManager.instance.InitializeBlocks();
        }
    }
}
