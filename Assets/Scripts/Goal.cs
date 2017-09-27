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
		// Load new scene
		SceneManager.LoadScene ("Boss");
	
		// Put Player in front of Boss
		/*if(coll.gameObject.name == "Player"d)
		{
			Vector3 p_new = new Vector3 (130, 0.5f, 130);
			coll.gameObject.transform.position = p_new;
		}*/
	}
}
