using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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


    public bool fightOver;
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

        fightOver = false;
    }
    

    /*
	 * Create objects for each Enemy
	 */
    void CreateEnemyObjects()
    {
        // Get the total number of enemies from somewhere based on encounter
        int n = UnityEngine.Random.Range(1, 5);
        n = 2;
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
            //Debug.Log(String.Format("i: {0} i/2: {1} x_offset: {2} z_offset: {3}", i, i / 2, x_offset, z_offset));

            // Create vector for position and instantiate an enemy
            Vector3 p = new Vector3(enemyLocRef.position.x + x_offset, enemyLocRef.position.y, enemyLocRef.position.z + z_offset);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;

            // Add enemy to list
            enemies.Add(e);

            // Create a text object based on the enemy
            GameObject t = CreateEnemyHPText(e, canvasTrans, string.Format("HP: {1}", e.name, e.hp));

            // Get location for pHP
            Vector3 pHP = camera.WorldToScreenPoint(p);
            t.transform.position = pHP;

            // Add the text object to list
            enemiesHP.Add(t.GetComponent<Text>());
        }
    }

    GameObject CreateEnemyHPText(EnemyTD enemy, Transform canvas_transform, string text_to_print)
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
        text.fontSize = 24;
        text.color = Color.red;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = enemyHPFont;

        return textObject;
    }


    private void SetTextFieldPositions()
	{
		// Set offsets
		int x_offsetTurn = 40;
		int y_offsetTurn = -55;

        /*
		 * Player
		 */

        // Set playerHP text to player location
        Vector3 playerPos = camera.WorldToScreenPoint(player.transform.position);
        playerHp.transform.position = playerPos;

        // Player turn indicator
		playerPos.x += x_offsetTurn;
		playerPos.y += y_offsetTurn;
		playerTurnText.transform.position = playerPos;        
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
            SceneManager.LoadScene("Schedule");
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
