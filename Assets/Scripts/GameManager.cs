using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class GameManager : MonoBehaviour {

	private UnityAction<Enemy> encListener;


	[SerializeField]
	public Player player;
	public Encounter encounterPrefab;
	public Encounter encounterInstance;
	public bool encActive;

	// Use this for initialization
	void Awake () 
	{
		encListener = new UnityAction<Enemy> (OnEncounter);
		EncounterEventManager.StartListening (Common.ENC_EVENT_STR, encListener);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (encActive)
		{
			//encounterInstance.go ();
			//encActive = false;
			//Destroy (encounterInstance);
		}
	}

	void onSomeEvent()
	{
		
	}

	void OnEncounter(Enemy e)
	{
		Debug.Log ("In OnEncounter");
		if(!GameObject.Find("Encounter(Clone)"))
		{
			encounterInstance = Instantiate (encounterPrefab) as Encounter;
			encounterInstance.init (player, e);
			encActive = true;
			player.GetComponent<IsoCharControl>().enabled = false;
		}
	}
}
