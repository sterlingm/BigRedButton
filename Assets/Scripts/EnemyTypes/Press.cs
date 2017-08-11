using System;

public class Press : EnemyType
{
	public Press ()
	{
		weakTo.Add (Common.TopicType.SMALL_TALK);
		strongTo.Add (Common.TopicType.HOSTILE_TALK);
	}
}


