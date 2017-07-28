using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Test1 : MonoBehaviour {

	private UnityAction someListener;

	void Awake()
	{
		someListener = new UnityAction (SomeFunction);
	}


	void OnEnable()
	{
		GameEventManager.StartListening ("test1", someListener);
	}

	void OnDisable()
	{
		GameEventManager.StopListening ("test1", someListener);
	}

	void SomeFunction()
	{
		Debug.Log ("In SomeFunction()");
	}
}
