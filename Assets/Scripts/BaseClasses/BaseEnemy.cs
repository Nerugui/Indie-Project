using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This makes this script visible in the inspector
[System.Serializable]

public class BaseEnemy:BaseClass {

    //Enemy types like pokemon, something we can change later
    public enum Type
    {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE

    }

    public Type EnemyType;

    public Rarity rarity;


}
