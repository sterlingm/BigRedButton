using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsoCharControl : MonoBehaviour {

	// Make it modifiable in inspector without being public
	[SerializeField]
	float moveSpeed = 4f;

	// Axes to move the character on (different from world frame axes)
	Vector3 forward, right;

	// Use this for initialization
	void Start () 
	{
		// Set forward and right equal the camera's axes
		forward = Camera.main.transform.forward;
		right 	= Camera.main.transform.right;

		// Make sure y is always 0
		forward.y 	= 0;
		right.y 	= 0;

		// Re-normalize the vectors after changing y
		forward = Vector3.Normalize (forward);
		right 	= Vector3.Normalize (right);

		// Center camera on the player
		CenterCameraOnPlayer ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey)
		{
			Move ();
		}
	}

	void Move()
	{
		// Use the camera right and forward axes to get character movement
		Vector3 rightMovement 	= right 	* Input.GetAxis ("HorizontalKey");
		Vector3 upMovement 		= forward 	* Input.GetAxis ("VerticalKey");

		// Heading is just the normalized direction
		Vector3 heading = Vector3.Normalize (rightMovement + upMovement);

		// Move vector is the position change in heading direction based on speed*time
		Vector3 moveVec = heading * (moveSpeed * Time.deltaTime);

		// Set new heading and add position change to transform.position
		transform.forward = heading;
		transform.position += moveVec;

		// Update the camera's position so that the player stays in view
		CenterCameraOnPlayer ();
	}

	void CenterCameraOnPlayer()
	{
		Vector3 v_player = transform.position;
		v_player.y = 20;

		Camera.main.transform.position = v_player;
	}
}
