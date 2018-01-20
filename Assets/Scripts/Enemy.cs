using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NUnit.Framework;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{

	public Common.EnemyType enemyType;
	public Common.MovementType movementType;

	public int id;
	public float hp;
	public String enemyName;
	public String initEncounter;
	public Text textbox;
	public bool collidingWithPlayer;
	public Transform navGoalTF;
   

	private Transform player;
	private List<Common.TopicType> weakTo;
	private List<Common.TopicType> strongTo;
	private float weakMod;
	private float strongMod;


	private BoxCollider boxCollider;
	private Rigidbody rb;

	private TopicList topicList;
	[SerializeField]
	public Dictionary<Topic, EnemyResponse> responses;
	public EnemyResponse lastResponse;

	public Vector3 start;
	public Vector3 goal;
	public Vector3 goalOrig;
	public List<Vector3> movementDetails;

	public bool move;
	public float speed;
	public float goalThreshold;

    private NavMeshAgent agent;
    private bool movingToGoal;

    // These are used to set the NavMeshAgent destinations
    private Vector3 navStart;
    private Vector3 navGoalPersist;


    public void StopMoving()
    {
        agent.isStopped = true;
    }
    public void StartMoving()
    {
        agent.isStopped = false;
    }


    public void AddResponse(EnemyResponse er)
	{
		responses.Add (topicList.list [er.i_topic], er);
	}

	void Awake()
	{
		hp = 10f;
		weakMod = 3f;
		strongMod = -3f;
		collidingWithPlayer = false;
		speed = 5f;
		goalThreshold = 0.15f;
		move = true;

		weakTo = new List<Common.TopicType> ();
		strongTo = new List<Common.TopicType> ();
		weakTo.Add (Common.TopicType.HOSTILE_TALK);
		strongTo.Add (Common.TopicType.SHOP_TALK);

		// Get and store transform of the player
		player = GameObject.FindGameObjectWithTag ("Player").transform;

		textbox = GameObject.Find ("Enemy Text").GetComponent<Text> ();
		topicList = GameObject.Find ("Topic List").GetComponent<TopicList> ();

		boxCollider = GetComponent<BoxCollider> ();
		rb 			= GetComponent<Rigidbody> ();

		responses = new Dictionary<Topic, EnemyResponse> ();

        /*
         * Old movement code for prototype
         */ 
		start = new Vector3 ();
		goal = new Vector3 ();
		movementDetails = new List<Vector3> ();

        // Set the Vector3 objects to use as destinations
        navStart        = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        navGoalPersist  = new Vector3(navGoalTF.position.x, navGoalTF.position.y, navGoalTF.position.z);
        movingToGoal    = true;

        Debug.Log("In Awake, navGoal: "+navGoalTF.position.ToString());
    }


	void Start () 
	{
		// Get reference to nav mesh agent
		agent = GetComponent<NavMeshAgent> ();

        // TODO: Why does agent.destination have y-value of 0.1 when navGoalPersist has y=0.8?
        agent.SetDestination(new Vector3(navGoalPersist.x, navGoalPersist.y, navGoalPersist.z));
    }


	
    void Update () 
	{
        // Remove this because NavMeshAgent will move the enemy now
        //Move ();

        // Check if navmeshagent has reached its goal 
        // If so, then reverse it
        //if(agent.remainingDistance < 0.1f && movingToGoal)
        if (agent.remainingDistance < 0.1f && movingToGoal)
        {
            agent.SetDestination(navStart);
            movingToGoal = false;
        }
        else if(agent.remainingDistance < 0.1f)
        {
            agent.SetDestination(navGoalPersist);
            movingToGoal = true;
        }
        else
        {
            Debug.Log(String.Format("Distance: {0}", agent.remainingDistance));
            Debug.Log(String.Format("agent.destination: {0} navGoalPersist.position: {1} navStart.position: {2}", agent.destination, navGoalPersist, navStart));
        }
    }

	void OnCollisionEnter(Collision coll)
	{
		if(coll.gameObject.name == "Player" && !collidingWithPlayer)
		{
			if(textbox.text.Length == 0)
			{
				textbox.text = String.Format ("{0}, I'm {1}", initEncounter, enemyName);
			}
			textbox.gameObject.SetActive (true);
			EncounterEventManager.TriggerEvent (Common.ENC_EVENT_STR, this);
			collidingWithPlayer = true;
		}
		else
		{
			Debug.Log (String.Format("coll.gameObject.name == {0}", coll.gameObject.name));
			collidingWithPlayer = false;
		}
	}

	private float CalculateDmg(Topic topic)
	{
		float result = topic.baseDmg;

		if(Common.weakTo[(int)enemyType].Contains(topic.type))
		{
			Debug.Log ("Adding weakMod");
			result += weakMod;
		}
		else if(Common.strongTo[(int)enemyType].Contains(topic.type))
		{
			Debug.Log ("Adding strongMod");
			result += strongMod;
		}

		Debug.Log ("Damage: " + result);
		return result;
	}


	public void ApplyTopic(Topic topic)
	{
		/*
		 *  Determine loss of hp
		 */
		hp -= CalculateDmg (topic);
		Debug.Log ("hp: " + hp);

		// Set response
		responses.TryGetValue (topic, out lastResponse);
		if(lastResponse.response.CompareTo("") == 0)
		{
			lastResponse = Common.enemyDefaultResp;
		}
	}

	public void Move()
	{
		// Check that the enemy should be moving
		if(move)
		{
			// Get vector between goal and start
			// Set y to 0 because transform will have nonzero y value based on environment plane

			// Both goal, start, and transform.position are relative to world frame
			Vector3 diff = goal - transform.position;
			diff.y = 0;

			// Check that we are greater than some threshold
			if (diff.magnitude > goalThreshold)
			{
				// Get the correct axes to move on
				Vector3 forward = Vector3.Normalize (Camera.main.transform.forward);
				Vector3 right = Vector3.Normalize (Camera.main.transform.right);

				// Project the diff vector onto the movement/camera axes
				Vector3 a = Vector3.Project (diff, forward);
				Vector3 b = Vector3.Project (diff, right);

				// Get a heading vector
				Vector3 heading = Vector3.Normalize (a + b);

				// Get movement vector
				Vector3 moveVec = heading * speed * Time.deltaTime;

				// Don't increase y value
				moveVec.y = 0;

				// Apply movement
				transform.position += moveVec;
			}
			// Else if it is at the goal (i.e. not at the start)
			else if( (goal - start).magnitude > 0.1 && movementDetails.Count > 0)
			{
				goal = start;
			}
			// Else if it's back at the start
			else
			{
				goal = goalOrig;
			}
		}	// end if move
	}

	public override string ToString()
	{
		return String.Format ("Enemy:\n\tName: {0}\n\tHP: {1}", enemyName, hp.ToString ());
	}



}
