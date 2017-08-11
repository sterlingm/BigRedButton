using System;

public class Congress : EnemyType
{
	public Congress ()
	{
		weakTo.Add (Common.TopicType.SHOP_TALK);
		strongTo.Add (Common.TopicType.SMALL_TALK);
	}
}

