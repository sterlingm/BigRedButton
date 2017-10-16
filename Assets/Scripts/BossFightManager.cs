using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BossFightManager : MonoBehaviour {

	public Player player;
	public Boss boss;



	public Dropdown dropDown;

	public bool choiceMade;

	private bool init;

	public Text bossActionText;

	private int i_activeChar;

	void Awake()
	{
		// Set dropdown object
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/BossFightActions").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

		// Set player object
		// Cannot link in inspector because Player comes from previous scene
		player = GameObject.Find ("Player").GetComponent<Player> ();

		init = false;
	}


	private void DropdownValueChanged(int choice)
	{
		Debug.Log ("In DropdownValueChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

	private void setOptions()
	{
		if(i_activeChar - 1 < player.allies.Count)
		{
			Debug.LogWarning (String.Format ("i_activeChar: {0} player.allies.Count: {1}", i_activeChar, player.allies.Count));
		}
		// Get list of actions for the current active character
		List<string> actionStrs = i_activeChar == 0 ? player.GetActionStrings () 
													: player.allies [i_activeChar - 1].GetActionsStrs ();

		// Insert "Make a selection to prompt the user
		actionStrs.Insert (0, "Make a selection");

		// Clear and re-set the options	
		dropDown.ClearOptions ();
		dropDown.AddOptions (actionStrs);
	}
		

	void Update()
	{
		if (!init)
		{
			setOptions ();
			init = true;
		}
		if(choiceMade)
		{
			// Player's turn
			// Get the choice from the Dropdown
			// Subtract 1 because the first index is "Make a selection"
			int choice = dropDown.value-1;

			// Apply the Action to the boss
			boss.ApplyPlayerAction (player.actionList.list [choice]);

			// If the player has selected an action for each character, it is the boss' turn
			if(i_activeChar == player.allies.Count)
			{
				// Make Boss choose an actions
				int bossChoice = UnityEngine.Random.Range (0, boss.actionList.list.Count);
				BossAction b = boss.actionList.list [bossChoice];
				player.ApplyBossAction (b);
				bossActionText.text = b.title;

				// Set active character back to player
				i_activeChar = 0;
			}
			else
			{
				i_activeChar++;
			}

			// Set dropdown options to show any new topics
			setOptions ();

			// Reset dropdown
			dropDown.value = 0;

			// Reset choiceMade
			choiceMade = false;
		}

		// Check if boss is dead
		if (boss.hp <= 0)
		{
			// Deal with enemy
			boss.gameObject.SetActive (false);

			// Destroy this Encounter object
			Destroy (gameObject);
		}
	}
}
