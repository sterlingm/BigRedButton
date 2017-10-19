using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;

public class BossFightManager : MonoBehaviour {

	public Player player;
	public Boss boss;
	public Dropdown dropDown;

	public bool choiceMade;

	private bool init;

	public Text bossActionText;

	private int i_activeChar;

	public Text playerTurnText;
	public Text allyOneTurnText;
	public Text allyTwoTurnText;
	public Text bossTurnText;
	public Text bossHp;

	public Camera camera;

	void Awake()
	{
		// Set dropdown object
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/BossFightActions").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

		// Set player object
		// Cannot link in inspector because Player comes from previous scene
		player = GameObject.Find ("Player").GetComponent<Player> ();

		// Set player turn indicator text position
		Vector3 playerPos = camera.WorldToScreenPoint (player.transform.position);
		playerPos.x += 50;
		playerPos.y -= 50;
		playerTurnText.transform.position = playerPos;

		// Set boss turn indicator text position
		Vector3 potusPos = camera.WorldToScreenPoint (boss.transform.position);
		potusPos.x += 30;
		potusPos.y += 50;
		bossTurnText.transform.position = potusPos;

		// Set ally turn indicator text position
		if(player.allies.Count > 0)
		{
			Ally temp = GameObject.Find ("Ally 1").GetComponent<Ally> ();
			Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
			screenPos.x += 50;
			screenPos.y -= 50;
			allyOneTurnText.transform.position = screenPos;
		}
		if(player.allies.Count > 1)
		{
			Ally temp = GameObject.Find ("Ally 2").GetComponent<Ally> ();
			Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
			screenPos.x += 50;
			screenPos.y -= 50;
			allyTwoTurnText.transform.position = screenPos;
		}

		// Set the turn indicator
		SetTurnIndicator ();

		// End initialization
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
		
	void SetTurnIndicator()
	{
		playerTurnText.gameObject.SetActive (false);
		bossTurnText.gameObject.SetActive (false);
		allyOneTurnText.gameObject.SetActive (false);
		allyTwoTurnText.gameObject.SetActive (false);
		if(i_activeChar == 0)
		{
			playerTurnText.gameObject.SetActive (true);
		}
		else if(i_activeChar == 1)
		{
			allyOneTurnText.gameObject.SetActive (true);
		}
		else if(i_activeChar == 2)
		{
			allyTwoTurnText.gameObject.SetActive (true);
		}
		else
		{
			bossTurnText.gameObject.SetActive (true);
		}
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

			// Set new turn indicator
			SetTurnIndicator ();

			// Set new boss HP
			bossHp.text = String.Format ("HP: {0}", boss.hp);

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
