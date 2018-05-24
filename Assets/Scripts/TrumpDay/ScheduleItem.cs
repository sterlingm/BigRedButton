
public class ScheduleItem
{

    public int durInMin;
    public string title;
    public int numEnemies;
    public ITEMTYPE type;


    public Common.EnemyType enemyType;

    // TODO: Make a way to consider different types of enemies

    public enum ITEMTYPE
    {
        FOX_AND_FRIENDS,
        PRESS_CONF,
        INTEL_BRIEF
    }
}
