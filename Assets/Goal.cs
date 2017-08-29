using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*
	 * When Goal is entered, load the Boss scene
	 */
	void OnCollisionEnter(Collision coll)
	{
		// Load new scene
		SceneManager.LoadScene ("Boss");
	}
}
