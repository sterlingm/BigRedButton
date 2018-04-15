using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class GameManager : MonoBehaviour {

	private UnityAction<Enemy> encListener;


	[SerializeField]
	public Player 			player;
	public Encounter 		encounterPrefab;
	public Encounter 		encounterInstance;
	public RectTransform 	scrollView;
	public bool 			encActive;

	/*
	 * Awake
	 */ 
	void Awake () 
	{
		encListener = new UnityAction<Enemy> (OnEncounter);
		EncounterEventManager.StartListening (Common.ENC_EVENT_STR, encListener);
        scrollView.gameObject.SetActive(false);
        //topicList.gameObject.SetActive(false);
    }

	/*
	 * Update
	 */ 
	void Update () 
	{
		// If no encounter is active, then make the scroll view inactive
		if (!encActive && scrollView.gameObject.activeInHierarchy)
		{
			scrollView.gameObject.SetActive (false);
        }
	}

	/*
	 * Listener for EncounterEvents
	 * This creates a new Encounter object, and turns on and off some objects
	 */ 
	void OnEncounter(Enemy e)
	{
		// Set the enemy text scroll view to active
		scrollView.gameObject.SetActive (true);

		// Create the Encounter and initialize it
		encounterInstance = Instantiate (encounterPrefab) as Encounter;
		encounterInstance.init (player, e);

		// Set encActive to true, and disable the player character movement
		encActive = true;
		player.GetComponent<IsoCharControl>().enabled = false;
	}
}
