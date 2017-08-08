using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NUnit.Framework;

public class Enemy : MonoBehaviour {

	public EnemyType type;
	public float hp;
	public String enemyName;
	public String initEncounter;
	public Text textbox;
	public bool collidingWithPlayer;

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
	}

	// Use this for initialization
	void Start () 
	{
	}


	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnCollisionEnter(Collision coll)
	{
		Debug.Log ("Hey we're in collision!");
		if(coll.gameObject.name == "Player" && !collidingWithPlayer)
		{
			textbox.text = String.Format ("{0}, I'm {1}", initEncounter, enemyName);
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

		if(weakTo.Contains(topic.type))
		{
			Debug.Log ("Adding weakMod");
			result += weakMod;
		}
		else if(strongTo.Contains(topic.type))
		{
			Debug.Log ("Adding strongMod");
			result += strongMod;
		}

		Debug.Log ("Damage: " + result);
		return result;
	}

	public IEnumerator DisplayResponse()
	{
		textbox.text = "";
		foreach(char letter in lastResponse.response.ToCharArray())
		{
			textbox.text += letter;

			yield return new WaitForSeconds (0.05f);
		}
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
		//StartCoroutine (DisplayResponse ());
	}

	public override string ToString()
	{
		return String.Format ("Enemy:\n\tName: {0}\n\tHP: {1}", enemyName, hp.ToString ());
	}



}
