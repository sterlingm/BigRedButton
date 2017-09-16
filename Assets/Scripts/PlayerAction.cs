using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerAction
{
	[SerializeField]
	public int id;
	public string title;

	public Animation anim;

	public int damage;
	public int actionType;
	public int allyType;

}

