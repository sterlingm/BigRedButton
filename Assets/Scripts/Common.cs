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
	public static List<TopicType> topicTypeList = new List<TopicType> { TopicType.SMALL_TALK, TopicType.SHOP_TALK, TopicType.HOSTILE_TALK };

	public static string ENC_EVENT_STR = "Encounter";


	public enum EnemyType
	{
		CABINET,
		CONGRESS,
		PRESS
	}

	private static List<TopicType> cabinetWeakTo = new List<TopicType> { TopicType.HOSTILE_TALK };
	private static List<TopicType> congressWeakTo = new List<TopicType> { TopicType.SHOP_TALK };
	private static List<TopicType> pressWeakTo = new List<TopicType> { TopicType.SMALL_TALK };
	public static List< List<TopicType> > weakTo = new List< List<TopicType> > { cabinetWeakTo, congressWeakTo, pressWeakTo };

	private static List<TopicType> cabinetStrongTo = new List<TopicType> { TopicType.SHOP_TALK };
	private static List<TopicType> congressStrongTo = new List<TopicType> { TopicType.SMALL_TALK };
	private static List<TopicType> pressStrongTo = new List<TopicType> { TopicType.HOSTILE_TALK };
	public static List< List<TopicType> > strongTo = new List< List<TopicType> > { cabinetStrongTo, congressStrongTo, pressStrongTo };

	public static EnemyResponse enemyDefaultResp = new EnemyResponse ();
}