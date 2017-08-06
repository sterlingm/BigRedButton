using System;
using System.Collections.Generic;

[System.Serializable]
public class EnemyResponse
{
	// string that is the response
	public string response;

	// Index of topic to respond to
	public int i_topic;

	// All Topics that the user can gain from this response 
	// This must be initialized from the TopicList
	public List<int> topicsToObtain;

	public void init(int i, string res)
	{
		i_topic = i;
		response = res;
	}
}


