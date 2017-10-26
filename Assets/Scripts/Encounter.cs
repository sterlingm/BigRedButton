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

		// Clear player topic options
		dropDown.ClearOptions ();

		// Populate topic options
		List<string> topicStrs = player.GetTopicStrings ();
		topicStrs.Insert (0, "Make a selection");
		dropDown.AddOptions (topicStrs);

		// Stop enemy from moving
		e.move = false;

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
		List<string> topicStrs = player.GetTopicStrings ();

		// Insert "Make a selection" to prompt the user
		topicStrs.Insert (0, "Make a selection");

		// Set options
		dropDown.ClearOptions ();
		dropDown.AddOptions (topicStrs);
	}

	/*
	 * Add any new topics that the player can obtain from the enemy's latest response
	 */ 
	public void addNewTopics()
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
	}

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

			// If the user selected "Make ally"
			if(choice == player.i_topics.Count)
			{
				// Check if they already have max # of allies
				if(player.allies.Count >= 2)
				{
					// Display some error message
					errorMsg.text = "You already have 2 allies!";
				}
				// Try to make an ally
				else if(tryMakeAlly())
				{
					player.BuildAlly (enemy);
					enemy.hp = 0;
				}
				else
				{
					// Some penalty
					// Maybe remove time from timer? Display a certain message? "The office is onto you!"
				}
			}

			// Else if a topic was selected
			else
			{
				// Apply topic to enemy
				enemy.ApplyTopic (player.GetTopic(choice));

				// Display enemy response
				StartCoroutine (DisplayEnemyResponse ());

				// Check enemy response for new topics
				addNewTopics ();

				// Increment numRounds
				numRounds++;
			}


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
			// Deal with enemy
			enemy.move = true;
			enemy.gameObject.SetActive (false);
			GameObject.Find ("Enemy Text").SetActive (false);
			GameObject.Find ("Scroll View").SetActive (false);

			enemy.textbox.text = "";

			// Enable character control again
			player.GetComponent<IsoCharControl> ().enabled = true;

			// Destroy this Encounter object
			Destroy (gameObject);
		}
	}	// End Update
}
