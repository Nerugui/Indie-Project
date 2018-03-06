using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;

    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    };

    public TurnState currentState;

    //for the progress bar
    private float curr_cooldown = 0.0f;
    private float max_cooldown = 8.0f;

	public GameObject selector;

    //taking gamespace values for this gameobject
    private Vector3 startPosition;

    //timeforaction variables
    private bool actionStarted = false;
    public GameObject heroToAttack;
    private float animSpeed = 10.0f;

	//alive
	private bool alive = true;

    // Use this for initialization
    void Start () {

        currentState = TurnState.PROCESSING;

		selector.SetActive(false);

        //Grabs the communal battle state machine from the scene
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();

        startPosition = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
        //Testing state switches
        //Debug.Log(currentState);
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpdateProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //idle state
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
		case (TurnState.DEAD):

			if (!alive) {

				return;
			} else {

				//change tag of enemy
				this.gameObject.tag = "DeadEnemy";
				//not attackable by heroes
				BSM.EnemiesInBattle.Remove(this.gameObject);
				//dissable selector
				selector.SetActive(false);
				//remove all input from this enemy from perform list
				if (BSM.EnemiesInBattle.Count > 0) {
					for (int i = 0; i < BSM.PerformList.Count; i++) {
				
						if (BSM.PerformList [i].AttackersGameObject == this.gameObject) {
					
							BSM.PerformList.Remove (BSM.PerformList [i]);
						}
						//changes heroes target to a random enemy if the enemy it previously selected died
						if (BSM.PerformList [i].AttackersTarget == this.gameObject) {

							BSM.PerformList [i].AttackersTarget = BSM.EnemiesInBattle [Random.Range (0, BSM.EnemiesInBattle.Count)];
						}

					}
				}
				//change color/death animations
				this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);
				//set alive to be false
				alive = false;
				//reset enemy buttons
				BSM.EnemyButtons();
				//check alive
				BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
			}



                break;
        }
    }

    void UpdateProgressBar()
    {
        curr_cooldown = curr_cooldown + Time.deltaTime;

        if (curr_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();

        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttackersGameObject = this.gameObject;
        myAttack.AttackersTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];

		//chooses random attack from enemy from ones in it's list
		int num = Random.Range (0, enemy.attacks.Count);
		myAttack.chosenAttack = enemy.attacks [num];
		Debug.Log (this.gameObject.name + " has chosen " + myAttack.chosenAttack.attackName + " and do " + myAttack.chosenAttack.attackDamage + " damage!");


        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if(actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy to go near the hero it is attacking
        Vector3 targetPosition = new Vector3(heroToAttack.transform.position.x - 1.5f, heroToAttack.transform.position.y, heroToAttack.transform.position.z);
        while (MoveTowardsTarget(targetPosition)){yield return null;}//while this is happening do nothing

        //wait a bit
        yield return new WaitForSeconds(0.5f);
        //do damage
		DoDamage();

        //animate enemy back to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }//while this is happening do nothing
        
        //remove this performer from the BSM list
        BSM.PerformList.RemoveAt(0);
        
        //reset BSM -> WAIT

        actionStarted = false;
        BSM.battleState = BattleStateMachine.PerformAction.WAIT;

        //reset hero state
        curr_cooldown = 0.0f;
        currentState = TurnState.PROCESSING;
    }

    //Check and see if these are reduntant if he never gets rid of one of them
    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

	void DoDamage()
	{
		//Does enemy base damage as well as damage of attack chosen which is grabbed from the perform list
		float calculatedDamage = enemy.curATK + BSM.PerformList [0].chosenAttack.attackDamage;
		heroToAttack.GetComponent<HeroStateMachine> ().TakeDamage (calculatedDamage);
	}

	public void TakeDamage(float getDamageAmount)
	{
		enemy.currentHP -= getDamageAmount;

		if (enemy.currentHP <= 0) {
			enemy.currentHP = 0;
			currentState = TurnState.DEAD;
		}
	}
}
