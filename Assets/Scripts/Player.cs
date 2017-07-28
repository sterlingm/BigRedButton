using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NUnit.Framework;

public class Player : MonoBehaviour
{

	private TopicList topicList;
	public List<int> topics;

	// Use this for initialization
	void Start ()
	{
		topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();
		topics = new List<int> ();
		topics.Add (0);
		topics.Add (1);
		topics.Add (2);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public Topic GetTopic(int i)
	{
		return topicList.list [topics [i]];
	}

	public List<string> GetTopicStrings()
	{
		List<string> result = new List<string>();

		for(int i=0;i<topics.Count;i++)
		{
			result.Add (topicList.list[topics [i]].title);
		}
		return result;
	}
}

