using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This makes this script visible in the inspector
[System.Serializable]
public class HandleTurn {

    public string Attacker; //Name of attacker
    public string Type;//Used to differentiate between enemies and heroes
    public GameObject AttackersGameObject; //Who is attacking
    public GameObject AttackersTarget; //Who is going to be attacked


    //Which attack is performed
	public BaseAttack chosenAttack;
}
