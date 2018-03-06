using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;

    //We make a public feild of classes, this is why we serialized the base class
    public BaseHero hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING, //may not need this
        ACTION,
        DEAD
    };

    public TurnState currentState;

    //For the progress bar
    private float curr_cooldown = 0.0f;
    private float max_cooldown = 5.0f;

    private Image progressBar;

    public GameObject selector;

    //For the IENumerator 
    public GameObject enemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10.0f;

	//Dead state
	private bool alive = true;

	//hero panel
	private HeroPanelStats stats;
	public GameObject heroPanel;
	private Transform heroPanelSpacer;

	void Start () {

		//Find spacer object
		heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");

		//Create panel, fill in information
		CreateHeroPanel();

        //Affects the progress bar, making starting random. Can be influenced by stats we put in later.
        curr_cooldown = Random.Range(0, 2.5f);

        selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;

        //Storing the start position
        startPosition = transform.position;

    }
	

	void Update () {

        //Testing state switches
        //Debug.Log(currentState);
        switch(currentState)
        {
		case (TurnState.PROCESSING):
			UpdateProgressBar ();
			break;
		case (TurnState.ADDTOLIST):
			BSM.heroesToManage.Add (this.gameObject);
			currentState = TurnState.WAITING;
                break;
		case (TurnState.WAITING):
			   //idle
            break;
		case (TurnState.ACTION):
				
			StartCoroutine (TimeForAction ());
			break;
		case (TurnState.DEAD):

			if (!alive) {

				return;
			} else {

				//change tag
				this.gameObject.tag = "DeadHero";
				//not attackable by enemy
				BSM.HeroesInBattle.Remove(this.gameObject);
				//not able to manage hero anymore
				BSM.heroesToManage.Remove(this.gameObject);
				//deactivate the selector
				selector.SetActive(false);
				//reset gui
				BSM.actionPanel.SetActive(false);
				BSM.enemySelectPanel.SetActive(false);

				//remove item from perform list
				if (BSM.HeroesInBattle.Count > 0) {
					for (int i = 0; i < BSM.PerformList.Count; i++) {

						//this removes the handleturn which may have been done by the player before the heor died
						if (BSM.PerformList [i].AttackersGameObject == this.gameObject) {
					
							BSM.PerformList.Remove (BSM.PerformList [i]);
						}

						//changes enemy target to a random hero if the hero it previously selected died
						if (BSM.PerformList [i].AttackersTarget == this.gameObject) {
					
							BSM.PerformList [i].AttackersTarget = BSM.HeroesInBattle [Random.Range (0, BSM.HeroesInBattle.Count)];
						}
					}
				}
				//change color /play death animation
				this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);
				//reset the hero input
				BSM.heroInput = BattleStateMachine.HeroGUI.ACTIVATE;

				//You dead son
				BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
				alive = false;
			}

                break;
        }

	}

    void UpdateProgressBar()
    {
        curr_cooldown = curr_cooldown + Time.deltaTime;
        //Getting a ratio for us to use for scaling the progress bar
        float calc_cooldown = curr_cooldown / max_cooldown;
        progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

        if (curr_cooldown >= max_cooldown)
        {
            currentState = TurnState.ADDTOLIST;
        }
    }


    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the hero to go near the enemy it is attacking
        Vector3 targetPosition = new Vector3(enemyToAttack.transform.position.x + 1.5f, enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        while (MoveTowardsTarget(targetPosition)) { yield return null; }//while this is happening do nothing

        //wait a bit
        yield return new WaitForSeconds(0.5f);
        //do damage
		DoDamage();

        //animate hero back to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }//while this is happening do nothing

        //remove this performer from the BSM list
        BSM.PerformList.RemoveAt(0);

        //reset BSM -> WAIT
		if (BSM.battleState != BattleStateMachine.PerformAction.WIN && BSM.battleState != BattleStateMachine.PerformAction.LOSE) {
			BSM.battleState = BattleStateMachine.PerformAction.WAIT;
			//reset enemy state
			curr_cooldown = 0.0f;
			currentState = TurnState.PROCESSING;
		} else {
			currentState = TurnState.WAITING;
		}
		//end coroutine
		actionStarted = false;
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

	public void TakeDamage(float getDamageAmount)
	{
		hero.currentHP -= getDamageAmount;

		if (hero.currentHP <= 0) {
			hero.currentHP = 0;
			currentState = TurnState.DEAD;
		}
		UpdateHeroPanel ();
	}

	//Do damage
	void DoDamage()
	{
		//Does heroes base damage as well as damage of attack chosen which is grabbed from the perform list
		float calculatedDamage = hero.curATK + BSM.PerformList [0].chosenAttack.attackDamage; //this is where the base attack chosen in bsm script effects the base attack
		enemyToAttack.GetComponent<EnemyStateMachine> ().TakeDamage (calculatedDamage);
	}

	//Creates hero panel
	void CreateHeroPanel()
	{
		heroPanel = Instantiate (heroPanel) as GameObject;
		stats = heroPanel.GetComponent<HeroPanelStats> ();
		stats.heroName.text = hero.theName;
		stats.heroHP.text = "HP: " + hero.currentHP;
		stats.heroMP.text = "MP: " + hero.currentMP;

		progressBar = stats.progressBar;
		heroPanel.transform.SetParent (heroPanelSpacer, false);

	}

	//Update stats on damage or healing
	void UpdateHeroPanel()
	{
		stats.heroHP.text = "HP: " + hero.currentHP;
		stats.heroMP.text = "MP: " + hero.currentMP;
	}
}
