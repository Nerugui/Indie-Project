using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOneSpell : BaseAttack {

	public FireOneSpell()
	{
		attackName = "Fire";
		attackDescription = "This is a basic fire attack";
		attackDamage = 20.0f;
		attackManaCost = 10.0f;

	}
}
