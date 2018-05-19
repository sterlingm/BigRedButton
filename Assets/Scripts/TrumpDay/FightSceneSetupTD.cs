using UnityEngine;
using UnityEngine.UI;

public class FightSceneSetupTD : MonoBehaviour
{

    int iEnc;
    public EnemyTD enemyPrefab;
    public Transform canvasTrans;


    // Enemy stuff
    public Transform enemyLocRef;
    public Font enemyHPFont;

    // Camera to use camera.WorldToScreen
    public Camera isoCamera;


    // Use this for initialization
    void Awake() 
	{
        iEnc = 0;
	}

    private void Start()
    {
        SetupPlayer();
    }


    /*
	 * Setup Player stuff for the scene
	 * new position, disable movement, etc
	 */
    void SetupPlayer()
	{
		// Set new player position
		PlayerTD player = GameObject.Find ("Player").GetComponent<PlayerTD> ();
		Vector3 playerPos = new Vector3 (25f, 0.5f, 15f);
		player.transform.position = playerPos;

		// Set player action list for player object
		player.actionList = GameObject.Find ("PlayerActionList").GetComponent<PlayerActionListTD> ();

        // Prune list?
        player.GetPrunedList(iEnc);

        CreateEnemyObjects();
	}



    /*
	 * Create objects for each Enemy
	 */
    void CreateEnemyObjects()
    {
        // Get the total number of enemies from somewhere based on encounter
        int n = Random.Range(1, 5);
        n = 2;

        int x_offset = 0;
        int z_offset = 0;

        // Make rows of enemies, 2 per row
        // Start at 2 to not run into issues with i=0 or 1
        // i/2 for i=0 and i=1 will be equal locations, and doing i+1 also runs into problems with making rows
        for (int i = 2; i <= n + 1; i++)
        {
            if (i % 2 == 0)
            {
                x_offset = (i / 2) * 3;
                z_offset = (i / 2) * 3;
            }
            else
            {
                x_offset = -(i / 2) * 3;
                z_offset = (i / 2) * 3;
            }
            //Debug.Log(String.Format("i: {0} i/2: {1} x_offset: {2} z_offset: {3}", i, i / 2, x_offset, z_offset));

            // Create vector for position and instantiate an enemy
            Vector3 p = new Vector3(enemyLocRef.position.x + x_offset, enemyLocRef.position.y, enemyLocRef.position.z + z_offset);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;

            // Add enemy to list
            FightManager.self.enemies.Add(e);

            // Create a text object based on the enemy
            GameObject t = CreateEnemyHPText(e, canvasTrans, string.Format("HP: {1}", e.name, e.hp));

            // Get location for pHP
            Vector3 pHP = isoCamera.WorldToScreenPoint(p);
            t.transform.position = pHP;

            // Add the text object to list
            FightManager.self.enemiesHP.Add(t.GetComponent<Text>());
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
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();
        Vector3 playerPos = isoCamera.WorldToScreenPoint(player.transform.position);
        FightManager.self.playerHp.transform.position = playerPos;

        // Player turn indicator
        playerPos.x += x_offsetTurn;
        playerPos.y += y_offsetTurn;
        FightManager.self.playerTurnText.transform.position = playerPos;
    }

}
