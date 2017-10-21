using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour {

	public float seconds;
	public Text gui;

	void updateGUI()
	{
		int displaySeconds = (int)(seconds % 60);
		int displayMinutes = (int)(seconds / 60);
		string displayTime = String.Format ("{0}:{1}", displayMinutes.ToString ("00"), displaySeconds.ToString ("00"));

		gui.text = displayTime;
	}

	// Use this for initialization
	void Start () {
		gui = GetComponent<Text> ();
		seconds = 120.0f;
	}
	
	// Update is called once per frame
	void Update () {
		seconds -= Time.deltaTime;
		if(seconds < 0)
		{
			seconds = 0;
			SceneManager.LoadScene ("GameLost");
		}
		updateGUI ();
	}
}
