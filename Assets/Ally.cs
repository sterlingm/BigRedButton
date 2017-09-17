using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;

[Serializable]
public class Ally
{
	public int id;
	public string name;

	/*
	 * Hold a list of indices to get correct actions
	 * TODO: Hold a list of PlayerAction objects, and 
	 * set the objects in a Player method called BuildAlly
	 */ 
	public List<int> i_actions;
	public PlayerActionList actionList;

	public Ally()
	{
		i_actions = new List<int> ();
	}

	public List<string> GetActionsStrs()
	{
		List<string> result = new List<string> ();

		foreach(int i in i_actions)
		{
			result.Add (actionList.list [i].title);
		}

		return result;
	}
}

