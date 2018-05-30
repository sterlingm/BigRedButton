using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleSceneSetup : MonoBehaviour {

    private List<ScheduleItem> items;
    private List<Text> itemTextObjs;
    private List<LineRenderer> itemLines;


    public Transform canvasTrans;
    public Font itemFont;
    public int curTime;

    // Use this for initialization
    void Start()
    {
        // The default time for starting the day is 8am
        curTime = 800;

        items = new List<ScheduleItem>();
        itemTextObjs = new List<Text>();
        itemLines = new List<LineRenderer>();

        // Make some schedule items
        ScheduleItem sa = new ScheduleItem
        {
            title = "Fox and Friends",
            durInMin = 60,
            numEnemies = 3,
            type = ScheduleItem.ITEMTYPE.FOX_AND_FRIENDS,
            enemyType = Common.EnemyType.FOX_ANCHOR
        };

        ScheduleItem sb = new ScheduleItem
        {
            title = "Press conference",
            durInMin = 30,
            numEnemies = 6,
            type = ScheduleItem.ITEMTYPE.PRESS_CONF,
            enemyType = Common.EnemyType.PRESS
        };

        ScheduleItem sc = new ScheduleItem
        {
            title = "Intelligence briefing",
            durInMin = 60,
            numEnemies = 4,
            type = ScheduleItem.ITEMTYPE.INTEL_BRIEF,
            enemyType = Common.EnemyType.HOUSE_R
        };

        items.Add(sa);
        items.Add(sb);
        items.Add(sc);

        SetItemTexts();

        // Set the next item
        SetNextFight();
    }

    private void SetItemTexts()
    {
        for (int i = 0; i < items.Count; i++)
        {
            GameObject obj = CreateTextForItem(items[i], curTime);
            curTime = AddDurToTime(curTime, items[i].durInMin);

            // Get location for pHP
            obj.transform.localPosition = new Vector3(0, (items.Count - i) * 100);
            if (i == PersistentData.itemsCompleted)
            {
                obj.GetComponent<Text>().color = Color.red;
            }
            itemTextObjs.Add(obj.GetComponent<Text>());

            CreateLineForItem(i);
        }
    }
 
    private void SetNextFight()
    {
        if(PersistentData.itemsCompleted < items.Count)
        {
            Debug.Log(string.Format("Setting next item. itemsCompleted: {0} (int)type: {1}", PersistentData.itemsCompleted, items[PersistentData.itemsCompleted].type));
            PersistentData.nextItem = items[PersistentData.itemsCompleted];
        }
        else
        {
            Debug.Log("No more items to complete");
        }
    }

	void Update ()
    {
		
	}

    // Consider when dur >= 60
    private int AddDurToTime(int t, int dur)
    {
        int hours = dur / 60;
        int minutes = dur % 60;

        // Initialize
        int result = t;

        // Hours
        // Were using military time to multiply by 100
        result += (hours * 100);

        // Minutes
        result += minutes;        

        return result;
    }

    // Time is the actual time right now
    // range is [800-2000]
    private string GetTimeString(int time)
    {
        string result = "";

        int hours = time / 100;
        int minutes = time % 100;
        bool am = true;


        // Check if minutes is >60
        if(minutes >= 60)
        {
            minutes -= 60;
            hours++;
        }

        // Check for PM
        if (hours > 12)
        {
            hours -= 12;
            am = false;
        }

        result = string.Format("{0}:{1} {2}", hours.ToString("00"), minutes.ToString("00"), am ? "AM" : "PM");

        return result;
    }

    private GameObject CreateLineForItem(int i)
    {
        GameObject lineObject = new GameObject("Done Marker");
        lineObject.transform.SetParent(canvasTrans);


        // Add the text component
        LineRenderer lRender = lineObject.AddComponent<LineRenderer>();
        lRender.positionCount = 2;
        /*{
            positionCount = 2
        };*/
        lRender.SetPosition(0, new Vector3(0, itemTextObjs[i].transform.position.y, 0));
        lRender.SetPosition(1, new Vector3(itemTextObjs[i].transform.position.x+100f, itemTextObjs[i].transform.position.y, itemTextObjs[i].transform.position.z));

        lRender.startColor = Color.red;
        lRender.endColor = Color.red;

        return lineObject;
    }

    private GameObject CreateTextForItem(ScheduleItem item, int curTime)
    {
        int t = AddDurToTime(curTime, item.durInMin);
        string tStr = GetTimeString(t);
        GameObject textObject = new GameObject(string.Format("{0} Text", item.title));

        // Set parent tf
        textObject.transform.SetParent(canvasTrans);

        // Add a rect transform to the object
        RectTransform rtf = textObject.AddComponent<RectTransform>();
        //rtf.anchoredPosition = new Vector2(0,0);
        rtf.sizeDelta = new Vector2(500, 50);

        // Add the text component
        Text text = textObject.AddComponent<Text>();
        text.text = string.Format("{0} - {1}", tStr, item.title);
        text.fontSize = 36;
        text.color = Color.black;
        text.alignment = TextAnchor.UpperCenter;
        text.font = itemFont;


        return textObject;
    }
}
