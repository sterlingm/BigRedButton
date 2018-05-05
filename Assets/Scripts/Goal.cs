using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {
	
	/*
	 * When Goal is entered, load the Boss scene
	 */
	void OnCollisionEnter(Collision coll)
	{
		// If player collided with goal
		// Load new scene 
		if(coll.gameObject.name == "Player")
		{
			SceneManager.LoadScene ("Boss");
		}

        
	}
}
