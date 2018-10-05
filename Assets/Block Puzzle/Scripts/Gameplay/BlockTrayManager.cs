using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;

/// <summary>
/// Block tray manager.
/// This component have all the blocks that will be used during gameplay, it will spawn random blocks based on the probability of the blocks.
/// </summary>
public class BlockTrayManager : MonoBehaviour
{
	public static BlockTrayManager instance;
	public Transform blockContainer;
	public List<Transform> blockList;
	float blockTransitionTime = 0.5F;


	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		if (instance != null) {
            if(instance.gameObject != this.gameObject)
            {
                instance = null;
            }
		}
        if(instance == null)
        {
            instance = this;
        }
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		StartGame ();
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	public void StartGame ()
	{
		int placedBlocks = 0;

		if (placedBlocks == 0) 
		{
			OnPlacingBlock ();
		}
	}

	/// <summary>
	/// Resets the game For Replay level.
	/// </summary>
	public void ResetGame ()
	{
		BlockManager.instance.ReInitializeBlocks ();
		foreach (Transform block in blockContainer) {
			if (block.childCount > 0) {
				Destroy (block.GetChild (0).gameObject);
			}
		}	
		GamePlay.instance.TotalMoves = 0;

		Invoke ("OnPlacingBlock", 0.5f);

		PlayerPrefs.DeleteKey ("GameData");
	}

	/// <summary>
	/// Swaps the object.
	/// </summary>
	/// <param name="parentBlock">Parent block.</param>
	/// <param name="blockToTansit">Block to tansit.</param>
	public void swapObject (Transform parentBlock, Transform blockToTansit)
	{
		blockToTansit.SetParent (parentBlock);
		EGTween.MoveTo (blockToTansit.gameObject, EGTween.Hash ("isLocal", true, "x", 0, "time", blockTransitionTime));
	}

	/// <summary>
	/// Raises the placing block event.
	/// </summary>
	public void OnPlacingBlock ()
	{
		int blockRemained = 0;
		foreach (Transform t in blockContainer) 
		{
			if (t.childCount > 0) {
				blockRemained++;
			}
		}
        if (blockRemained == 0)
        {
            for (int i = 0; i < 9; i++)
            {
                GameObject obj = null;
                obj = (GameObject)Instantiate(blockList[i].gameObject);
                obj.transform.SetParent(blockContainer.GetChild(i).transform);
                obj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                obj.gameObject.SetActive(true);
            }
            EGTween.MoveFrom(blockContainer.gameObject, EGTween.Hash("isLocal", true, "x", 50, "time", blockTransitionTime));
        }
	}

    public void RespawnBlockAfterDelete(int index)
    {
        GameObject obj = null;
        obj = (GameObject)Instantiate(blockList[index].gameObject);
        obj.transform.SetParent(blockContainer.GetChild(index).transform);
        obj.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
        obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        obj.gameObject.SetActive(true);
    }

	/// <summary>
	/// Shuffles the generic list. 重新排序算法。
	/// </summary>
	/// <param name="list">List.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void ShuffleGenericList<T> (List<T> list)
	{  
		System.Random rng = new System.Random (); 
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next (n + 1);  
			T value = list [k];  
			list [k] = list [n];  
			list [n] = value;  
		}  
	}
}