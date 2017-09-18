using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.iOS;

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
	public List<PlayerAction> actions;

	public Ally()
	{
		i_actions = new List<int> ();
		actions = new List<PlayerAction> ();
	}

	public List<string> GetActionsStrs()
	{
		List<string> result = new List<string> ();

		foreach(PlayerAction pa in actions)
		{
			result.Add (pa.title);
		}

		return result;
	}
}

