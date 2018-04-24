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


    public enum EnemyActionType
    {
        NORMAL,
        DESPERATE,
        ACID
    }
    public static List<EnemyActionType> enemyActionTypeList = new List<EnemyActionType> { EnemyActionType.NORMAL, EnemyActionType.DESPERATE, EnemyActionType.ACID };

    public static string ENC_EVENT_STR = "Encounter";

	public enum MovementType
	{
		LOITER,
		PATH,
		PATROL
	}

	public enum EnemyType
	{
		CABINET,
		CONGRESS,
		PRESS
    }


    /*
     * strongTo and weakTo are setup as global 2D arrays
     * Each element is an array for an enemy type
     * strongTo: { cabinetStrongTo: {shop talk}, congressStrongTo: {small talk} ... }
     * weakTo: { cabinetWeakTo: {hostile talk}, congressWeakTo: {shop talk} ... }
     * In other parts of the code, the values can be accessed in this way:
     *      Check if an enemy is weak to a given topic:
     *          if(Common.weakTo[ (int)enemy.enemyType ] [ (int)topic.type ])
     */
    private static List<TopicType> cabinetWeakTo    = new List<TopicType> { TopicType.HOSTILE_TALK };
	private static List<TopicType> congressWeakTo   = new List<TopicType> { TopicType.SHOP_TALK };
	private static List<TopicType> pressWeakTo      = new List<TopicType> { TopicType.SMALL_TALK };
    public static List< List<TopicType> > weakTo    = new List< List<TopicType> > { cabinetWeakTo, congressWeakTo, pressWeakTo };

	private static List<TopicType> cabinetStrongTo  = new List<TopicType> { TopicType.SHOP_TALK };
	private static List<TopicType> congressStrongTo = new List<TopicType> { TopicType.SMALL_TALK };
	private static List<TopicType> pressStrongTo    = new List<TopicType> { TopicType.HOSTILE_TALK };
	public static List< List<TopicType> > strongTo  = new List< List<TopicType> > { cabinetStrongTo, congressStrongTo, pressStrongTo };

	public static EnemyResponse enemyDefaultResp = new EnemyResponse ();
}