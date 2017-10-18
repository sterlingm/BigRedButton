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


	public bool active;
	public bool displayingResponse;
	public Enemy enemy;
	public Player player;
	private bool choiceMade;

	public Text errorMsg;


	// Use this for initialization
	void Awake () 
	{
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/TopicList").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

		errorMsg = GameObject.Find ("/GUI/ErrorMsgs").GetComponent<Text> ();
	}

	void ApplyPlayerAction()
	{
		
	}


	private void DropdownValueChanged(int choice)
	{
		Debug.Log ("In DropdownValueChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

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

		// Set enemy text box
		//enemy.textbox.transform.position = enemy.gameObject.transform.position;
	}

	private void setOptions()
	{
		List<string> topicStrs = player.GetTopicStrings ();
		topicStrs.Insert (0, "Make a selection");
		dropDown.ClearOptions ();
		dropDown.AddOptions (topicStrs);
	}

	public void checkNewTopics()
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

	private bool tryMakeAlly()
	{
		int threshold = (int)Math.Floor(enemy.hp);

		int num = UnityEngine.Random.Range (0, 10);

		return num >= threshold;
	}

	public IEnumerator DisplayEnemyResponse()
	{
		displayingResponse = true;
		enemy.textbox.text = "";
		dropDown.interactable = false;
		foreach(char letter in enemy.lastResponse.response.ToCharArray())
		{
			enemy.textbox.text += letter;

			yield return new WaitForSeconds (0.05f);
		}
		dropDown.interactable = true;
		displayingResponse = false;
	}

	public void Update()
	{
		if(choiceMade)
		{
			// Player's turn
			// Get the choice from the Dropdown
			// Subtract 1 because the first index is "Make a selection"
			int choice = dropDown.value-1;
			//Debug.Log ("choice: " + player.topics[choice].title);

			// If the user selected "Make ally"
			if(choice == player.i_topics.Count)
			{
				if(player.allies.Count >= 2)
				{
					// Display some error message
					errorMsg.text = "You already have 2 allies!";
				}
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

			else
			{
				// Apply topic to enemy
				enemy.ApplyTopic (player.GetTopic(choice));

				// Display enemy response
				StartCoroutine (DisplayEnemyResponse ());

				// Check enemy response for new topics
				checkNewTopics ();
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

			// Enable character control again
			player.GetComponent<IsoCharControl> ().enabled = true;

			// Destroy this Encounter object
			Destroy (gameObject);
		}
	}


	public void go()
	{
		Debug.Log ("Encounter started with "+enemy.enemyName);

		Debug.Log (String.Format ("Enemy: {0}", enemy.initEncounter));

		choiceMade = false;
	}
}
