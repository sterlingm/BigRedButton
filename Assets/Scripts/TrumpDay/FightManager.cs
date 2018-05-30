﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FightManager : MonoBehaviour
{
    // Singleton
    public static FightManager self = null;

    public ScheduleItem currentItem;

    public PlayerTD player;
	public List<Ally> allies;
	public Dropdown actionsDropDown;
    public Dropdown targetDropDown;
    public EnemyTD enemy;

    public EnemyTD enemyPrefab;
    public List<EnemyTD> enemies;
    public List<Text> enemiesHP;
    public List<Text> enemiesTurn;

	public bool choiceMade;
    public bool targetSelected;

    public bool fightOver;
    private bool init;
    private int i_activeChar;


	public Text playerTurnText;
	public Text playerHp;
    public Text actionTitleText;
    

	void Awake()
	{
        if(self == null)
        {
            self = this;
        }

        // Set dropdown listeners and booleans		
		actionsDropDown.onValueChanged.AddListener(ActionsDropdownChanged);
        targetDropDown.onValueChanged.AddListener(TargetDropdownChanged);
        choiceMade      = false;
        targetSelected  = false;

        // Set player object
        // Cannot link in inspector because Player comes from previous scene
        player = GameObject.Find ("Player").GetComponent<PlayerTD> ();

		// Set List<Ally> elements
		for(int i=0;i<player.allies.Count;i++)
		{
			string n = String.Format ("Ally {0}", i+1);
			Ally temp = GameObject.Find (n).GetComponent<Ally> ();
			allies.Add (temp);
		}
        

		// Set the turn indicator
		SetTurnIndicator ();

        // Set index of active character
        i_activeChar = 0;
        
		// End initialization
		init = false;
        
        // Set flag
        fightOver = false;
    }
    
    

	private void ActionsDropdownChanged(int choice)
	{
		Debug.Log ("In ActionsDropdownChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

    private void TargetDropdownChanged(int choice)
    {
        Debug.Log("In TargetDropdownChanged");
        if (!targetSelected)
        {
            targetSelected = true;
        }
    }


    private void SetActionsDropdown()
	{
		if(i_activeChar - 1 < player.allies.Count)
		{
			Debug.LogWarning (String.Format ("i_activeChar: {0} player.allies.Count: {1}", i_activeChar, player.allies.Count));
		}
		
        // Get actions for the player
        List<string> actionStrs = player.GetActionStrings();

		// Insert "Make a selection to prompt the user
		actionStrs.Insert (0, "Make a selection");

		// Clear and re-set the options	
		actionsDropDown.ClearOptions ();
		actionsDropDown.AddOptions (actionStrs);
	}

    private void SetTargetsDropdown()
    {
        List<string> strs = new List<string>();
        for(int i=0;i<enemies.Count;i++)
        {
            strs.Add(i.ToString());
        }
        
        // Insert "Make a selection to prompt the user
        strs.Insert(0, "Select target");

        // Clear and re-set the options	
        targetDropDown.ClearOptions();
        targetDropDown.AddOptions(strs);
    }

    /*
     * Update the HP text objects for all characters
     */
    void UpdateHpText()
	{
		playerHp.text = String.Format ("{0}", player.hp);

        for(int i=0;i<enemiesHP.Count;i++)
        {
            enemiesHP[i].text = String.Format("{0}", enemies[i].hp);
        }
    }
		
    /*
     * Maybe use this later on to give a little delay for each enemy to use their move
     */ 
	void SetTurnIndicator()
	{
		playerTurnText.gameObject.SetActive (false);
		//enemyTurnText.gameObject.SetActive (false);

        //allyOneTurnText.gameObject.SetActive (false);
		//allyTwoTurnText.gameObject.SetActive (false);

		if(i_activeChar == 0)
		{
			playerTurnText.gameObject.SetActive (true);
		}
		/*else if(i_activeChar == 1)
		{
			allyOneTurnText.gameObject.SetActive (true);
		}
		else if(i_activeChar == 2)
		{
			allyTwoTurnText.gameObject.SetActive (true);
		}*/
	}
    
    /*
     * Apply an enemy action
     * Reduce hp, change status, etc
     */ 
    void ApplyEnemyAction(EnemyActionTD e)
    {
        player.ApplyEnemyAction(e);
    }

    /*
     * Check for terminal state
     */ 
	void CheckGameOver()
	{
		if(enemies.Count == 0)
		{
			SceneManager.LoadScene ("GameWon");
		}

		if(player.hp <= 0)
		{
			SceneManager.LoadScene ("GameLost");
		}
	}

    /*
     * Removing an enemy requires destroying scene game objects 
     * and removing instances from lists
     */ 
    private void DestroyEnemyAtIndex(int i)
    {
        // Destroy GameObjects
        Destroy(enemies[i].gameObject);
        Destroy(enemiesHP[i].gameObject);

        // Remove objects 
        enemies.RemoveAt(i);
        enemiesHP.RemoveAt(i);
    }


    /*
     * Manage the turn-based combat
     */ 
	void Update()
	{
		if (!init)
		{
			SetActionsDropdown ();
            SetTargetsDropdown();
			init = true;
		}
        if (choiceMade)
        {
            // Player's turn
            // Get the choice from the Dropdown
            // Subtract 1 because the first index is "Make a selection"
            int choice = actionsDropDown.value - 1;

            // Get the target
            if(targetSelected || enemies.Count == 1)
            {
                int i_target = enemies.Count == 1 ? 0 : targetDropDown.value - 1;

                DisplayAttack(0, choice);

                // Apply the Action to the boss
                enemies[i_target].ApplyPlayerAction(player.GetAction(choice));
                if (enemies[i_target].hp <= 0)
                {
                    DestroyEnemyAtIndex(i_target);
                }

                i_activeChar++;


                // Set dropdown options to show any new topics
                SetActionsDropdown();
                SetTargetsDropdown();

                // Reset dropdown
                actionsDropDown.value = 0;

                // Set new turn indicator
                SetTurnIndicator();

                // Reset choiceMade
                //choiceMade = false;

                choiceMade = false;
                targetSelected = false;
            }
        }

        // Check if we have killed all enemies
        if (enemies.Count == 0)
        {
            fightOver = true;
            PersistentData.itemsCompleted++;
            Debug.Log("Ending fight and incrementing itemsCompleted to : " + PersistentData.itemsCompleted);
            SceneManager.LoadScene("Schedule");
        }

        // Player is index 0, so if they have gone then it is the enemies' turn
        if (i_activeChar  % 2 == 1 && enemies.Count > 0)
		{
            // Choose a random enemy to go
            int i_enemy = UnityEngine.Random.Range(0, enemies.Count);
            EnemyTD eActive = enemies[i_enemy];

            // Log which enemy takes a turn
            Debug.Log(String.Format("i_enemy: {0} enemies.Count: {1} i_activeChar: {2}", i_enemy, enemies.Count, i_activeChar));


			// Make Boss choose an actions
			int enemyChoice = UnityEngine.Random.Range (0, eActive.actions.Count);
            EnemyActionTD e = eActive.actions[enemyChoice];

            Debug.Log(String.Format("Enemy action dmg: {0}", e.baseDmg));

            DisplayAttack(i_enemy + 1, enemyChoice);

            // Apply the action to the player and allies
            ApplyEnemyAction (e);
            
			// Check if game is over
			CheckGameOver ();

			// Set active character back to player
			i_activeChar = 0;
		}

        // Update the HP texts
        UpdateHpText();
	}   // End Update


    void DisplayAttack(int i_char, int i_action)
    {
        Debug.Log(string.Format("i_char: {0} i_action: {1} player.actions.Count: {2} enemies.Count: {3}", i_char, i_action, player.actions.Count, enemies.Count));
        if(i_char == 0)
        {
            actionTitleText.text = string.Format("POTUS used {0}!", player.actions[i_action].title);
        }
        else
        {
            actionTitleText.text = string.Format("{0} used {1}!", enemies[i_char - 1].enemyName, enemies[i_char - 1].actions[i_action].title);
        }
    }

    

}
