using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FightManager : MonoBehaviour 
{

	public Player player;
	public Boss boss;
	public List<Ally> allies;
	public Dropdown dropDown;

	public bool choiceMade;

	private bool init;

	public Text bossActionText;

	private int i_activeChar;

	public Text playerTurnText;
	public List<Text> allyTurnTexts;
	public List<Text> allyHpTexts;
	public Text allyOneTurnText;
	public Text allyTwoTurnText;
	public Text bossTurnText;
	public Text bossHp;
	public Text playerHp;
	public Text allyOneHp;
	public Text allyTwoHp;

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

		// Set List<Ally> elements
		for(int i=0;i<player.allies.Count;i++)
		{
			string n = String.Format ("Ally {0}", i+1);
			Ally temp = GameObject.Find (n).GetComponent<Ally> ();
			allies.Add (temp);
		}


		// Set text field positions for Hp, turn indicators, etc
		// Do this programatically because we don't know how many allies the player will have
		SetTextFieldPositions ();

		// Set the turn indicator
		SetTurnIndicator ();

		// End initialization
		init = false;
	}


	private void SetTextFieldPositions()
	{
		// Set offsets
		int x_offsetTurn = 40;
		int y_offsetTurn = -55;
		int x_offsetHp = 60;
		int y_offsetHp = -35;

		/*
		 * Player
		 */ 
		// Player turn indicator
		Vector3 playerPos = camera.WorldToScreenPoint (player.transform.position);
		playerPos.x += x_offsetTurn;
		playerPos.y += y_offsetTurn;
		playerTurnText.transform.position = playerPos;

		// Reset playerPos and apply HP text offset
		playerPos = camera.WorldToScreenPoint (player.transform.position);
		playerPos.x += x_offsetHp;
		playerPos.y += y_offsetHp;
		playerHp.transform.position = playerPos;

		/*
		 * Boss
		 */ 
		// Set boss turn indicator text position
		Vector3 potusPos = camera.WorldToScreenPoint (boss.transform.position);
		potusPos.x += x_offsetTurn;
		potusPos.y += y_offsetTurn;
		bossTurnText.transform.position = potusPos;

		/*
		 * Allies
		 */ 
		// Can't put these into Lists because then we can't set the values in the Inspector
		// So set them both manually instead of in a loop
		if(player.allies.Count > 0)
		{
			Ally temp = GameObject.Find ("Ally 1").GetComponent<Ally> ();
			Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
			screenPos.x += x_offsetTurn;
			screenPos.y += y_offsetTurn;
			allyOneTurnText.transform.position = screenPos;

			screenPos = camera.WorldToScreenPoint (temp.transform.position);
			screenPos.x += x_offsetHp;
			screenPos.y += y_offsetHp;
			allyOneHp.transform.position = screenPos;
		}
        else
        {
            Debug.Log("In Else");
            Text temp = GameObject.Find("AllyOneHP").GetComponent<Text>();
            Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
            screenPos.x += 1000f;
            screenPos.y += 1000f;
            screenPos.z += 1000f;
            allyOneHp.transform.position = screenPos;

            temp = GameObject.Find("AllyTwoHP").GetComponent<Text>();
            screenPos = camera.WorldToScreenPoint(temp.transform.position);
            screenPos.x += 1000f;
            screenPos.y += 1000f;
            screenPos.z += 1000f;
            allyTwoHp.transform.position = screenPos;
        }
		if(player.allies.Count > 1)
		{
			Ally temp = GameObject.Find ("Ally 2").GetComponent<Ally> ();
			Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
			screenPos.x += x_offsetTurn;
			screenPos.y += y_offsetTurn;
			allyTwoTurnText.transform.position = screenPos;

			screenPos = camera.WorldToScreenPoint (temp.transform.position);
			screenPos.x += x_offsetHp;
			screenPos.y += y_offsetHp;
			allyTwoHp.transform.position = screenPos;
		}
	}

	private void DropdownValueChanged(int choice)
	{
		Debug.Log ("In DropdownValueChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

	private void SetOptions()
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

	void UpdateHpText()
	{
		playerHp.text = String.Format ("HP: {0}", player.hp);

		if(allies.Count > 0)
		{
			allyOneHp.text = String.Format ("HP: {0}", allies [0].hp);
		}
		if(allies.Count > 1)
		{
			allyTwoHp.text = String.Format ("HP: {0}", allies [1].hp);
		}
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

	void ApplyBossAction(BossAction b)
	{
		player.ApplyBossAction (b);
		foreach(Ally a in allies)
		{
			a.ApplyBossAction (b);
		}
	}

	void CheckGameOver()
	{
		if(boss.hp <= 0)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene ("GameWon");
		}

		if(player.hp <= 0)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene ("GameLost");
		}
	}

	void Update()
	{
		if (!init)
		{
			SetOptions ();
			init = true;
		}
		if(choiceMade)
		{
			// Player's turn
			// Get the choice from the Dropdown
			// Subtract 1 because the first index is "Make a selection"
			int choice = dropDown.value-1;

			// Apply the Action to the boss
			boss.ApplyPlayerAction (PlayerActionList.self.list [choice]);

			// If the player has selected an action for each character, it is the boss' turn
			if(i_activeChar == player.allies.Count)
			{				
				// Make Boss choose an actions
				int bossChoice = UnityEngine.Random.Range (0, boss.actionList.list.Count);
				BossAction b = boss.actionList.list [bossChoice];

				// Apply the action to the player and allies
				ApplyBossAction (b);

				// Set text string to show the action
				bossActionText.text = String.Format("POTUS used: {0}", b.title);

				// Update the HP texts
				UpdateHpText ();

				// Check if game is over
				CheckGameOver ();

				// Set active character back to player
				i_activeChar = 0;
			}
			else
			{
				i_activeChar++;
			}

			// Set dropdown options to show any new topics
			SetOptions ();

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
	}	// End Update
}
