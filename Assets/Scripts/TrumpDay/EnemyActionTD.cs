using System;

[System.Serializable]
public class EnemyActionTD : ICloneable
{
    // Topic id
    private int id;

    // String for displaying purposes
    public string title;

    // Type
    public Common.EnemyActionType type;

    // Base damage (i.e. how much people want to talk about this)
    // Lower means less hp done meaning people really want to talk about it
    public int baseDmg;

    public int maxTargets;

    public void init(int id, string title, Common.EnemyActionType type, int baseDmg, int maxTargets)
    {
        this.id = id;
        this.title = title;
        this.type = type;
        this.baseDmg = baseDmg;
        this.maxTargets = maxTargets;
    }

    public object Clone()
    {
        EnemyActionTD result = new EnemyActionTD();

        result.id           = id;
        result.title        = title;
        result.type         = type;
        result.baseDmg      = baseDmg;
        result.maxTargets   = maxTargets;

        return result;
    }
}

