using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player self;
    
    // This is set in EncounterManager
    public bool inEncounter;

    //[SerializeField]
    // Here to compile Boss scene, fix this soon
    public PlayerActionList actionList;

	// Boss fight stats
	public int hp;

	public List<AllyInfo> allies;

	[SerializeField]
	public AllyActionList allyActionList;

	// 2D list to hold the indices of the possible actions for each enemy type
	// [type][action]
	[SerializeField]
	public List< List<int> > i_allyActionsForType;


	// Use this for initialization
	void Awake ()
	{
        if (self == null)
        {
            self = this;
        }
        
        /*
		 *  Initialize stuff
		 */
        // If Game scene is loaded, grab topicList object
        if (SceneManager.GetActiveScene().buildIndex == 0)
		{

			// Set ally action list
			allyActionList = GameObject.Find ("AllyActionList").GetComponent<AllyActionList> ();

			// Initialize the ally actions list
			i_allyActionsForType = new List<List<int>> ();

			// Find a better way to do this...
			List<int> zeros = new List<int> ();
			List<int> ones = new List<int> ();
			List<int> twos = new List<int> ();
			i_allyActionsForType.Add (zeros);
			i_allyActionsForType.Add (ones);
			i_allyActionsForType.Add (twos);

			/*
			 * Go through the ally action list and initialize the 2d list of indices
			 * for each ally type
			 */ 
			// For each ally action
			for (int i = 0; i < allyActionList.list.Count; i++)
			{
				// For each type for that ally action
				// BUILD A LIST, SHOULD NOT TRY TO ADD ON TO CURRENT LIST
				for (int j = 0; j < allyActionList.list [i].allyType.Count; j++)
				{
					int i_type = allyActionList.list [i].allyType [j];
					int actionId = allyActionList.list [i].id;

					i_allyActionsForType [i_type].Add (actionId);
				}
			}
		}   // end if scene index == 0

		hp = 10;
	}


	public void BuildAlly(Enemy e)
	{
		AllyInfo a = new AllyInfo ();

		// Set name and id
		a.name = e.enemyName;

		// Get type
		Common.EnemyType type = e.enemyType;

		List<int> actions = i_allyActionsForType [(int)type];
		foreach(int i in actions)
		{
			// ***************************************************************
			// **** Add a random check to see if the ally gets the action ****
			// ***************************************************************

			// Add the action
			a.actions.Add (allyActionList.list[i]);
		}

		allies.Add (a);
	}


	// Update is called once per frame
	void Update ()
	{
	
	}

	public void ApplyBossAction(BossAction b)
	{
		hp -= b.baseDmg;
	}

    public void ApplyEnemyAction(EnemyAction a)
    {
        hp -= a.baseDmg;
    }
    

    public PlayerAction GetAction(int i)
    {
        return PlayerActionList.self.list[i];
    }
    

	public List<string> GetActionStrings()
	{
		List<string> result = new List<string> ();
		for(int i=0;i< PlayerActionList.self.list.Count;i++)
		{
			result.Add (PlayerActionList.self.list[i].title);
		}
		return result;
	}
}

