using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class AllyActionList : MonoBehaviour
{

	public class Row
	{
		public string id;
		public string title;
		public string baseDmg;
		public string allyType;
	}

	[SerializeField]
	List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	public TextAsset file;

	[SerializeField]
	public List<AllyAction> list;


	void Awake ()
	{
		rowList = new List<Row>();
		file = Resources.Load ("ally-actions") as TextAsset;
		Load (file);
		init ();
	}


	void init()
	{
		list = new List<AllyAction> ();

		// Go through row count
		for(int i=0;i<rowList.Count;i++)
		{
			int id;
			if(Int32.TryParse(rowList[i].id, out id))
			{
				AllyAction aa = new AllyAction ();
				aa.id = id;

				aa.title = rowList [i].title;

				Int32.TryParse (rowList [i].baseDmg, out aa.baseDmg);

				string[] types = rowList[i].allyType.Split (',');
				for(int j=0;j<types.Length;j++)
				{
					int value;
					Int32.TryParse (types [j], out value);
					aa.allyType.Add (value);
				}

				list.Add (aa);
			}
			else
			{
				Debug.LogWarning ("An error occurerd reading POTUS actions");
			}
		}	// end for each row
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
			row.id 			= grid[i][0];
			row.title 		= grid[i][1];
			row.baseDmg 	= grid[i][2];
			row.allyType 	= grid[i][3];
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
}

