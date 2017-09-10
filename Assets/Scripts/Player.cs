using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework;

public class Player : MonoBehaviour
{

	private TopicList topicList;
	private PlayerActionList actionList;
	public List<int> i_topics;

	// Boss fight actions
	[SerializeField]
	public List<PlayerAction> actions;

	// Use this for initialization
	void Start ()
	{
		/*
		 *  Initialize stuff
		 */ 
		// Make some fake actions for now
		PlayerAction a = new PlayerAction ();
		a.title = "Action a";
		PlayerAction b = new PlayerAction ();
		b.title = "Action b";
		PlayerAction c = new PlayerAction ();
		c.title = "Action c";

		actions = new List<PlayerAction> ();
		actions.Add (a);
		actions.Add (b);
		actions.Add (c);


		// Give the player a few topics to bring up initially
		topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();
		i_topics = new List<int> ();
		i_topics.Add (0);
		i_topics.Add (1);
		i_topics.Add (2);

	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
		return result;
	}

	public List<string> GetActionStrings()
	{
		List<string> result = new List<string> ();
		for(int i=0;i<actions.Count;i++)
		{
			result.Add (actions [i].title);
		}
		return result;
	}
}

