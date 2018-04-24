using UnityEngine;

[System.Serializable]
public class EnemyAction
{
    // Topic id
    private int id;

    // String for displaying purposes
    public string title;

    // Type
    public Common.EnemyActionType type;

    // Base damage (i.e. how much people want to talk about this)
    // Lower means less hp done meaning people really want to talk about it
    public float baseDmg;

    public int maxTargets;

    public void init(int id, string title, Common.EnemyActionType type, double baseDmg, int maxTargets)
    {
        this.id = id;
        this.title = title;
        this.type = type;
        this.baseDmg = (float)baseDmg;
        this.maxTargets = maxTargets;
    }
}

