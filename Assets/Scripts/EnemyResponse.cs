using System;
using System.Collections.Generic;
using NUnit.Framework;

[System.Serializable]
public class EnemyResponse
{
	// string that is the response
	public string response;

	// Index of topic to respond to
	public int i_topic;

	// All Topics that the user can gain from this response 
	// This must be initialized from the TopicList
	public List<int> topicsToObtain = new List<int> ();

	public EnemyResponse()
	{
		response = "I don't know anything about that.";
	}

	public void init(int i, string res)
	{
		i_topic = i;
		response = res;
	}
}


