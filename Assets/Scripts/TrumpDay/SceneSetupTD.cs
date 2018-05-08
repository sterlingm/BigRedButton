using UnityEngine;
using UnityEngine.UI;

public class SceneSetupTD : MonoBehaviour
{

    int iEnc;
    EnemyTD enemyPrefab;
    public Transform canvasTrans;
    
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


    // NOT CALLED!! 
    // Currently using the version in FightManager
    void CreateEnemyObjects()
    {
        PlayerTD player = GameObject.Find("Player").GetComponent<PlayerTD>();
        for (int i=0;i<FightManager.self.enemies.Count;i++)
        {
            Vector3 p = new Vector3(player.transform.position.x - (10 * (i + 1)), player.transform.position.y, player.transform.position.z);
            EnemyTD e = Instantiate(enemyPrefab, p, Quaternion.identity) as EnemyTD;
            GameObject t = CreateText(e, canvasTrans, p.x, p.y + 10, string.Format("{0} HP: {1}", e.name, e.hp), 10, Color.green);
            FightManager.self.enemiesHP.Add( t.GetComponent<Text>() );
        }
    }

    GameObject CreateText(EnemyTD enemy, Transform canvas_transform, float x, float y, string text_to_print, int font_size, Color text_color)
    {
        // Create object and set parent
        GameObject textObject = new GameObject(string.Format("{0} HP", enemy.name));
        textObject.transform.SetParent(canvas_transform);

        // Make a rect transform
        RectTransform trans = textObject.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(x, y);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = font_size;
        text.color = text_color;

        return textObject;
    }
}
