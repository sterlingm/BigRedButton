using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.AI;

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
		public string movement_type;
		public string movement_details;
		public List<string> responses = new List<string> ();
		public List<string> topics = new List<string> ();
	}

	[SerializeField]
	TextAsset file;
	private int i_startResps = 7;
	public List<Enemy> list;
	public List<Row> rowList;
	bool isLoaded = false;


	public Enemy enemyPrefab;

	[SerializeField]
	public List<Enemy> enemyList;

	// Use this for initialization
	void Awake ()
	{
		enemyList = new List<Enemy> ();
		rowList = new List<Row>();
        file = Resources.Load("enemy-list") as TextAsset;
		Load (file);
		init ();
	}

	/*
	 * Parse the string and build a list of vector2 positions
	 */
	public List<Vector3> getMovementDetails(string csv_str)
	{
		List<Vector3> result = new List<Vector3> ();

		string[] strs = csv_str.Split (',');
		for(int i=0;i<strs.Length-1;i+=2)
		{
			int x, z;
			Int32.TryParse (strs [i], out x);
			Int32.TryParse (strs [i+1], out z);

			Vector3 pos = new Vector3 ();
			pos.x = x;
			pos.z = z;

			result.Add (pos);
		}

		return result;
	}

	/*
	 * Initialize the list
	 */ 
	public void init()
	{

        for (int i = 0; i < rowList.Count;i++)
		{
			// If an id exists, get all the other fields
			int id;
			if(Int32.TryParse(rowList[i].id, out id))
			{
				// Name
				string name = rowList [i].name;


				// Position
				Vector3 p = new Vector3 ();
				float.TryParse (rowList [i].x_pos, out p.x);
				float.TryParse (rowList [i].y_pos, out p.z);

				// Set y based on prefab y so that mesh is flush with the movement plane
				p.y = enemyPrefab.transform.position.y;

                // Get random point on navmesh for initial position
                float walkRadius = 40f;
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                Vector3 finalPosition = hit.position;

                Debug.Log(String.Format("p: {0} randomDirection: {1} finalPosition: {2}", p, randomDirection, finalPosition));

                // Create/Instantiate the enemy
                //Enemy e = Instantiate (enemyPrefab, p, Quaternion.identity) as Enemy;
                Enemy e = Instantiate(enemyPrefab, finalPosition, Quaternion.identity) as Enemy;

                // Set id
                e.id = id;

				// Set name
				e.name = name;
				e.enemyName = name;

				// Set start position
				e.start = p;

				// Enemy Type
				int enemyType;
				Int32.TryParse (rowList [i].type, out enemyType);

				// Set the enemy type
				switch(enemyType)
				{
				case 0:
					e.enemyType = Common.EnemyType.CABINET;
					break;
				case 1:
					e.enemyType = Common.EnemyType.CONGRESS;
					break;
				case 2:
					e.enemyType = Common.EnemyType.PRESS;
					break;
				}

				// Movement type
				int movementType;
				Int32.TryParse (rowList [i].movement_type, out movementType);

				// Set the movement type
				switch(movementType)
				{
				case 0:
					e.movementType = Common.MovementType.LOITER;
					break;
				case 1:
					e.movementType = Common.MovementType.PATH;
					break;
				case 2:
					e.movementType = Common.MovementType.PATROL;
					break;
				}

                /*
                 * Set the NPC's goal position (based on MovementDetails)
                 */ 
				// Get the movement details
				e.movementDetails = getMovementDetails (rowList [i].movement_details);
                // If there are movement details, then it will move to a goal
                if (e.movementDetails.Count > 0)
				{
                    // Get random point on navmesh for initial position
                    randomDirection = UnityEngine.Random.insideUnitSphere * walkRadius;
                    NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                    e.goal = hit.position;
                    e.navGoalPersist = hit.position;
                }
                // Else, the NPC will just stand around
				else
				{
					e.goal = e.start;
				}
				e.goalOrig = e.goal;

				/*
				 * Responses
				 */ 
				Char delimiter = ' ';
				for(int j=0;j<rowList[i].responses.Count;j++)
				{
					// Make EnemyResponse
					EnemyResponse er = new EnemyResponse ();
					er.init (j, rowList[i].responses[j]);
					er.i_topic = j;

					// Get and add the topics that the player can obtain from the response
					string topsStr = rowList [i].topics [j];
					String[] tops = topsStr.Split (delimiter);
					foreach(string t in tops)
					{
						int topic;
						Int32.TryParse (t, out topic);
						er.topicsToObtain.Add (topic);
					}

					// Add response to enemy's list of responses
					e.AddResponse (er);
				}

				// Add the enemy to list
				enemyList.Add (e);
			}	// end if id field exists
		}	// end for each element in rowList
	}	// End init()


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
			row.movement_type = grid [i] [5];
			row.movement_details = grid [i] [6];

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