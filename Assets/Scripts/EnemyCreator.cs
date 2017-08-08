using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using UnityEngine.Networking.NetworkSystem;

public class EnemyCreator : MonoBehaviour
{


	// From Csv2Table
	public class Row
	{
		public string id;
		public string name;
		public string type;
		public string x_pos;
		public string y_pos;
		public List<string> responses = new List<string> ();
		public List<string> topics = new List<string> ();
	}

	[SerializeField]
	TextAsset file;
	private int i_startResps = 5;
	public List<Enemy> list;
	public List<Row> rowList;
	bool isLoaded = false;


	public Enemy enemyPrefab;

	[SerializeField]
	public List<Enemy> enemyList;

	// Use this for initialization
	void Start ()
	{
		enemyList = new List<Enemy> ();
		rowList = new List<Row>();
		file = Resources.Load ("enemies") as TextAsset;
		Load (file);
		init ();
	}

	/*
	 * Initialize the list
	 */ 
	public void init()
	{

		for(int i=0;i<rowList.Count;i++)
		{
			int id;
			if(Int32.TryParse(rowList[i].id, out id))
			{
				string name = rowList [i].name;
				int type;
				Int32.TryParse (rowList [i].type, out type);

				Vector3 p = new Vector3 ();
				float.TryParse (rowList [i].x_pos, out p.x);
				float.TryParse (rowList [i].y_pos, out p.z);

				Enemy e = Instantiate (enemyPrefab, p, Quaternion.identity) as Enemy;
				Debug.Log (String.Format ("p: {0}", p.ToString ()));
				//e.transform.position = p;

				//e.type = type;

				Char delimiter = ' ';
				for(int j=0;j<rowList[i].responses.Count;j++)
				{
					// Make EnemyResponse
					EnemyResponse er = new EnemyResponse ();
					er.init (j, rowList[i].responses[j]);
					er.i_topic = j;

					string topsStr = rowList [i].topics [j];
					String[] tops = topsStr.Split (delimiter);
					Debug.Log ("rowList[" + i + "] Topics "+j+" topsStr " + topsStr);
					foreach(string t in tops)
					{
						int topic;
						Int32.TryParse (t, out topic);
						er.topicsToObtain.Add (topic);
					}

					e.AddResponse (er);
				}
				enemyList.Add (e);
			}
		}

		//Debug.Log (String.Format("TopicList initialized with {0} elements", list.Count));
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
			row.id = grid[i][0];
			row.name = grid[i][1];
			row.type = grid[i][2];
			row.x_pos = grid[i][3];
			row.y_pos = grid[i][4];

			// Load in response strings
			for(int j=i_startResps;j<grid[i].Length;j+=2)
			{
				row.responses.Add (grid [i] [j]);
			}

			// Load in topics
			for(int j=i_startResps+1;j<grid[i].Length;j+=2)
			{
				row.topics.Add (grid [i] [j]);
			}

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
	public Row Find_name(string find)
	{
		return rowList.Find(x => x.name == find);
	}
	public List<Row> FindAll_name(string find)
	{
		return rowList.FindAll(x => x.name == find);
	}
	public Row Find_type(string find)
	{
		return rowList.Find(x => x.type == find);
	}
	public List<Row> FindAll_type(string find)
	{
		return rowList.FindAll(x => x.type == find);
	}


}