using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// Game mode.
/// </summary>
public enum GameMode
{
	classic = 0
}

/// <summary>
/// Game play.
/// </summary>
public class GamePlay : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler,IBeginDragHandler
{
	public static GamePlay instance;


    public int levelNum;
    public Text txtLevel;
	public Text txtGameOverReason;

	public Transform sampleBlock;

	bool blockSelected = false;
	RectTransform SelectedObject;
	Transform LastCheckedBlock;
	public Color blockColor = new Color (0.77F, 0.82F, 0.87F, 1.00F);
	List<Image> hintColorBlocks = new List<Image> ();
	public static GameMode GamePlayMode;

	public AudioClip SFX_BlockPlace;
	public AudioClip SFX_GameOver;

	public AudioClip[] SFX_SingleLine;
	public AudioClip[] SFX_DoubleLine;
	public AudioClip[] SFX_MultiLine;

	public int TotalMoves = 0;

	public Sprite dynamiteImage,blockImage;

	/// <summary>
	/// Column List For hexa Mode
	/// </summary>
	public List<RowColumnData> column1List;
	public List<RowColumnData> column2List;


	bool isGamePaused = false;
	public List<Color> ThemeColors;
	//public Button btnUndo;


	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
        if (instance != null)
        {
            if (instance.gameObject != this.gameObject)
            {
                instance = null;
            }
        }
        if (instance == null)
        {
            instance = this;
        }
    }


    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start ()
	{
		EnableSettingsMenu ();
        txtLevel.text = "LEVEL " + levelNum;

		blockColor = (ThemeManager.instance.isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
    }

	public void TogglePauseGame(bool paused)
	{
		isGamePaused = paused;
	}


	/// <summary>
	/// Raises the undo button pressed event.
	/// </summary>
	public void OnUndoButtonPressed	()
	{
		AudioManager.instance.PlayButtonClickSound ();
	}

    IEnumerator CheckNearSameBlock(int chkNum)
    {
        List<BlockData> BlocksToDestroy = new List<BlockData>();
        foreach(var block in BlockManager.instance.BlockList)
        {
            if(block.numOfBlockTray == chkNum)
            {
                BlocksToDestroy.Add(block);
            }
        }
        if(BlocksToDestroy.Count > 0)
        {
            foreach (BlockData d in BlocksToDestroy)
            {
                GameObject currentBlock = (GameObject)Instantiate(sampleBlock.gameObject);
                currentBlock.transform.SetParent(sampleBlock.parent);
                currentBlock.transform.localScale = Vector3.one;
                currentBlock.GetComponent<RectTransform>().position = d.block.GetComponent<RectTransform>().position;
                currentBlock.GetComponent<Image>().color = d.block.color;
                currentBlock.GetComponent<Image>().sprite = d.block.sprite;
                currentBlock.gameObject.SetActive(true);
                d.isFilled = false;
                d.block.color = blockColor;
                d.block.sprite = blockImage;
                d.numOfBlockTray = -1;

                yield return new WaitForSeconds(0.01F);
            }
            BlockTrayManager.instance.RespawnBlockAfterDelete(chkNum);
        }

    }

    #region IPointerDownHandler implementation

    /// <summary>
    /// Raises the pointer down event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            if (clickedObject.name.Contains("Block_"))
            {
                string[] id = clickedObject.name.Split('_');
                if (id.Length >= 3)
                {
                    int rowId = id[1].TryParseInt(-1);
                    int columnId = id[2].TryParseInt(-1);
                    if (rowId != -1 && columnId != -1)
                    {
                        BlockData data = BlockManager.instance.BlockList.Find(o => o.rowId == rowId && o.columnId == columnId);
                        if(data != null)
                        {
                            StartCoroutine(CheckNearSameBlock(data.numOfBlockTray));
                        }
                    }
                }
            }
            if (clickedObject.name.Contains("objcontainer"))
            {
                if (clickedObject.transform.childCount > 0)
                {
                    blockSelected = true;
                    SelectedObject = clickedObject.transform.GetChild(0).GetComponent<RectTransform>();
                    SelectedObject.Find("blocksContainer").localScale = Vector3.one;
                    SelectedObject.localScale = new Vector3(0.9f, 0.9f, 0.9f);

                    Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
                    pos.z = SelectedObject.position.z;
                    SelectedObject.position = pos;
                }
            }
        }
    }

    #endregion

    //int layerMask = 1 << LayerMask.NameToLayer ("yer");
    int layerMask = 1;
	#region IBeginDragHandler implementation

	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag (PointerEventData eventData)
	{
		if (blockSelected && SelectedObject != null) {
			Vector3 pos = Camera.main.ScreenToWorldPoint (eventData.position);  //AA
			pos.z = SelectedObject.position.z;
			SelectedObject.position = pos;

			RaycastHit2D hit = Physics2D.Raycast (SelectedObject.GetComponent<Block> ().ObjectDetails.ColliderObject.position, Vector2.zero, layerMask);
			if (hit.collider != null) {
				if (hit.collider.name.Contains ("Block_")) {
					if (LastCheckedBlock != hit.collider.transform) {
						LastCheckedBlock = hit.collider.transform;
						CheckForHintBlocks (hit.collider.transform);
					}
				} else {
					ResetBlockColor ();
				}
			} else {
				ResetBlockColor ();
			}
		}
	}

	#endregion

	#region IDragHandler implementation

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag (PointerEventData eventData)
	{
		if (blockSelected && SelectedObject != null) {

			Vector3 pos = Camera.main.ScreenToWorldPoint (eventData.position);
			pos.z = SelectedObject.position.z;
			SelectedObject.position = pos;

			RaycastHit2D hit = Physics2D.Raycast (SelectedObject.GetComponent<Block> ().ObjectDetails.ColliderObject.position, Vector2.zero, layerMask);
			if (hit.collider != null) {
				if (hit.collider.name.Contains ("Block_")) {
					if (LastCheckedBlock != hit.collider.transform) {
						LastCheckedBlock = hit.collider.transform;
						CheckForHintBlocks (hit.collider.transform);
					}
				} else {
					ResetBlockColor ();
				}
			} else {
				ResetBlockColor ();
			}
		}
	}

	#endregion

	#region IPointerUpHandler implementation

	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerUp (PointerEventData eventData)
	{
		ResetBlockColor ();
		if (blockSelected) {
			if (SelectedObject != null) {
				//int layerMask = 1 << LayerMask.NameToLayer ("objBlockLayer");

				int layerMask = 1;

				RaycastHit2D hit = Physics2D.Raycast (SelectedObject.GetComponent<Block> ().ObjectDetails.ColliderObject.position, Vector2.zero, layerMask);
				if (hit.collider != null) {
					if (hit.collider.name.Contains ("Block_")) {
						bool CanplaceObject = CheckForEmptyBlocks (hit.collider.transform);
						if (CanplaceObject) {
							StartCoroutine ("PlaceObject", hit.collider.transform);		
						} else {
							SelectedObject.localScale = new Vector3 (0.4f, 0.4f, 1);
							SelectedObject.GetComponent<Block> ().ResetScaling ();
							SelectedObject.anchoredPosition = Vector2.zero;
						}
					} else {
						SelectedObject.localScale = new Vector3 (0.4f, 0.4f, 1);
						SelectedObject.GetComponent<Block> ().ResetScaling ();
						SelectedObject.anchoredPosition = Vector2.zero;
					}
				} else {
					SelectedObject.localScale = new Vector3 (0.4f, 0.4f, 1);
					SelectedObject.GetComponent<Block> ().ResetScaling ();
					SelectedObject.anchoredPosition = Vector2.zero;
				}
			}
		}
	}

	#endregion

	/// <summary>
	/// Places the object on given block.
	/// </summary>
	/// <param name="BlockToCheck">Block to check.</param>
	IEnumerator PlaceObject (Transform BlockToCheck)
	{
		if (BlockToCheck != null) {
			string[] id = BlockToCheck.name.Split ('_');
			if (id.Length >= 3) {
				int rowId = id [1].TryParseInt (-1);
				int columnId = id [2].TryParseInt (-1);
				if (rowId != -1 && columnId != -1) {
					BlockData data = BlockManager.instance.BlockList.Find (o => o.rowId == rowId && o.columnId == columnId);
					if (data != null) {
						if (!data.isFilled) {
							//Color ColorToSet = SelectedObject.GetComponent<Block> ().ObjectDetails.blockColor;
                            Sprite SpriteToSet = SelectedObject.GetComponent<Block>().ObjectDetails.blockSprite;
                            int numOfOrder = int.Parse(SelectedObject.transform.parent.name.Replace("objcontainer", "")) - 1;
							List<BlockShapeDetails> ObjectBlocks = SelectedObject.GetComponent<Block> ().ObjectDetails.objectBlocksids;
							SelectedObject.transform.position = data.block.transform.position;
							foreach (BlockShapeDetails s  in ObjectBlocks) {
								BlockData chkBlock = BlockManager.instance.BlockList.Find (o => o.rowId == (rowId + s.rowID) && o.columnId == (columnId + s.columnId));
								if (chkBlock != null && !chkBlock.isFilled) {
									//chkBlock.block.color = ColorToSet;
                                    chkBlock.block.sprite = SpriteToSet;
									chkBlock.isFilled = true;
                                    chkBlock.numOfBlockTray = numOfOrder;
								}
							}
							AudioManager.instance.PlayOneShotClip (SFX_BlockPlace);                          
						}
					}
				}
			}
		}

		blockSelected = false;
		Destroy (SelectedObject.gameObject);
		SelectedObject = null;
		yield return StartCoroutine( CheckForRowColumn());

		CheckForRestObjectPlacing ();
		TotalMoves++;
	}

	/// <summary>
	/// Checks for row column Filled or Not.
	/// </summary>
	IEnumerator CheckForRowColumn ()
	{
		List<List<BlockData>> BlocksToDestroy = new List<List<BlockData>> ();
		
		//CheckColumns
		int Count = 0;
        for (int i = 0; i < BlockManager.instance.TotalColumns; i++)
        {
            List<BlockData> blocklist = BlockManager.instance.BlockList.FindAll(o => o.rowId <= BlockManager.instance.TotalRows && o.columnId == i);
            if (blocklist != null && blocklist.Count > 0)
            {
                if (blocklist.FindAll(o => o.isFilled == false).Count == 0)
                {
                    Count++;
                    BlocksToDestroy.Add(blocklist);
                }
            }
        }
        //CheckRows
        for (int i = 0; i < BlockManager.instance.TotalRows; i++) {
			List<BlockData> blocklist = BlockManager.instance.BlockList.FindAll (o => o.rowId == i && o.columnId <= BlockManager.instance.TotalColumns);
			if (blocklist  != null && blocklist.Count > 0) {
				if (blocklist.FindAll (o => o.isFilled == false).Count == 0) {
					Count++;
					BlocksToDestroy.Add(blocklist);
				}
			}
		}
		
		if(BlocksToDestroy != null && BlocksToDestroy.Count == BlockManager.instance.TotalRows + BlockManager.instance.TotalColumns)
		{
			if (Count == 1)
			{
				if (SFX_SingleLine.Length > 0) {
					AudioManager.instance.PlayOneShotClip (SFX_SingleLine[UnityEngine.Random.Range(0,SFX_SingleLine.Length)]);
				}
			} 
			else if (Count == 2)
			{
				if (SFX_DoubleLine.Length > 0) {
					AudioManager.instance.PlayOneShotClip (SFX_DoubleLine[UnityEngine.Random.Range(0,SFX_DoubleLine.Length)]);
				}
			} 
			else if (Count >= 3)
			{
				if (SFX_MultiLine.Length > 0) {
					AudioManager.instance.PlayOneShotClip (SFX_MultiLine[UnityEngine.Random.Range(0,SFX_MultiLine.Length)]);
				}
			}

			for (int i = 0; i < BlocksToDestroy.Count; i++) 
			{
				foreach (BlockData d in BlocksToDestroy[i]) 
				{
					GameObject currentBlock = (GameObject)Instantiate (sampleBlock.gameObject);
					currentBlock.transform.SetParent (sampleBlock.parent);
					currentBlock.transform.localScale = Vector3.one;
					currentBlock.GetComponent<RectTransform> ().position = d.block.GetComponent<RectTransform> ().position;
					currentBlock.GetComponent<Image> ().color = d.block.color;
                    currentBlock.GetComponent<Image>().sprite = d.block.sprite;
					currentBlock.gameObject.SetActive (true);
					d.isFilled = false;
					d.block.color = blockColor;
                    d.block.sprite = blockImage;

					yield return new WaitForSeconds (0.01F);
				}
			}
            if(levelNum == 5)
            {
                StartCoroutine(LoadGameOver("More levels to expect, please wait!!"));
            }
            else
            {
                StartCoroutine(LoadGameVictory());
            }
        }
	}

	/// <summary>
	/// Checks for rest object placing.
	/// </summary>
	void CheckForRestObjectPlacing ()
	{
		Transform nextBlocks = transform.Find ("GamePlay-Content").Find ("NextBlocks-Container");
		List<RectTransform> objectToplace = new List<RectTransform> ();

		foreach (Transform nextBlock in nextBlocks) {
			if (nextBlock.childCount > 0) {
				objectToplace.Add (nextBlock.GetChild (0).GetComponent<RectTransform> ());
			}
		}

		List<bool> CanPlaceObject = new List<bool> ();
		if (objectToplace.Count > 0) {
			foreach (RectTransform s in objectToplace) {
				SelectedObject = s;
				foreach (BlockData d in BlockManager.instance.BlockList) {
					if (!d.isFilled) {
						if (!CheckForEmptyBlocks (d.block.transform)) {
							CanPlaceObject.Add (false);
						} else {
							CanPlaceObject.Add (true);
							break;
						}
					}
				}

				if (CanPlaceObject.FindIndex (o => o == true) != -1) {
					break;
				}
			}

			if(CanPlaceObject.FindIndex(o=>o == true) == -1)
			{
				StartCoroutine(LoadGameOver("No Space to fit any block!"));
			}
		}
	}


	IEnumerator LoadGameOver(string reason)
	{
		txtGameOverReason.text = reason.ToString ();
		txtGameOverReason.transform.parent.gameObject.ScaleTo (EGTween.Hash ("X", 1, "Y", 1, "Z",1, "Time", 0.7F));
		yield return new WaitForSeconds (2F);
		txtGameOverReason.transform.parent.gameObject.ScaleTo (EGTween.Hash ("X", 0, "Y", 0, "Z",0, "Time", 0.7F));
		yield return new WaitForSeconds (0.4F);

		GameController.instance.SpawnUIScreen ("GameOver", true).GetComponent<GameOver> ().setScore (GamePlayMode, reason);
		AudioManager.instance.PlayOneShotClip (SFX_GameOver);

	}

    IEnumerator LoadGameVictory()
    {
        yield return new WaitForSeconds(0.4F);

        GameController.instance.SpawnUIScreen("GameVictory", true).GetComponent<GameVictory>();
        AudioManager.instance.PlayOneShotClip(SFX_GameOver);
    }

	/// <summary>
	/// Checks for empty blocks.
	/// Check object is placed on this block or not
	/// </summary>
	/// <returns><c>true</c>, if empty blocks was checked, <c>false</c> otherwise.</returns>
	/// <param name="BlockToCheck">Block to check.</param>
	bool CheckForEmptyBlocks (Transform BlockToCheck)
	{
		bool canPlaceBlock = false;
		if (BlockToCheck != null) {
			string[] id = BlockToCheck.name.Split ('_');
			if (id.Length >= 3) {
				int rowId = id [1].TryParseInt (-1);
				int columnId = id [2].TryParseInt (-1);

				if (rowId != -1 && columnId != -1) {
					BlockData data = BlockManager.instance.BlockList.Find (o => o.rowId == rowId && o.columnId == columnId);
					if (data != null) {
						if (!data.isFilled) {
							List<BlockShapeDetails> ObjectBlocks = SelectedObject.GetComponent<Block> ().ObjectDetails.objectBlocksids;
							bool BlockFilled = false;
							foreach (BlockShapeDetails s  in ObjectBlocks) {
								BlockData chkBlock = BlockManager.instance.BlockList.Find (o => o.rowId == (rowId + s.rowID) && o.columnId == (columnId + s.columnId));
								if (chkBlock != null && !chkBlock.isFilled) {
									
								} else {
									BlockFilled = true;
									break;
								}
							}

							canPlaceBlock = !BlockFilled;
						}
					}
				}
			}
		}

		return canPlaceBlock;
	}

	/// <summary>
	/// Resets the color of the block.
	/// </summary>
	void ResetBlockColor ()
	{
		if (hintColorBlocks != null && hintColorBlocks.Count > 0) {
			foreach (Image i in hintColorBlocks) {
				i.color = blockColor;
                i.sprite = blockImage;
			}
			hintColorBlocks.Clear ();
		}
	}

	/// <summary>
	/// Checks for empty blocks.
	/// Check object is placed on this block or not
	/// </summary>
	/// <returns><c>true</c>, if empty blocks was checked, <c>false</c> otherwise.</returns>
	/// <param name="BlockToCheck">Block to check.</param>
	void CheckForHintBlocks (Transform BlockToCheck)
	{
		ResetBlockColor ();
		if (BlockToCheck != null) {
			string[] id = BlockToCheck.name.Split ('_');

			if (id.Length >= 3) {
				int rowId = id [1].TryParseInt (-1);
				int columnId = id [2].TryParseInt (-1);

				if (rowId != -1 && columnId != -1) {
					BlockData data = BlockManager.instance.BlockList.Find (o => o.rowId == rowId && o.columnId == columnId);

					if (data != null) {
						if (!data.isFilled) {
							Block objManager = SelectedObject.GetComponent<Block> ();
							List<BlockShapeDetails> ObjectBlocks = objManager.ObjectDetails.objectBlocksids;

							bool BlockFilled = false;
							foreach (BlockShapeDetails s  in ObjectBlocks) {
								BlockData chkBlock = BlockManager.instance.BlockList.Find (o => o.rowId == (rowId + s.rowID) && o.columnId == (columnId + s.columnId));
								if (chkBlock != null && !chkBlock.isFilled) {
									hintColorBlocks.Add (chkBlock.block.GetComponent<Image>());
								} else {
									hintColorBlocks.Clear ();
									BlockFilled = true;
									break;
								}
							}

							if (!BlockFilled && hintColorBlocks != null && hintColorBlocks.Count > 0) {
								foreach (Image i in hintColorBlocks) {
									//i.color = new Color (objManager.ObjectDetails.blockColor.r, objManager.ObjectDetails.blockColor.g, objManager.ObjectDetails.blockColor.b, 0.5f);
                                    i.sprite = objManager.ObjectDetails.blockSprite;
                                }
							}
						}
					}
				}
			}
		}
	}


	void EnableSettingsMenu()
	{
		if (SettingsContent.instance.settings_gameplay != null) {
			SettingsContent.instance.settings_gameplay.gameObject.SetActive(true);
			SettingsContent.instance.transform.SetAsLastSibling();
		}
	}
	
	void DisableSettingsMenu() {
		if (SettingsContent.instance.settings_gameplay != null) {
			SettingsContent.instance.settings_gameplay.gameObject.SetActive(false);
		}
	}

	void OnEnable()
	{
		ThemeManager.OnThemeChangedEvent += OnThemeChangedEvent;
	}

	void OnDisable()
	{
		ThemeManager.OnThemeChangedEvent -= OnThemeChangedEvent;
		DisableSettingsMenu ();
	}


	void OnThemeChangedEvent (bool isDarkTheme)
	{
		blockColor = (isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
	}

}


[System.Serializable]
public class RowColumnData
{
	public int RowStartIndex;
	public int AddtoRow;
	[Space]
	public int columnStartIndex;
	public int AddtoColumn;
	[Space]
	public int rowLastElement;
	public int columnLastElement;

}