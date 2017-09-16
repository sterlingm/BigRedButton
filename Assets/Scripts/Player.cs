using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework;

public class Player : MonoBehaviour
{

	// List of topics for the main game loop
	private TopicList topicList;
	public List<int> i_topics;

	[SerializeField]
	// Boss fight actions
	public PlayerActionList actionList;

	// Boss fight stats
	public int hp;


	// Use this for initialization
	void Awake ()
	{
		/*
		 *  Initialize stuff
		 */ 

		// Give the player a few topics to bring up initially
		topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();
		i_topics = new List<int> ();
		i_topics.Add (0);
		i_topics.Add (1);
		i_topics.Add (2);

		hp = 10;
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

