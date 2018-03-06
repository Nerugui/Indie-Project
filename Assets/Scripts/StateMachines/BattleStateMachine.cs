using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour {


    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
		CHECKALIVE,
		WIN,
		LOSE
    };

    //A list made to keep track of who's turn it currently is
    public List<HandleTurn> PerformList = new List<HandleTurn>();

    //Lists filled with what objects are currently in battle
    public List<GameObject> HeroesInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    public PerformAction battleState;



    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUTONE,
        INPUTTWO,
        DONE
    }

    public HeroGUI heroInput;

    public List<GameObject> heroesToManage = new List<GameObject>();
    private HandleTurn heroChoice;

    public GameObject enemyButton;
    public Transform spacer;

    //Taking in panels from the battle canvas
    public GameObject actionPanel;
    public GameObject enemySelectPanel;
	public GameObject magicPanel;

	//magic attack
	public Transform actionSpacer;
	public Transform magicSpacer;
	public GameObject actionButton;
	public GameObject magicButton;
	private List<GameObject> atkBtns = new List<GameObject> ();

	//enemy buttons
	private List<GameObject> enemyBtns = new List<GameObject>();


	void Start () {

        //Starting state 
        battleState = PerformAction.WAIT;

        //Populates the lists with all enemies or heroes in battle scene 
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        //Starting state
        heroInput = HeroGUI.ACTIVATE;

        //Deactivating panels on start
        actionPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
		magicPanel.SetActive (false);

        EnemyButtons();


    }
	
	void Update () {

        switch (battleState)
        {
            case (PerformAction.WAIT):
                if(PerformList.Count > 0)
                {
                    battleState = PerformAction.TAKEACTION;
                }
                break;
		case (PerformAction.TAKEACTION):
                //Grabbing the performer that should currently be attacking
			GameObject performer = GameObject.Find (PerformList [0].Attacker);
			if (PerformList [0].Type == "Enemy") {
				//Getting the state machine from the enemy that is currently attacking
				EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine> ();

				//Checks if the hero they are attacking is still alive
				for (int i = 0; i < HeroesInBattle.Count; i++) {

					if (PerformList [0].AttackersTarget == HeroesInBattle [i]) {
				
						//Setting the hero to attack based on the handleturn object that was sent over to the performlist
						ESM.heroToAttack = PerformList [0].AttackersTarget;
						ESM.currentState = EnemyStateMachine.TurnState.ACTION;
						break;
					} else {

						//Makes enemy attack a new targer if old target is dead
						PerformList[0].AttackersTarget = HeroesInBattle[Random.Range(0,HeroesInBattle.Count)];

						ESM.heroToAttack = PerformList [0].AttackersTarget;
						ESM.currentState = EnemyStateMachine.TurnState.ACTION;
					}
				}

			}
			if (PerformList [0].Type == "Hero")
                {
                    //Getting the state machine from the hero that is currently attacking
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>(); //WE'RE ALL IN THIS TOGETHER

                    //Setting the enemy to attack based on the handleturn object that was sent over to the performlist
                    HSM.enemyToAttack = PerformList[0].AttackersTarget;

                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }

                battleState = PerformAction.PERFORMACTION;
             
                break;
            case (PerformAction.PERFORMACTION):
                //idle
                break;

			//visual studio crapped out on me so monogames if f#$king with all the spacing when putting in new code....
		case(PerformAction.CHECKALIVE): //This handles win and lose changes and dead hero cleanup

			if (HeroesInBattle.Count < 1) {

				//lose battle
				battleState = PerformAction.LOSE;

			} else if (EnemiesInBattle.Count < 1) {

				//win battle
				battleState = PerformAction.WIN;
			} else {
				//call function
				ClearActionPanel();
				heroInput = HeroGUI.ACTIVATE;
			}

			break;
		case(PerformAction.WIN):
			Debug.Log ("You win");
			for (int i = 0; i < HeroesInBattle.Count; i++) {
			
				HeroesInBattle [i].GetComponent<HeroStateMachine> ().currentState = HeroStateMachine.TurnState.WAITING;
			}
			break;
		case(PerformAction.LOSE):
			Debug.Log ("You lose");
			break;
        }

        switch(heroInput)
        {
            case (HeroGUI.ACTIVATE):

                //Checks to see if there are any heroes to manage 
                if(heroesToManage.Count > 0)
                {
                    heroesToManage[0].transform.Find("Selector").gameObject.SetActive(true);
					//vreates new handleturn instance
                    heroChoice = new HandleTurn();

                    actionPanel.SetActive(true);

					//populates action buttons
					CreateAttackButtons();

                    heroInput = HeroGUI.WAITING;
                }

                break;

            case (HeroGUI.WAITING):
                //idle state
                break;

            case (HeroGUI.DONE):

                HeroInputDone();

                break;

        }
    }

    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);
    }

    //Adds enemy buttons into the UI dynamically
    public void EnemyButtons()
    {
		//cleanup
		foreach (GameObject enemyBtn in enemyBtns) {
			Destroy (enemyBtn);
		}
		enemyBtns.Clear ();

		//create buttons
        foreach (GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine curr_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>(); //uses findChild in tutorial, see if this works
            buttonText.text = curr_enemy.enemy.theName;

            button.enemyGameObject = enemy;

            newButton.transform.SetParent(spacer,false); //This false is supposed to stop an overflow error from ocassionally occuring 
			enemyBtns.Add(newButton);
        }
    }

    //This fills up the handle turn info when the attack button is pressed
    public void InputOne()
    {
        //When making seperate heroes we will have the change this to grab the name from their script instead
        heroChoice.Attacker = heroesToManage[0].name;

        //Sets the attackers game object to the hero being managed right now
        heroChoice.AttackersGameObject = heroesToManage[0];

        //Making sure it is of type hero
        heroChoice.Type = "Hero";

		//Makes heroes chosen base attack be the first thing on the attack list in the prefab
		heroChoice.chosenAttack = heroesToManage [0].GetComponent<HeroStateMachine> ().hero.attacks [0];

        //Don't need the attack panel anymore since attack button was pressed
        actionPanel.SetActive(false);

        //Enables enemy select so we can choose which enemy to attack
        enemySelectPanel.SetActive(true);
    }

    //Filling handle turn info with the attackers target
    public void InputTwo(GameObject chosenEnemy)
    {
        heroChoice.AttackersTarget = chosenEnemy;

        //starts the next case
        heroInput = HeroGUI.DONE;
    }

    //Cleans up after turn is all done
    void HeroInputDone()
    {
        //adds the hero choice turn into the list of turns to perform
        PerformList.Add(heroChoice);

		//clears action panel
		ClearActionPanel ();
        
        heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);

        //Removes the current hero from the list because it's action has already been done and handled. 
        //This will move the next hero on the list into the "0" space so they can be managed as well
        //The removed heroes will get added back in when there progress bar fills back up again
        heroesToManage.RemoveAt(0);

        heroInput = HeroGUI.ACTIVATE;
    }

	//Clears the action panel 
	void ClearActionPanel()
	{
		enemySelectPanel.SetActive(false);
		actionPanel.SetActive (false);
		magicPanel.SetActive (false);

		//clean the attack panel
		foreach (GameObject atkBtn in atkBtns) {

			Destroy (atkBtn);
		}
		atkBtns.Clear ();

	}

	//Create action buttons
	void CreateAttackButtons()
	{
		GameObject AttackButton = Instantiate (actionButton) as GameObject;
		Text AttackButtonText = AttackButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
		AttackButtonText.text = "Attack";
		//adds a listener to the button through script
		AttackButton.GetComponent<Button> ().onClick.AddListener (() => InputOne ());
		AttackButton.transform.SetParent (actionSpacer, false);
		atkBtns.Add (AttackButton);

		GameObject MagicAttackButton = Instantiate (actionButton) as GameObject;
		Text MagicAttackButtonText = MagicAttackButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
		MagicAttackButtonText.text = "Magic";
		//adds a listener to the button through script
		MagicAttackButton.GetComponent<Button> ().onClick.AddListener (() => InputThree ());
		MagicAttackButton.transform.SetParent (actionSpacer, false);
		atkBtns.Add (MagicAttackButton);

		if (heroesToManage [0].GetComponent<HeroStateMachine> ().hero.magicAttacks.Count > 0) {
		
			foreach (BaseAttack magicAttack in heroesToManage [0].GetComponent<HeroStateMachine> ().hero.magicAttacks) {

				GameObject MagicButton = Instantiate (magicButton) as GameObject;
				Text MagicButtonText = MagicButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
				MagicButtonText.text = magicAttack.attackName;

				//Creating reference to attack button script so access and populate magicAttackToPerform
				AttackButton ATB = MagicButton.GetComponent<AttackButton> ();
				ATB.magicAttackToPerform = magicAttack;
				MagicButton.transform.SetParent (magicSpacer, false);
				atkBtns.Add (MagicButton);

			}
		} else {

			//Make it so if you have no magic you can't interact with the magic button
			MagicAttackButton.GetComponent<Button> ().interactable = false;
		}
	}

	public void InputThree()//switching to magic attacks
	{
		actionPanel.SetActive (false);
		magicPanel.SetActive (true);
	}

	public void InputFour(BaseAttack chosenMagic)//Chosen magic attack
	{
		//When making seperate heroes we will have the change this to grab the name from their script instead
		heroChoice.Attacker = heroesToManage[0].name;

		//Sets the attackers game object to the hero being managed right now
		heroChoice.AttackersGameObject = heroesToManage[0];

		//Making sure it is of type hero
		heroChoice.Type = "Hero";

		heroChoice.chosenAttack = chosenMagic;
		magicPanel.SetActive (false);
		enemySelectPanel.SetActive (true);
	}
		
}
