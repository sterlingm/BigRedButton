using UnityEngine;
using System.Collections.Generic;
using System;

// CSV loading stuff is copied over from CSV2Table asset in asset store
public class TopicList : MonoBehaviour
{
	public List<Row> rowList;
	bool isLoaded = false;

	[SerializeField]
	TextAsset file;

	[SerializeField]
	public List<Topic> list;

	// From Csv2Table
	public class Row
	{
		public string id;
		public string title;
		public string type;
		public string baseDamage;
	}

	void Awake ()
	{
		rowList = new List<Row>();
		file = Resources.Load ("topics") as TextAsset;
		Load (file);
		init ();
	}

	/*
	 * Initialize the list
	 */ 
	public void init()
	{
		list = new List<Topic> ();
		for(int i=0;i<rowList.Count;i++)
		{
			int id, type;
			double baseDmg;
			if(Int32.TryParse(rowList[i].id, out id) && Int32.TryParse(rowList[i].type, out type) && Double.TryParse(rowList[i].baseDamage, out baseDmg))
			{
				// Get TopicType from the list
				Common.TopicType topicType 	= Common.topicTypeList [Int32.Parse (rowList [i].type)];

				// Initialize a topic
				Topic t = new Topic ();
				t.init (id, rowList[i].title, topicType, baseDmg);

				// Add to list
				list.Add (t);
			}
			else
			{
				Debug.LogError ("Problem with  file, row must be in format: int, string, int, float");
				Debug.LogError (String.Format("rowList[{0}]: {1}, {2}, {3}, {4}", 
					i, rowList[i].id, rowList[i].title, rowList[i].type, rowList[i].baseDamage));
			}
		}

		Debug.Log (String.Format("TopicList initialized with {0} elements", list.Count));
	}


	/*
	 ******************************************************************
	 *                       Csv2Table code below
	 ******************************************************************
	 */ 
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
			row.type 		= grid[i][2];
			row.baseDamage 	= grid[i][3];

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

	public Row Find_type(string find)
	{
		return rowList.Find(x => x.type == find);
	}
	public List<Row> FindAll_type(string find)
	{
		return rowList.FindAll(x => x.type == find);
	}

	public Row Find_baseDamage(string find)
	{
		return rowList.Find(x => x.baseDamage == find);
	}
	public List<Row> FindAll_baseDamage(string find)
	{
		return rowList.FindAll(x => x.baseDamage == find);
	}

}

