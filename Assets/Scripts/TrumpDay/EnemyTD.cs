using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;

public class EnemyTD : MonoBehaviour 
{

	public Common.EnemyType     enemyType;
	public Common.MovementType  movementType;

	public int          id;
	public float        hp;
	public String       enemyName;
   
    
	private List<Common.TopicType> weakTo;
	private List<Common.TopicType> strongTo;
	private float weakMod;
	private float strongMod;
    

	private BoxCollider boxCollider;
	private Rigidbody rb;
        

    // New
    private EnemyActionList actionList;
    [SerializeField]
    public List<EnemyAction> actions;

    
    
	void Awake()
	{
		weakMod = 3f;
		strongMod = -3f;

		weakTo      = new List<Common.TopicType> ();
		strongTo    = new List<Common.TopicType> ();
		weakTo.Add      (Common.TopicType.HOSTILE_TALK);
		strongTo.Add    (Common.TopicType.SHOP_TALK);
        
		boxCollider = GetComponent<BoxCollider> ();
		rb 			= GetComponent<Rigidbody> ();
     }


	void Start () 
	{
    }

    	
    void Update () 
	{
        // Check if this object has EnemyResponse values
        // If not, then it was created in the inspector 
        // Use default values to initialize its fields
        if(actions.Count < 1)
        {
            // Use default initialization for Enemy
            InitInspectorEnemy();
        }        
    }

	
    public void InitInspectorEnemy()
    {
        Debug.Log("In initInspectorEnemy");
        // Create or grab a list of default EnemyResponses
        // Add them to responses field with AddResponse
        
        // enemyType should be set in Inspector so use that
        // to set strongTo and weakTo

        // start and goal can be set in Inspector (I think?)
        // use those to set start and goal fields so the enemy can reverse
        // when it reaches the goal
        //for(int i=0;i<EnemyActionList.self.list.Count-1;i++)
        //{
            //actions.Add((EnemyAction)EnemyActionList.self.list[i].Clone());
        //}
    }

    public void SetStrongWeak()
    {
        switch(enemyType)
        {
            case Common.EnemyType.CABINET:
                weakTo.Add(Common.TopicType.HOSTILE_TALK);
                strongTo.Add(Common.TopicType.SHOP_TALK);
                break;
            case Common.EnemyType.CONGRESS:
                break;
            case Common.EnemyType.PRESS:
                break;
            default:
                break;
        }
            
    }


    private float CalculateDmg(PlayerActionTD action)
	{
		float result = action.baseDmg;

		/*if(Common.weakTo[ (int)enemyType ].Contains(action.type))
		{
			Debug.Log ("Adding weakMod");
			result += weakMod;
		}
		else if(Common.strongTo[ (int)enemyType ].Contains(action.type))
		{
			Debug.Log ("Adding strongMod");
			result += strongMod;
		}*/

		Debug.Log ("Damage: " + result);
		return result;
	}


	public void ApplyAction(PlayerActionTD action)
	{
		/*
		 *  Determine loss of hp
		 */
		hp -= CalculateDmg (action);
		Debug.Log ("hp: " + hp);
	}

	

	public override string ToString()
	{
		return String.Format ("Enemy:\n\tName: {0}\n\tHP: {1}", enemyName, hp.ToString ());
	}



}
