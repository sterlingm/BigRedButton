using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneSetup : MonoBehaviour {

	public Ally allyPrefab;

	// Use this for initialization
	void Awake() 
	{
		SetupPlayer ();
		CreateAllies ();
	}

	/*
	 * Setup Player stuff for the scene
	 * new position, disable movement, etc
	 */ 
	void SetupPlayer()
	{
		// Set new player position
		Player player = GameObject.Find ("Player").GetComponent<Player> ();
		Vector3 playerPos = new Vector3 (25f, 0.5f, 15f);
		player.transform.position = playerPos;

		// Set player action list for player object
		player.actionList = GameObject.Find ("PlayerActionList").GetComponent<PlayerActionList> ();

		// Disable player movement
        player.GetComponent<IsoCharControl> ().enabled = false;
	}

	/*
	 * Create objects for each player ally
	 */ 
	void CreateAllies()
	{
		Player player = GameObject.Find ("Player").GetComponent<Player> ();

		for(int i=0;i<player.allies.Count;i++)
		{
			Vector3 p = i == 0 ? 
				new Vector3 (player.transform.position.x - 10, player.transform.position.y, player.transform.position.z)
				:
				new Vector3 (player.transform.position.x + 10, player.transform.position.y, player.transform.position.z);

			Ally a = Instantiate (allyPrefab, p, Quaternion.identity) as Ally;
			a.allyInfo = player.allies [i];

			a.name = i == 0 ? "Ally 1" : "Ally 2";
			//Enemy e = Instantiate (enemyPrefab, p, Quaternion.identity) as Enemy;
		}
	}
}
