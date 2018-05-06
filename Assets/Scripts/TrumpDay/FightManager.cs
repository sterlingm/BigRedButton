using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FightManager : MonoBehaviour
{
    // Singleton
    public static FightManager self = null;

    public PlayerTD player;
	public Boss boss;
	public List<Ally> allies;
	public Dropdown dropDown;
    public EnemyTD enemy;

    public EnemyTD enemyPrefab;
    public List<EnemyTD> enemies;
    public List<Text> enemiesHP;
    public List<Text> enemiesTurn;

	public bool choiceMade;


	public Text bossActionText;


    private bool init;
    private int i_activeChar;

	public Text playerTurnText;
	//public List<Text> allyTurnTexts;
	//public List<Text> allyHpTexts;
	public Text enemyTurnText;
    public Text enemyHp;
	public Text playerHp;

	public Camera camera;

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


		// Set text field positions for Hp, turn indicators, etc
		// Do this programatically because we don't know how many allies the player will have
		SetTextFieldPositions ();

		// Set the turn indicator
		SetTurnIndicator ();

        // Set index of active character
        i_activeChar = 0;
        
		// End initialization
		init = false;

        // Create the enemies
        CreateEnemyObjects();
	}
    

    /*
	 * Create objects for each Enemy
	 */
    void CreateEnemyObjects()
    {
        // Get the total number of enemies from somewhere based on encounter
        int n = UnityEngine.Random.Range(1, 5);
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();

        for (int i = 0; i < n; i++)
        {
            Vector3 p = new Vector3(player.transform.position.x - (10 * (i + 1)), player.transform.position.y, player.transform.position.z);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;
            enemies.Add(e);
        }
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
		enemyTurnText.transform.position = potusPos;

        /*
		 * Allies
		 */
        //SetAllyTextFieldPositions();

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
		//List<string> actionStrs = i_activeChar == 0 ? player.GetActionStrings () 
		//											: player.allies [i_activeChar - 1].GetActionsStrs ();

        List<string> actionStrs = player.GetActionStrings();

		// Insert "Make a selection to prompt the user
		actionStrs.Insert (0, "Make a selection");

		// Clear and re-set the options	
		dropDown.ClearOptions ();
		dropDown.AddOptions (actionStrs);
	}

	void UpdateHpText()
	{
		playerHp.text = String.Format ("HP: {0}", player.hp);
        enemyHp.text = String.Format("HP: {0}", enemy.hp);

		/*if(allies.Count > 0)
		{
			allyOneHp.text = String.Format ("HP: {0}", allies [0].hp);
		}
		if(allies.Count > 1)
		{
			allyTwoHp.text = String.Format ("HP: {0}", allies [1].hp);
		}*/
	}
		
	void SetTurnIndicator()
	{
		playerTurnText.gameObject.SetActive (false);
		enemyTurnText.gameObject.SetActive (false);

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
		else
		{
			enemyTurnText.gameObject.SetActive (true);
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

    void ApplyEnemyAction(EnemyActionTD e)
    {
        player.ApplyEnemyAction(e);
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
        if (choiceMade)
        {
            // Player's turn
            // Get the choice from the Dropdown
            // Subtract 1 because the first index is "Make a selection"
            int choice = dropDown.value - 1;

            // Apply the Action to the boss
            enemy.ApplyAction(player.GetAction(choice));

            i_activeChar++;

            choiceMade = false;
        }

			// Player is index 0, so if they have gone then it is the enemies' turn
			if(i_activeChar > 0 && i_activeChar < enemies.Count)
			{
                Debug.Log(String.Format("enemies.Count: {0} i_activeChar: {1}", enemies.Count, i_activeChar));
                EnemyTD eActive = enemies[i_activeChar - 1];

				// Make Boss choose an actions
				int enemyChoice = UnityEngine.Random.Range (0, eActive.actions.Count);
                EnemyActionTD e = eActive.actions[enemyChoice];

				// Apply the action to the player and allies
				ApplyEnemyAction (e);

				// Set text string to show the action
				bossActionText.text = String.Format("Enemy used: {0}", e.title);

				// Update the HP texts
				UpdateHpText ();

				// Check if game is over
				CheckGameOver ();

				// Set active character back to player
				i_activeChar++;
			}
            // Once all enemies have gone, reset the index back to 0
			else if(i_activeChar == enemies.Count)
			{
				i_activeChar = 0;
			}

			// Set dropdown options to show any new topics
			SetOptions ();

			// Reset dropdown
			dropDown.value = 0;

			// Set new turn indicator
			SetTurnIndicator ();

			// Set new boss HP
			//enemyHp.text = String.Format ("HP: {0}", enemy.hp);

			// Reset choiceMade
			choiceMade = false;
		//}

		// Check if boss is dead
		if (boss.hp <= 0)
		{
			// Deal with enemy
			boss.gameObject.SetActive (false);

			// Destroy this Encounter object
			Destroy (gameObject);
		}
	}   // End Update









    /*private void SetAllyTextFieldPositions()
    {
        
        // Can't put these into Lists because then we can't set the values in the Inspector
        // So set them both manually instead of in a loop
        if (player.allies.Count > 0)
        {
            Ally temp = GameObject.Find("Ally 1").GetComponent<Ally>();
            Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
            screenPos.x += x_offsetTurn;
            screenPos.y += y_offsetTurn;
            allyOneTurnText.transform.position = screenPos;

            screenPos = camera.WorldToScreenPoint(temp.transform.position);
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
        if (player.allies.Count > 1)
        {
            Ally temp = GameObject.Find("Ally 2").GetComponent<Ally>();
            Vector3 screenPos = camera.WorldToScreenPoint(temp.transform.position);
            screenPos.x += x_offsetTurn;
            screenPos.y += y_offsetTurn;
            allyTwoTurnText.transform.position = screenPos;

            screenPos = camera.WorldToScreenPoint(temp.transform.position);
            screenPos.x += x_offsetHp;
            screenPos.y += y_offsetHp;
            allyTwoHp.transform.position = screenPos;
        }
        
    }*/


}
