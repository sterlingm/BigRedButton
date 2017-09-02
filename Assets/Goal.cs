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
	}
}
