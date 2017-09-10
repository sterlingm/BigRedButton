using UnityEngine;
using System.Collections.Generic;

public class PlayerActionList : MonoBehaviour
{

	public class Row
	{
		public string id;
		public string title;
		public string base_damage;
		public string action_type;
		public string ally_type;

	}



	List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	void Start ()
	{

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
			row.base_damage = grid[i][2];
			row.action_type = grid[i][3];
			row.ally_type = grid[i][4];

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
		return rowList.Find(x => x.base_damage == find);
	}
	public List<Row> FindAll_baseDamagePlayerActionList(string find)
	{
		return rowList.FindAll(x => x.base_damage == find);
	}
	public Row Find_actionTypePlayerActionList(string find)
	{
		return rowList.Find(x => x.action_type == find);
	}
	public List<Row> FindAll_actionTypePlayerActionList(string find)
	{
		return rowList.FindAll(x => x.action_type == find);
	}
	public Row Find_allyTypePlayerActionList(string find)
	{
		return rowList.Find(x => x.ally_type == find);
	}
	public List<Row> FindAll_allyTypePlayerActionList(string find)
	{
		return rowList.FindAll(x => x.ally_type == find);
	}


}

