using System.Collections.Generic;
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
	public Dropdown dropDown;
    public EnemyTD enemy;

    public EnemyTD enemyPrefab;
    public List<EnemyTD> enemies;
    public List<Text> enemiesHP;
    public List<Text> enemiesTurn;

	public bool choiceMade;

    public bool fightOver;
    private bool init;
    private int i_activeChar;


	public Text playerTurnText;
	//public List<Text> allyTurnTexts;
	//public List<Text> allyHpTexts;
	public Text playerHp;
    

	void Awake()
	{
        if(self == null)
        {
            self = this;
        }

		// Set dropdown object
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/BossFightActions").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

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
		
        // Get actions for the player
        List<string> actionStrs = player.GetActionStrings();

		// Insert "Make a selection to prompt the user
		actionStrs.Insert (0, "Make a selection");

		// Clear and re-set the options	
		dropDown.ClearOptions ();
		dropDown.AddOptions (actionStrs);
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
			UnityEngine.SceneManagement.SceneManager.LoadScene ("GameWon");
		}

		if(player.hp <= 0)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene ("GameLost");
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
			SetOptions ();
			init = true;
		}
        if (choiceMade)
        {
            // Player's turn
            // Get the choice from the Dropdown
            // Subtract 1 because the first index is "Make a selection"
            int choice = dropDown.value - 1;

            int i_target = 0;
            
            // Apply the Action to the boss
            enemies[i_target].ApplyPlayerAction(player.GetAction(choice));
            if(enemies[i_target].hp <= 0)
            {
                DestroyEnemyAtIndex(i_target);
            }
            
            i_activeChar++;

            choiceMade = false;
        }

		// Player is index 0, so if they have gone then it is the enemies' turn
		if(i_activeChar > 0 && i_activeChar <= enemies.Count)
		{
            Debug.Log(String.Format("enemies.Count: {0} i_activeChar: {1}", enemies.Count, i_activeChar));
            EnemyTD eActive = enemies[i_activeChar - 1];

			// Make Boss choose an actions
			int enemyChoice = UnityEngine.Random.Range (0, eActive.actions.Count);
            EnemyActionTD e = eActive.actions[enemyChoice];

            Debug.Log(String.Format("Enemy action dmg: {0}", e.baseDmg));

			// Apply the action to the player and allies
			ApplyEnemyAction (e);

			// Check if game is over
			CheckGameOver ();

			// Set active character back to player
			i_activeChar++;
		}
        // Once all enemies have gone, reset the index back to 0
		else
		{
			i_activeChar = 0;
        }

        // Update the HP texts
        UpdateHpText();

        // Set dropdown options to show any new topics
        SetOptions ();

		// Reset dropdown
		dropDown.value = 0;

		// Set new turn indicator
		SetTurnIndicator ();
        
		// Reset choiceMade
		choiceMade = false;

        if(enemies.Count == 0)
        {
            fightOver = true;
            PersistentData.itemsCompleted++;
            SceneManager.LoadScene("Schedule");
        }
	}   // End Update



    

}
