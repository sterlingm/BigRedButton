using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IsoCharControl : MonoBehaviour 
{

	// Make it modifiable in inspector without being public
	[SerializeField]
	float moveSpeed = 0.0f;

	// Axes to move the character on (different from world frame axes)
	Vector3 forward, right;

	// Offset between camera and player
	private Vector3 offset;

	// Main camera
	Camera camMain;

	// Player (set in inspector)
	public Player player;

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

		// Reference to main camera
		camMain = Camera.main;

		// Desired offset between camera and player based on the initial positions
		offset = camMain.transform.position - transform.position;

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

	// Use LateUpdate to move camera? I tried it once and couldn't tell a significant difference
	/*void LateUpdate()
	{
		CenterCameraOnPlayer ();
	}*/

	void Move()
	{
		// Use the camera right and forward axes to get character movement
		Vector3 rightMovement 	= right 	* Input.GetAxis ("HorizontalKey");
		Vector3 upMovement 		= forward 	* Input.GetAxis ("VerticalKey");

		// Heading is just the normalized direction
		Vector3 heading = Vector3.Normalize (rightMovement + upMovement);

		// Move vector is the position change in heading direction based on speed*time
		Vector3 moveVec = heading * (moveSpeed * Time.deltaTime);
		Debug.Log(String.Format("position: {0} moveVec: {1}", transform.position.ToString(), moveVec.ToString()));
		// Set new heading and add position change to transform.position
		transform.forward = heading;
		transform.position += moveVec;
		Debug.Log(String.Format("After, position: {0}", transform.position.ToString()));
				

		// Update the camera's position so that the player stays in view
		CenterCameraOnPlayer ();
	}

	void CenterCameraOnPlayer()
	{
		// Set the camera's position based on the player's position and desired offset
		camMain.transform.position = player.transform.position + offset;
	}
}
