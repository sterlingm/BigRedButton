using NUnit.Framework;
using System.Collections.Generic;

public class Common
{

	public enum TopicType
	{
		SMALL_TALK,
		SHOP_TALK,
		HOSTILE_TALK
	}
	public static List<TopicType> typeList = new List<TopicType> { TopicType.SMALL_TALK, TopicType.SHOP_TALK, TopicType.HOSTILE_TALK };

	public static string ENC_EVENT_STR = "Encounter";

}

