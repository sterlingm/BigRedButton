using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossFightManager : MonoBehaviour {

	public Player player;
	public Boss boss;

	public Dropdown dropDown;

	public bool choiceMade;

	private bool init;

	void Awake()
	{
		choiceMade = false;
		dropDown = GameObject.Find ("/GUI/BossFightActions").GetComponent<Dropdown> ();
		dropDown.onValueChanged.AddListener(DropdownValueChanged);

		init = false;
		choiceMade = false;
	}


	private void DropdownValueChanged(int choice)
	{
		Debug.Log ("In DropdownValueChanged");
		if(!choiceMade)
		{
			choiceMade = true;
		}
	}

	private void setOptions()
	{
		List<string> actionStrs = player.GetActionStrings ();
		actionStrs.Insert (0, "Make a selection");
		dropDown.ClearOptions ();
		dropDown.AddOptions (actionStrs);
	}

	void Update()
	{
		if (!init)
		{
			setOptions ();
			init = true;
		}
		if(choiceMade)
		{
			// Player's turn
			// Get the choice from the Dropdown
			// Subtract 1 because the first index is "Make a selection"
			int choice = dropDown.value-1;

			// Apply the Action to the boss
			boss.hp -= player.actions [choice].damage;

			// Set dropdown options to show any new topics
			setOptions ();

			// Reset dropdown
			dropDown.value = 0;

			// Reset choiceMade
			choiceMade = false;
		}

		// Check if boss is dead


		// Check if enemy is dead
		if (boss.hp <= 0)
		{
			// Deal with enemy
			boss.gameObject.SetActive (false);

			// Destroy this Encounter object
			Destroy (gameObject);
		}
	}
}
