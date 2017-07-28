using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class EncounterEventManager : MonoBehaviour
{
	private Dictionary<string, EncounterEvent> eventDict;

	private static EncounterEventManager encEventManager;

	// instead of singleton, get the instance as a getter so we
	// can call Init the first time it is grabbed
	public static EncounterEventManager instance
	{
		get
		{
			if(!encEventManager)
			{
				// Get reference if we don't have it
				encEventManager = FindObjectOfType (typeof(EncounterEventManager)) as EncounterEventManager;

				// Print an error if no reference exists
				if(!encEventManager)
				{
					Debug.LogError ("There needs to be one active GameEventManager script on a GameObject in the scene");
				}
				// If we did find one, initialize the event manager
				else
				{
					encEventManager.Init ();
				}
			}

			return encEventManager;
		}
	}

	void Init()
	{
		if(eventDict == null)
		{
			eventDict = new Dictionary<string, EncounterEvent> ();
		}
	}

	// Allow for listeners to register for events
	public static void StartListening(string eventName, UnityAction<Enemy> listener)
	{
		EncounterEvent thisEvent = null;

		// If we find the event, register the listener
		if(instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		}
		// Otherwise, create a new event
		else
		{
			thisEvent = new EncounterEvent ();
			thisEvent.AddListener (listener);
			instance.eventDict.Add (eventName, thisEvent);
		}
	}

	public static void StopListening(string eventName, UnityAction<Enemy> listener)
	{
		if(encEventManager == null)
		{
			return;
		}

		EncounterEvent thisEvent = null;
		if(instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}

	/*
	 * Runs the callbacks for each event listener for eventName
	 */ 
	public static void TriggerEvent(string eventName, Enemy e)
	{
		EncounterEvent thisEvent = null;
		if(instance.eventDict.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke (e);
		}
	}
}

