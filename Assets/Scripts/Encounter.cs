using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using UnityEngine.WSA;
using UDB;
using System.Xml.Schema;


public class Encounter : MonoBehaviour {

	[SerializeField]
	public Dropdown dropDown;


	// Player and enemy involved in the encounter
	public Enemy 	enemy;
	public Player 	player;

	// Flag for if the enemy response is still being displayed
	public bool 	displayingResponse;

	// Flag for when the player has selected a topic
	private bool choiceMade;

	// Display any error messages
	public Text errorMsg;

	// Number of times the player has selected a topic
	private int numRounds;

	/*
	 * Awake
	 */ 
	void Awake () 
	{
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/TopicList").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

		errorMsg = GameObject.Find ("/GUI/ErrorMsgs").GetComponent<Text> ();

		numRounds = 0;
	}

	/*
	 * Change choiceMade to true
	 * Take an arg to satisfy listener signature
	 */ 
	private void DropdownValueChanged(int choice)
	{
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

	/*
	 * Initialize the Encounter object with a player and enemy
	 */ 
	public void init(Player p, Enemy e)
	{
		// Set references
		player = p;
		enemy = e;

        Player.self.inEncounter = true;

		// Clear player topic options
		dropDown.ClearOptions ();

		// Populate topic options
		List<string> actionStrs = player.GetActionStrings ();
		actionStrs.Insert (0, "Make a selection");
		dropDown.AddOptions (actionStrs);

		// Stop enemy from moving
		//e.move = false;
        e.StopMoving();

		// Set numRounds
		numRounds = 0;
	}

	/*
	 * Set the options for the dropdown
	 * The options are the topics that a player can choose
	 */ 
	private void setOptions()
	{
		// Get topic strings
		List<string> actionStrs = player.GetActionStrings ();

		// Insert "Make a selection" to prompt the user
		actionStrs.Insert (0, "Make a selection");

		// Set options
		dropDown.ClearOptions ();
		dropDown.AddOptions (actionStrs);
	}

	/*
	 * Add any new topics that the player can obtain from the enemy's latest response
	 */ 
	/*public void addNewTopics()
	{
		foreach(int i_topics in enemy.lastResponse.topicsToObtain)
		{
			Debug.Log ("i_topics: " + i_topics);
			if(player.i_topics.Contains(i_topics))
			{
				Debug.Log ("Player already has topic i");
			}
			else
			{
				Debug.Log ("Player does not have topic "+i_topics);
				player.i_topics.Add (i_topics);
			}
		}
	}*/

	/*
	 * Attempt to make the enemy an Ally
	 * Return true if successful
	 */ 
	private bool tryMakeAlly()
	{
		int threshold = (int)Math.Floor(enemy.hp);

		int num = UnityEngine.Random.Range (0, 10);

		return numRounds >= 2 && num <= threshold;
	}

	/*
	 * Display the enemy's response on screen
	 * It displays each character one by one with a slight delay
	 */ 
	public IEnumerator DisplayEnemyResponse()
	{
		// Set flag 
		displayingResponse = true;

		// Reset text and make dropdop not interactable while displaying response
		enemy.textbox.text = "";
		dropDown.interactable = false;

		// Display the characters
		foreach(char letter in enemy.lastResponse.response.ToCharArray())
		{
			enemy.textbox.text += letter;

			yield return new WaitForSeconds (0.05f);
		}

		// Turn stuff back on
		dropDown.interactable = true;
		displayingResponse = false;
	}
    

    /*
	 * Update
	 */
    public void Update()
	{
		if(choiceMade)
		{
			// Player's turn
			// Get the choice from the Dropdown
			// Subtract 1 because the first index is "Make a selection"
			int choice = dropDown.value-1;
			
			// Apply topic to enemy
			enemy.ApplyAction (player.GetAction(choice));

            // Display enemy response
            //StartCoroutine (DisplayEnemyResponse ());

            // Make Boss choose an actions
            int enemyChoice = UnityEngine.Random.Range(0, enemy.actions.Count);
            EnemyAction b = enemy.actions[enemyChoice];

            player.ApplyEnemyAction(b);
            /*foreach (Ally a in allies)
            {
                a.ApplyBossAction(b);
            }*/

            // Set text string to show the action
            enemy.textbox.text = b.title;

            // Increment numRounds
            numRounds++;
			
			// Set dropdown options to show any new topics
			setOptions ();

			// Reset dropdown
			dropDown.value = 0;

			// Reset choiceMade
			choiceMade = false;
		}
			
		// Check if enemy is dead
		if (enemy.hp <= 0 && !displayingResponse)
		{
			/* 
             * Deal with enemy
             * 
             */
			//enemy.move = true;
            enemy.StartMoving();
            enemy.gameObject.SetActive (false);
			GameObject.Find ("Enemy Text").SetActive (false);
			GameObject.Find ("Scroll View").SetActive (false);

            enemy.textbox.text = "";

			// Enable character control again
			player.GetComponent<IsoCharControl> ().enabled = true;

            // Set inEncounter
            Player.self.inEncounter = false;

            // Destroy this Encounter object
            Destroy (gameObject);
		}
	}	// End Update
}
