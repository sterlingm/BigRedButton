using UnityEngine;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
	public int hp;
	public List<BossAction> actions;

	private POTUSActionList actionList;
}

