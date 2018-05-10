using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleSceneSetup : MonoBehaviour {

    private List<ScheduleItem> items;
    public Transform canvasTrans;
    public Font itemFont;

    // Use this for initialization
    void Start()
    {
        items = new List<ScheduleItem>();

        // Make some schedule items
        ScheduleItem sa = new ScheduleItem
        {
            title = "Fox and Friends",
            time = 1f,
            numEnemies = 3
        };

        ScheduleItem sb = new ScheduleItem
        {
            title = "Press conference",
            time = 0.5f,
            numEnemies = 6
        };

        items.Add(sa);
        items.Add(sb);

        for(int i=0;i<items.Count;i++)
        {
            GameObject obj = CreateTextForItem(items[i]);

            // Get location for pHP
            obj.transform.localPosition = new Vector3(0, (i + 1) * 100);
        }
    }
	

	void Update ()
    {
		
	}

    private GameObject CreateTextForItem(ScheduleItem item)
    {
        GameObject textObject = new GameObject(string.Format("{0} HP", item.title));

        // Set parent tf
        textObject.transform.SetParent(canvasTrans);

        // Add a rect transform to the object
        RectTransform rtf = textObject.AddComponent<RectTransform>();
        //rtf.anchoredPosition = new Vector2(0,0);
        rtf.sizeDelta = new Vector2(500, 50);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = item.title;
        text.fontSize = 36;
        text.color = Color.black;
        text.alignment = TextAnchor.UpperCenter;
        text.font = itemFont;


        return textObject;
    }
}
