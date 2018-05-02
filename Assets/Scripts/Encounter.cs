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


    // Text objects to display hp values
    [SerializeField]
    public Dropdown playerActionDropdown;
    public ScrollRect encSummaryBox;
    public Text displayEncSummary;





    /*
	 * Awake
	 */
    void Awake () 
	{
		choiceMade = false;
		playerActionDropdown = GameObject.Find ("/GUI/Player Action List").GetComponent<Dropdown> ();
		playerActionDropdown.onValueChanged.AddListener(DropdownValueChanged);

		errorMsg = GameObject.Find ("/GUI/ErrorMsgs").GetComponent<Text> ();


        // Get game objects used for displaying information to the user
        encSummaryBox       = GameObject.Find("Scroll View").GetComponent<ScrollRect>();
        displayEncSummary   = GameObject.Find("Encounter summary").GetComponent<Text>();

        // Make them visible
        displayEncSummary.gameObject.SetActive(true);
        encSummaryBox.gameObject.transform.localPosition    = new Vector3(325f, -125f, 0f);
        playerActionDropdown.gameObject.transform.localPosition = new Vector3(250f, 8.5f, 0f);

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
	public void Init(Player p, Enemy e)
	{
		// Set references
		player = p;
		enemy = e;

        Player.self.inEncounter = true;

		// Clear player topic options
		playerActionDropdown.ClearOptions ();

		// Populate topic options
		List<string> actionStrs = player.GetActionStrings ();
		actionStrs.Insert (0, "Make a selection");
		playerActionDropdown.AddOptions (actionStrs);

		// Stop enemy from moving
		//e.move = false;
        e.StopMoving();

		// Set numRounds
		numRounds = 0;

        // Set initial summary of encounter
        SetInitialSummary();
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
	private bool TryMakeAlly()
	{
		int threshold = (int)Math.Floor(enemy.hp);

		int num = UnityEngine.Random.Range (0, 10);

		return numRounds >= 2 && num <= threshold;
	}

    /*
	 * Display the enemy's response on screen
	 * It displays each character one by one with a slight delay
	  
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
	}*/


    private void SetInitialSummary()
    {
        String summary = String.Format("You have encountered {0}!\nSelect actions until you defeat them!", enemy.name);
        displayEncSummary.text = summary;
    }

    private void SetSummary(PlayerAction pa, EnemyAction ea)
    {
        String summary = String.Format("Enemy: {0}\nHP: {1}\n{0} used {2}\n\nPlayer HP: {3}\nYour last attack: {4}\n", enemy.name, enemy.hp, ea.title, player.hp, pa.title);
        displayEncSummary.text = summary;
    }

    /*
	 * Set the options for the dropdown
	 * The options are the topics that a player can choose
	 */
    private void SetOptions()
    {
        // Get topic strings
        List<string> actionStrs = player.GetActionStrings();

        // Insert "Make a selection" to prompt the user
        actionStrs.Insert(0, "Make a selection");

        // Set options
        playerActionDropdown.ClearOptions();
        playerActionDropdown.AddOptions(actionStrs);
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
			int choice = playerActionDropdown.value-1;
			
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
            //enemy.textbox.text = b.title;

            // Increment numRounds
            numRounds++;
			
			// Set dropdown options to show any new topics
			SetOptions ();

            // Display Encounter updates to the user
            SetSummary(player.GetAction(choice), b);

			// Reset dropdown
			playerActionDropdown.value = 0;

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
			//GameObject.Find ("Scroll View").SetActive (false);


			// Enable character control again
			player.GetComponent<IsoCharControl> ().enabled = true;

            // Set inEncounter
            Player.self.inEncounter = false;

            // Make encounter box and ui stuff invisible
            displayEncSummary.text = "";
            encSummaryBox.gameObject.transform.localPosition = new Vector3(1000f, 1000f, 1000f);
            playerActionDropdown.gameObject.transform.localPosition = new Vector3(1000f, 1000f, 1000f);

            // Destroy this Encounter object
            Destroy (gameObject);
		}
	}	// End Update
}
