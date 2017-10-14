using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.iOS;

[Serializable]
public class Ally
{
	public int id;	// Should equal the Enemy id that was used to create the Ally
	public string name;

	/*
	 * Hold a list of indices to get correct actions
	 * TODO: Hold a list of PlayerAction objects, and 
	 * set the objects in a Player method called BuildAlly
	 */ 
	public List<int> i_actions;
	public List<AllyAction> actions;

	public Ally()
	{
		i_actions = new List<int> ();
		actions = new List<AllyAction> ();
	}

	public List<string> GetActionsStrs()
	{
		List<string> result = new List<string> ();

		foreach(AllyAction aa in actions)
		{
			result.Add (aa.title);
		}

		return result;
	}
}

