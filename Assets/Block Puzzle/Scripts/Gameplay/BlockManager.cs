using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

public class BlockManager : MonoBehaviour 
{
	public int TotalRows = 10;
	public int TotalColumns = 10;
	public List<BlockData> BlockList = new List<BlockData>();
	public static BlockManager instance;
	public Color blockColor;
    public Sprite blockSprite;
	public Color[] ThemeColors;

	void Awake()
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

	void Start()
	{
        blockColor = (ThemeManager.instance.isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
		InitializeBlocks ();
	}

	/// <summary>
	/// Initializes the blockList.
	/// </summary>
	public void InitializeBlocks()
	{
		Transform obj;
		int blockId = 0;
		for(int i = 0;i<TotalRows;i++)
		{
			for(int j = 0;j<TotalColumns;j++)
			{
				obj = transform.Find ("Block_" + i + "_" + j);
	
				if (obj != null) 
				{
					obj.GetComponent<Image>().color = blockColor;
                    BlockList.Add(new BlockData(blockId, i, j, false, obj.GetComponent<Image>()));
                }
				blockId++;
			}
		}
	}

	/// <summary>
	/// ReInitializes the blockList.
	/// </summary>
	public void ReInitializeBlocks()
	{
		Transform obj;
		int blockId = 0;
		BlockList.Clear ();
		for(int i = 0;i<TotalRows;i++)
		{
			for(int j = 0;j<TotalColumns;j++)
			{
				obj = transform.Find ("Block_" + i + "_" + j);
				if (obj != null) {
					BlockList.Add (new BlockData (blockId, i, j, false, obj.GetComponent<Image> ()));
					obj.GetComponent<Image> ().color = blockColor;
                    obj.GetComponent<Image>().sprite = blockSprite;
					blockId++;
				}
			}
		}
	}


	void OnEnable()
	{
		ThemeManager.OnThemeChangedEvent += OnThemeChangedEvent;
	}

	void OnDisable()
	{
		ThemeManager.OnThemeChangedEvent -= OnThemeChangedEvent;
	}

	void OnThemeChangedEvent (bool isDarkTheme)
	{
		blockColor = (isDarkTheme) ? ThemeColors [0] : ThemeColors [1];
		foreach (Transform t in transform) {
			Image img = t.GetComponent<Image> ();
			if (img.color == ThemeColors [0] || img.color == ThemeColors [1]) {
				t.GetComponent<Image> ().color = blockColor;
			}
		}
	}
}

public class BlockData
{
	public int blockId;
	public int rowId;
	public int columnId;
	public bool isFilled;
	public Image block;

	public BlockData (int blockId, int rowId, int columnId, bool isFilled,Image block)
	{
		this.blockId = blockId;
		this.rowId = rowId;
		this.columnId = columnId;
		this.isFilled = isFilled;
		this.block = block;
	}	
}
