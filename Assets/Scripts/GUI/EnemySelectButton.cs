using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour {

    public GameObject enemyGameObject;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().InputTwo(enemyGameObject);//Save input of enemy prefab
    }

	//Functions just for the selector to appear above enemys when hovering over button
	public void HideSelector()
	{
		enemyGameObject.transform.Find ("Selector").gameObject.SetActive (false);
	}

	public void ShowSelector()
	{
		enemyGameObject.transform.Find ("Selector").gameObject.SetActive (true);
	}
}
