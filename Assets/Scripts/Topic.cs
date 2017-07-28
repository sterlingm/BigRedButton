using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

[System.Serializable]
public class Topic 
{
	// Topic id
	private int id;

	// String for displaying purposes
	public string title;

	// Type
	public Common.TopicType type;

	// Base damage (i.e. how much people want to talk about this)
	// Lower means less hp done meaning people really want to talk about it
	public float baseDmg;

	public void init(int id, string title, Common.TopicType type, double baseDmg)
	{
		this.id 		= id;
		this.title 		= title;
		this.type 		= type;
		this.baseDmg 	= (float)baseDmg;
	}



}

