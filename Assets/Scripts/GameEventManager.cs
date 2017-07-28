using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class GameEventManager : MonoBehaviour
{
	private Dictionary<string, UnityEvent> eventDictNoArgs;

	private static GameEventManager gameEventManager;

	// instead of singleton, get the instance as a getter so we
	// can call Init the first time it is grabbed
	public static GameEventManager instance
	{
		get
		{
			if(!gameEventManager)
			{
				// Get reference if we don't have it
				gameEventManager = FindObjectOfType (typeof(GameEventManager)) as GameEventManager;

				// Print an error if no reference exists
				if(!gameEventManager)
				{
					Debug.LogError ("There needs to be one active GameEventManager script on a GameObject in the scene");
				}
				// If we did find one, initialize the event manager
				else
				{
					gameEventManager.Init ();
				}
			}

			return gameEventManager;
		}
	}

	void Init()
	{
		if(eventDictNoArgs == null)
		{
			eventDictNoArgs = new Dictionary<string, UnityEvent> ();
		}
	}

	// Allow for listeners to register for events
	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;

		// If we find the event, register the listener
		if(instance.eventDictNoArgs.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		}
		// Otherwise, create a new event
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictNoArgs.Add (eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, UnityAction listener)
	{
		if(gameEventManager == null)
		{
			return;
		}

		UnityEvent thisEvent = null;
		if(instance.eventDictNoArgs.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	/*
	 * Runs the callbacks for each event listener for eventName
	 */ 
	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		if(instance.eventDictNoArgs.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}
}

