using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.iOS;

[Serializable]
public class Ally : MonoBehaviour
{
	public AllyInfo allyInfo;
	public int hp;

	void Awake()
	{
		hp = 10;
	}

	public void ApplyBossAction(BossAction b)
	{
		hp -= b.baseDmg;

		if(hp <= 0)
		{
			Destroy (gameObject);
		}
	}
}

