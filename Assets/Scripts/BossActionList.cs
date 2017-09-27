using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections;
using System.IO;
using System;

[Serializable]
public class BossActionList : MonoBehaviour
{
	public class Row
	{
		public string id;
		public string title;
		public string baseDamage;
		public string maxTargets;
	}


	[SerializeField]
	List<Row> rowList = new List<Row>();
	bool isLoaded = false;

	TextAsset file;

	[SerializeField]
	public List<BossAction> list;


	void Awake ()
	{
		rowList = new List<Row>();
		file = Resources.Load ("potus-actions") as TextAsset;
		Load (file);
		init ();
	}

	public void init()
	{
		list = new List<BossAction> ();

		// Go through row count
		for(int i=0;i<rowList.Count;i++)
		{
			int id;
			if(Int32.TryParse(rowList[i].id, out id))
			{
				BossAction ba = new BossAction ();
				ba.id = id;

				ba.title = rowList [i].title;

				Int32.TryParse (rowList [i].baseDamage, out ba.baseDmg);

				Int32.TryParse (rowList [i].maxTargets, out ba.maxTargets);

				list.Add (ba);
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
			row.baseDamage = grid[i][2];
			row.maxTargets = grid[i][3];

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
	public Row Find_baseDamage(string find)
	{
		return rowList.Find(x => x.baseDamage == find);
	}
	public List<Row> FindAll_baseDamage(string find)
	{
		return rowList.FindAll(x => x.baseDamage == find);
	}
	public Row Find_maxTargets(string find)
	{
		return rowList.Find(x => x.maxTargets == find);
	}
	public List<Row> FindAll_maxTargets(string find)
	{
		return rowList.FindAll(x => x.maxTargets == find);
	}

}

