using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

[Serializable]
public class AllyAction
{
	[SerializeField]
	public int id;
	public string title;

	public Animation anim;

	public int baseDmg;
	public List<int> allyType = new List<int>();
}

