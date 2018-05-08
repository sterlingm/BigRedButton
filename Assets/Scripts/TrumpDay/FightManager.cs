using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FightManager : MonoBehaviour
{
    // Singleton
    public static FightManager self = null;

    public PlayerTD player;
	public List<Ally> allies;
	public Dropdown dropDown;
    public EnemyTD enemy;

    public EnemyTD enemyPrefab;
    public List<EnemyTD> enemies;
    public List<Text> enemiesHP;
    public List<Text> enemiesTurn;

	public bool choiceMade;

    // Enemy stuff
    public Transform enemyLocRef;
    public Font enemyHPFont;
    public Transform canvasTrans;


    private bool init;
    private int i_activeChar;

	public Text playerTurnText;
	//public List<Text> allyTurnTexts;
	//public List<Text> allyHpTexts;
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
        n = 4;
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();

        int x_offset = 0;
        int z_offset = 0;

        // Make rows of enemies, 2 per row
        // Start at 2 to not run into issues with i=0 or 1
        // i/2 for i=0 and i=1 will be equal locations, and doing i+1 also runs into problems with making rows
        for (int i = 2; i <= n+1; i++)
        {
            if(i % 2 == 0)
            {
                x_offset = (i/2) * 3;
                z_offset = (i/2) * 3;
            }
            else
            {
                x_offset = -(i/2) * 3;
                z_offset = (i/2) * 3;
            }
            Debug.Log(String.Format("i: {0} i/2: {1} x_offset: {2} z_offset: {3}", i, i / 2, x_offset, z_offset));
            Vector3 p = new Vector3(enemyLocRef.position.x + x_offset, enemyLocRef.position.y, enemyLocRef.position.z + z_offset);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;
            enemies.Add(e);


            GameObject t = CreateText(e, canvasTrans, string.Format("{0} HP: {1}", e.name, e.hp), 10, Color.green);


            // Reset playerPos and apply HP text offset
            Vector3 pHP = camera.WorldToScreenPoint(p);
            pHP.x += 0;
            pHP.y += -75;
            t.transform.position = pHP;

            Debug.Log(String.Format("Location is ({0})", pHP));
            enemiesHP.Add(t.GetComponent<Text>());
        }
    }

    GameObject CreateText(EnemyTD enemy, Transform canvas_transform, string text_to_print, int font_size, Color text_color)
    {
        // Create object and set parent
        GameObject textObject = new GameObject(string.Format("{0} HP", enemy.name));
        textObject.transform.SetParent(canvas_transform);

        // Make a rect transform
        RectTransform trans = textObject.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(enemy.transform.position.x, enemy.transform.position.y);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = font_size;
        text.color = text_color;
        text.font = enemyHPFont;

        return textObject;
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
         * Enemies
         */ 
        // for(int i=0;i<enemies.Count;i++)
        //{
        //    Vector3 p = camera.WorldToScreenPoint(enemies[i].transform.position);
        //    p.x += x_offsetTurn;
        //    p.y += y_offsetTurn;

        //    //Text eHP = new Text();
        //    enemiesHP.Add(eHP);
        //}

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
        //enemyHp.text = String.Format("HP: {0}", enemy.hp);

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
		if(enemies.Count == 0)
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

			// Set text string to show the action
			//bossActionText.text = String.Format("Enemy used: {0}", e.title);

			// Update the HP texts
			UpdateHpText ();

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

		// Check if boss is dead
		/*if (boss.hp <= 0)
		{
			// Deal with enemy
			boss.gameObject.SetActive (false);

			// Destroy this Encounter object
			Destroy (gameObject);
		}*/
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
