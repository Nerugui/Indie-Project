using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonOneSpell : BaseAttack {

	public PoisonOneSpell()
	{
		attackName = "Poison";
		attackDescription = "This is a basic poison attack";
		attackDamage = 5.0f;
		attackManaCost = 5.0f;

	}
}
