using UnityEngine;
using System.Collections;

public class Cabinet : EnemyType
{
	public Cabinet()
	{
		weakTo.Add (Common.TopicType.HOSTILE_TALK);
		strongTo.Add (Common.TopicType.SHOP_TALK);
	}
}

