using UnityEngine;
using System.Collections;

public class SimpleDoorTrigger : MonoBehaviour {
	public Transform Door;
	public float OpenAngleAmount = 88.0f;
	public float SmoothRotation;	
	public string interactText = "Press F To Interact";
	public GUIStyle InteractTextStyle;
		
	private bool init = false;
	private bool hasEntered = false;
	private bool doorOpen = false;
	private Vector3 startAngle;
	private Vector3 openAngle;
    private Vector3 currentAngle;
	private Rect interactTextRect;
		
	void Start () {
		//Check if Door Game Object is properly assigned
		if(Door == null){
			Debug.LogError (this + " :: Door Object Not Defined!");
		}

        //Init Start and Open door angles
        startAngle = Door.localEulerAngles;
        currentAngle = startAngle;
        openAngle = Door.localEulerAngles + Vector3.up * OpenAngleAmount;

        //Init Interact text Rect
        Vector2 textSize = InteractTextStyle.CalcSize(new GUIContent(interactText));
		interactTextRect = new Rect(Screen.width / 2 - textSize.x / 2, Screen.height - (textSize.y + 5), textSize.x, textSize.y);
		
		init = true;
	}
		
	void Update () {
		if(!init)
			return;
		
		HandleDoorRotation();
		HandleUserInput();	
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			hasEntered = true;
		}
	}
	
	void OnTriggerExit(Collider other){
		hasEntered = false;
	}
	
	void OnGUI(){
		if(!init || !hasEntered)
			return;
		
		GUI.Label(interactTextRect, interactText, InteractTextStyle);
	}
	
	void HandleDoorRotation(){
        if (doorOpen == false) {
            currentAngle = Vector3.Slerp(currentAngle, startAngle, Time.deltaTime * SmoothRotation);
            Door.localEulerAngles = currentAngle;            
        }
        else {
            currentAngle = Vector3.Slerp(currentAngle, openAngle, Time.deltaTime * SmoothRotation);
            Door.localEulerAngles = currentAngle;            
        }
	}
	
	void HandleUserInput(){
		if(Input.GetKeyDown(KeyCode.F) && hasEntered){
			doorOpen = !doorOpen;
		}			
	}
}