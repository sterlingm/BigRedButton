using UnityEngine;

public class Boss : MonoBehaviour
{
	public int hp;

	//public List<BossAction> actions;

	[SerializeField]
	public BossActionList actionList;

	void Awake()
	{
	}

	public void ApplyPlayerAction(PlayerAction pa)
	{
		hp -= pa.baseDmg;
	}
}

