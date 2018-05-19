using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerActionListTD : MonoBehaviour
{
    // Singleton
    public static PlayerActionListTD self = null;

	public class Row
	{
		public string id;
		public string title;
		public string damage;
		public string actionType;
        public string encType;
    }


    [SerializeField]
    public List<PlayerActionTD> list;


    List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	TextAsset file;



	void Awake ()
	{
        if(self == null)
        {
            self = this;
        }
		rowList = new List<Row>();
		file = Resources.Load ("TrumpDay/player-actions") as TextAsset;
		Load (file);
		init ();
	}


	void init()
	{
		list = new List<PlayerActionTD> ();

		// Go through row count
		for(int i=0;i<rowList.Count;i++)
		{
			int id;
			if(Int32.TryParse(rowList[i].id, out id))
			{
                PlayerActionTD pa = new PlayerActionTD();
				pa.id = id;

				pa.title = rowList [i].title;

				Int32.TryParse (rowList [i].damage, out pa.baseDmg);

				Int32.TryParse (rowList [i].actionType, out pa.actionType);

                Int32.TryParse (rowList[i].encType, out pa.encType);

				list.Add (pa);
			}
			else
			{
				Debug.LogWarning ("An error occurerd reading POTUS actions");
			}
		}
		
	}



	public bool IsLoaded()
	{
		return isLoaded;
	}

	public List<Row> GetRowList()
	{
		return rowList;
	}

	public void Load(TextAsset csv)
	{
		rowList.Clear();
		string[][] grid = CsvParser2.Parse(csv.text);
		for(int i = 1 ; i < grid.Length ; i++)
		{
			Row row = new Row();
			row.id = grid[i][0];
			row.title = grid[i][1];
			row.damage = grid[i][2];
			row.actionType = grid[i][3];
            row.encType = grid[i][4];

            rowList.Add(row);
		}
		isLoaded = true;
	}

	public int NumRows()
	{
		return rowList.Count;
	}

	public Row GetAt(int i)
	{
		if(rowList.Count <= i)
			return null;
		return rowList[i];
	}

	public Row Find_id(string find)
	{
		return rowList.Find(x => x.id == find);
	}
	public List<Row> FindAll_id(string find)
	{
		return rowList.FindAll(x => x.id == find);
	}
	public Row Find_title(string find)
	{
		return rowList.Find(x => x.title == find);
	}
	public List<Row> FindAll_title(string find)
	{
		return rowList.FindAll(x => x.title == find);
	}
	public Row Find_baseDamagePlayerActionList(string find)
	{
		return rowList.Find(x => x.damage == find);
	}
	public List<Row> FindAll_baseDamagePlayerActionList(string find)
	{
		return rowList.FindAll(x => x.damage == find);
	}
	public Row Find_actionTypePlayerActionList(string find)
	{
		return rowList.Find(x => x.actionType == find);
	}
	public List<Row> FindAll_actionTypePlayerActionList(string find)
	{
		return rowList.FindAll(x => x.actionType == find);
	}


}

