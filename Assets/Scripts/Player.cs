using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using System.Security.AccessControl;
using System.Deployment.Internal;

public class Player : MonoBehaviour
{

	[SerializeField]
	// List of topics for the main game loop
	private TopicList topicList;
	public List<int> i_topics;

	[SerializeField]
	// Boss fight actions
	public PlayerActionList actionList;

	// Boss fight stats
	public int hp;

	public List<Ally> allies;

	[SerializeField]
	public AllyActionList allyActionList;

	// 2D list to hold the indices of the possible actions for each enemy type
	// [type][action]
	[SerializeField]
	public List< List<int> > i_allyActionsForType;


	// Use this for initialization
	void Awake ()
	{
		/*
		 *  Initialize stuff
		 */ 
		// If Game scene is loaded, grab topicList object
		if(SceneManager.GetActiveScene().buildIndex == 0)
		{
			// Give the player a few topics to bring up initially
			topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();
			i_topics = new List<int> ();
			i_topics.Add (0);
			i_topics.Add (1);
			i_topics.Add (2);

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
		}

		hp = 10;
	}


	public void BuildAlly(Enemy e)
	{
		Ally a = new Ally ();

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

	public Topic GetTopic(int i)
	{
		return topicList.list [i_topics [i]];
	}

	public List<string> GetTopicStrings()
	{
		List<string> result = new List<string>();

		for(int i=0;i<i_topics.Count;i++)
		{
			result.Add (topicList.list[i_topics [i]].title);
		}

		// Insert "Make Ally"
		result.Add ("Make ally");
		return result;
	}


	public List<string> GetActionStrings()
	{
		List<string> result = new List<string> ();
		for(int i=0;i<actionList.list.Count;i++)
		{
			result.Add (actionList.list[i].title);
		}
		return result;
	}
}

