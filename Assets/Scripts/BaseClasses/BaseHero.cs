using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This makes this script visible in the inspector
[System.Serializable]

public class BaseHero: BaseClass {

    //basic optional stats. Will change to ours later on
    public int stamina;
    public int intellect;
    public int dexterity;
    public int agility;

	public List<BaseAttack> magicAttacks = new List<BaseAttack> ();


}
