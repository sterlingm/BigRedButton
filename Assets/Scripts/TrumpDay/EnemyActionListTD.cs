using UnityEngine;
using System.Collections.Generic;
using System;

// CSV loading stuff is copied over from CSV2Table asset in asset store
public class EnemyActionListTD : MonoBehaviour
{
    // Singleton
    public static EnemyActionListTD self;

	private List<Row> rowList;
	private bool isLoaded = false;

	[SerializeField]
	private TextAsset file;

	[SerializeField]
	public List<EnemyActionTD> list;

	// From Csv2Table
	private class Row
	{
		public string id;
		public string title;
		public string type;
		public string baseDamage;
        public string maxTargets;
	}

	void Awake ()
	{
        if(self == null)
        {
            self = this;
        }

		rowList = new List<Row>();
		file = Resources.Load ("TrumpDay/enemy-actions") as TextAsset;
		Load (file);
		Init ();
	}

	/*
	 * Initialize the list
	 */ 
	public void Init()
	{
		list = new List<EnemyActionTD> ();
		for(int i=0;i<rowList.Count;i++)
		{
			int id, type, baseDmg, maxTargets;
			if(Int32.TryParse(rowList[i].id, out id) && Int32.TryParse(rowList[i].type, out type) && Int32.TryParse(rowList[i].baseDamage, out baseDmg) && Int32.TryParse(rowList[i].maxTargets, out maxTargets))
			{
				// Get TopicType from the list
				Common.EnemyActionType enemyActType 	= Common.enemyActionTypeList[Int32.Parse (rowList [i].type)];

				// Initialize a topic
				EnemyActionTD t = new EnemyActionTD();
				t.init (id, rowList[i].title, enemyActType, baseDmg, maxTargets);

				// Add to list
				list.Add (t);
			}
			else
			{
				Debug.LogError ("Problem with  file, row must be in format: int, string, int, float");
				Debug.LogError (String.Format("rowList[{0}]: {1}, {2}, {3}, {4}, {5}", 
					i, rowList[i].id, rowList[i].title, rowList[i].type, rowList[i].baseDamage, rowList[i].maxTargets));
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
            row.maxTargets	= grid[i][4];

            rowList.Add(row);
		}
		isLoaded = true;
	}
    
}

