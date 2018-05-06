using UnityEngine;

public class SceneSetupTD : MonoBehaviour
{

    int iEnc;
    EnemyTD enemyPrefab;
    
	// Use this for initialization
	void Awake() 
	{
        iEnc = 0;
		SetupPlayer ();
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
        
	}


    void createEnemyObjects()
    {
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();
        for (int i=0;i<FightManager.self.enemies.Count;i++)
        {
            Vector3 p = new Vector3(player.transform.position.x - (10 * (i + 1)), player.transform.position.y, player.transform.position.z);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;
        }
    }
}
