using UnityEngine;
using System.Collections.Generic;
using System;


[Serializable]
public class BossAction
{
	[SerializeField]
	public int id;
	public string title;

	public Animation anim;

	public int baseDmg;

	public int maxTargets;
	public List<int> i_targets;
}

