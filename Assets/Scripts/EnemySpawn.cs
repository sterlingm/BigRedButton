using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class EnemySpawn : MonoBehaviour
{
	[SerializeField]public List<Room> rooms;
	public Enemy enemyPrefab;


	// Use this for initialization
	void Start ()
	{
		Vector3 pos = rooms [0].tiles [0].gameObject.transform.position;
		pos.y = 0.75f;
		Enemy enemyInstance = Instantiate (enemyPrefab, pos, Quaternion.identity) as Enemy;
		enemyInstance.move = false;
	}

}

