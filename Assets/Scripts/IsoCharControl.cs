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
		right = Vector3.Normalize (right);

		string s = String.Format("Camera.right: {0},{1},{2}", Camera.main.transform.right.x, Camera.main.transform.right.y, Camera.main.transform.right.z);
		Debug.Log (s);
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
		Vector3 rightMovement 	= right   * moveSpeed * Time.deltaTime * Input.GetAxis ("HorizontalKey");
		Vector3 upMovement 		= forward * moveSpeed * Time.deltaTime * Input.GetAxis ("VerticalKey");
		Vector3 moveVec 		= rightMovement + upMovement;

		Vector3 heading = Vector3.Normalize (moveVec);

		transform.forward = heading;
		transform.position += moveVec;
	}
}
