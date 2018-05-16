using UnityEngine;
using UnityEngine.UI;

public class ScheduleSceneManager : MonoBehaviour {

    public static ScheduleSceneManager self = null;

    public Button readyButton;


    // Use this for initialization
    void Start ()
    {
		if(self == null)
        {
            self = this;
        }


        readyButton.onClick.AddListener(ReadyButtonListener);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    public void ReadyButtonListener()
    {
        Debug.Log("In ReadyButtonListener");
        
        // Go into fight scene
    }

}
