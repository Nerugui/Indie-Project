using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class BaseAttack : MonoBehaviour {

	public string attackName;
	public string attackDescription;

	public float attackDamage; //Base damage
	public float attackManaCost; //If it's a spell attack this is referenced (Mana Cost)
}
