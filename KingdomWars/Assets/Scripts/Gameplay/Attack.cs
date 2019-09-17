using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    bool initialize = false;

    GameManager gameManager;
    UIController UIManager;
    TroopAnimController animController;
    public GameObject SelectedTroop;
    public TroopScript troopScript;
    public bool troopSelected;

    public GameObject enemySelectedTroop;
    public TroopScript enemyTroopScript;

    RaycastHit TroopHit;
    Ray TroopRay;
    RaycastHit enemyTroopHit;
    Ray enemyTroopRay;

    int RedTroopMask = 1 << 9;
    int BlueTroopMask = 1 << 10;

    public bool awaitingInput;
    public bool enemyIndicator = true; 

    int dist = 0;
    public int KnightBlockChance = 35;

    GameObject tank;

    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        if (initialize == false)
        {
            gameManager = GetComponent<GameManager>();
            UIManager = GetComponent<UIController>();
            animController = GetComponent<TroopAnimController>();

            initialize = true;
        }

        //if gameplay state and attack phase of the game
        if (gameManager.state == State.Gameplay && gameManager.phase == Phase.Attack)
        {
            //if troop not already selected
            if (troopSelected == false)
            {
                SelectTroop(); //Select a troop
            }

            //if troop already selected
            else if (troopSelected == true)
            {
                //left mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    SelectEnemy(); //choose enemy to attack
                }
            }
        }

        if (awaitingInput == true)
        {
            if (UIManager.YesNo == yesno.yes)
            {
                //awaitingInput = false;
                UIManager.HideQuestion();
                UIManager.YesNo = yesno.notSelected;
                //swap tanks position with whatever troop is being targetted
                switch (gameManager.turn)
                {
                    case Turn.Red:
                        tank = gameManager.SearchList(gameManager.BlueTeam, "Tank");
                        break;

                    case Turn.Blue:
                        tank = gameManager.SearchList(gameManager.RedTeam, "Tank");
                        break;
                }

                StartCoroutine(TankAbility());
            }

            else if (UIManager.YesNo == yesno.no)
            {
                //if target troop is knight
                if (enemySelectedTroop.tag == "Knight")
                {
                    KnightBlock();
                }
                else
                {
                    //if attacking troop is archer
                    if (SelectedTroop.tag == "Archer")
                    {
                        //attack animation either here or in calculate damage
                        CalculateArcherDamage(); //call archer unique attack and calculate damage
                        //gameManager.ChangePhase();
                    }
                    else
                    {
                        //attack animation either here or in calculate damage
                        CalculateDamage(); //call attack and calculate damage
                        //gameManager.ChangePhase();
                    }
                }
                awaitingInput = false;
                UIManager.HideQuestion();
                UIManager.YesNo = yesno.notSelected;
            }
        }
    }

    //Select troop function
    void SelectTroop()
    {
        switch (gameManager.turn)
        {
            case (Turn.Red): //red turn
                //if the active piece is not a pawn
                if (gameManager.RedCurPiece.tag != "Pawn")
                {
                    //automatically select the troop and declare its variables
                    SelectedTroop = gameManager.RedCurPiece;
                    troopScript = SelectedTroop.GetComponent<TroopScript>();
                    gameManager.troopScript = troopScript;
                    troopSelected = true;
                    enemyIndicator = true;
                }
                
                //if active piece is a pawn
                else if (gameManager.RedCurPiece.tag == "Pawn")
                {
                    //left mouse click2 
                    if (Input.GetMouseButtonDown(0))
                    {
                        //cast ray to mouse position
                        TroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                        //if ray hits a red troop
                        if (Physics.Raycast(TroopRay, out TroopHit, 100, RedTroopMask))
                        {
                            //if tag of troop is pawn
                            if (TroopHit.transform.tag == "Pawn")
                            {
                                //select and declare its variables
                                SelectedTroop = TroopHit.transform.gameObject;
                                troopScript = SelectedTroop.GetComponent<TroopScript>();
                                gameManager.troopScript = troopScript;
                                troopSelected = true;
                                enemyIndicator = true;
                            }
                        }
                    }
                }
                break;

            case (Turn.Blue): //blue turn
                //if active piece is not a pawn
                if (gameManager.BlueCurPiece.tag != "Pawn")
                {
                    //automatically select the troop and declare its variables
                    SelectedTroop = gameManager.BlueCurPiece;
                    troopScript = SelectedTroop.GetComponent<TroopScript>();
                    gameManager.troopScript = troopScript;
                    troopSelected = true;
                    enemyIndicator = true;
                }

                //if active piece is a pawn
                else if (gameManager.BlueCurPiece.tag == "Pawn")
                {
                    //left mouse click
                    if (Input.GetMouseButtonDown(0))
                    {
                        //cast ray to mouse position
                        TroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                        //if ray hits a red troop
                        if (Physics.Raycast(TroopRay, out TroopHit, 100, BlueTroopMask))
                        {
                            //if tag of troop is pawn
                            if (TroopHit.transform.tag == "Pawn")
                            {
                                //select and declare its variables
                                SelectedTroop = TroopHit.transform.gameObject;
                                troopScript = SelectedTroop.GetComponent<TroopScript>();
                                gameManager.troopScript = troopScript;
                                troopSelected = true;
                                enemyIndicator = true;
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }
    }
    //Select troop function

    //Select enemy function
    void SelectEnemy()
    {
        switch (gameManager.turn)
        {
            case (Turn.Red): //red turn
                //cast ray to mouse position
                enemyTroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (SelectedTroop.tag == "Priest")
                {
                    //if ray hits a blue troop
                    if (Physics.Raycast(enemyTroopRay, out enemyTroopHit, 100, RedTroopMask))
                    {
                        //store enemy gameobject and enemy script
                        enemySelectedTroop = enemyTroopHit.transform.gameObject;
                        enemyTroopScript = enemySelectedTroop.GetComponent<TroopScript>();

                        if (enemySelectedTroop.tag != "Priest")
                        {
                            enemyTroopScript.ResetHealth();
                            animController.FaceEnemy(SelectedTroop, enemySelectedTroop);
                            enemyIndicator = false;
                        }
                    }
                }
                else
                {
                    //if ray hits a blue troop
                    if (Physics.Raycast(enemyTroopRay, out enemyTroopHit, 100, BlueTroopMask))
                    {
                        //store enemy gameobject and enemy script
                        enemySelectedTroop = enemyTroopHit.transform.gameObject;
                        enemyTroopScript = enemySelectedTroop.GetComponent<TroopScript>();

                        if (SelectedTroop.tag == "Mage")
                        {
                            if (SelectedTroop.transform.position.x == enemySelectedTroop.transform.position.x || SelectedTroop.transform.position.z == enemySelectedTroop.transform.position.z)
                            {
                                if (enemySelectedTroop.tag == "King")
                                {
                                    //if more than 1 enemy pawn on board
                                    if (gameManager.BluePawnCount > 1)
                                    {
                                        //Debug.Log("cannot attack the king as the pawns are alive."); //change so message appears in UI
                                        //Undeclare the enemy gameobject and enemy script
                                        enemySelectedTroop = null;
                                        enemyTroopScript = null;
                                    }

                                    else if (gameManager.BluePawnCount <= 1)
                                    {
                                        tank = gameManager.SearchList(gameManager.BlueTeam, "Tank"); //search list to see if tank is alive

                                        //if tank is not null/is alive
                                        if (tank != null && enemySelectedTroop.tag != "Tank")
                                        {
                                            //play tank animation & ask player if we wants to swap
                                            UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                            awaitingInput = true; //waiting for players input.
                                        }

                                        //if tank is not alive
                                        else
                                        {
                                            //attack animation either here or in calculate damage
                                            CalculateDamage(); //call attack and calculate damage
                                                               //gameManager.ChangePhase();
                                        }
                                    }
                                }

                                else
                                {
                                    tank = gameManager.SearchList(gameManager.BlueTeam, "Tank"); //search list to see if tank is alive

                                    //if tank is not null/is alive
                                    if (tank != null && enemySelectedTroop.tag != "Tank")
                                    {
                                        //play tank animation & ask player if we wants to swap
                                        //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                        UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                        awaitingInput = true; //waiting for players input.
                                    }

                                    //if target troop is knight
                                    else if (enemySelectedTroop.tag == "Knight")
                                    {
                                        KnightBlock();
                                    }

                                    //if tank is not alive
                                    else
                                    {
                                        CalculateDamage();
                                    }
                                }
                            }

                            else if (SelectedTroop.transform.position.x != enemySelectedTroop.transform.position.x && SelectedTroop.transform.position.z != enemySelectedTroop.transform.position.z)
                            {
                                //Undeclare the enemy gameobject and enemy script
                                enemySelectedTroop = null;
                                enemyTroopScript = null;
                            }
                        }

                        else
                        {
                            //if enemy is the king
                            if (enemySelectedTroop.tag == "King") //REMEMBER IF KING DIES THEN GAME OVER
                            {
                                //if more than 1 enemy pawn on board
                                if (gameManager.BluePawnCount > 1)
                                {
                                    //Debug.Log("cannot attack the king as the pawns are alive."); //change so message appears in UI
                                    //Undeclare the enemy gameobject and enemy script
                                    enemySelectedTroop = null;
                                    enemyTroopScript = null;
                                }

                                //if 1 or less enemy pawn on the board
                                else if (gameManager.BluePawnCount <= 1)
                                {
                                    bool inRange = CheckRange(); //check is enemy troop is in attack range; return bool

                                    //if return true
                                    if (inRange == true)
                                    {
                                        tank = gameManager.SearchList(gameManager.BlueTeam, "Tank"); //search list to see if tank is alive

                                        //if tank is not null/is alive
                                        if (tank != null && enemySelectedTroop.tag != "Tank")
                                        {
                                            //play tank animation & ask player if we wants to swap
                                            UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                            awaitingInput = true; //waiting for players input.
                                        }

                                        //if tank is not alive
                                        else
                                        {
                                            //if attacking troop is archer
                                            if (SelectedTroop.tag == "Archer")
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateArcherDamage(); //call archer unique attack and calculate damage
                                                                         //gameManager.ChangePhase();
                                            }
                                            else
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateDamage(); //call attack and calculate damage
                                                                   //gameManager.ChangePhase();
                                            }
                                        }
                                    }

                                    //if not in range
                                    else if (inRange == false)
                                    {
                                        //Undeclare the enemy gameobject and enemy script
                                        enemySelectedTroop = null;
                                        enemyTroopScript = null;
                                    }
                                }
                            }

                            //if king is not the target
                            else
                            {
                                Debug.Log("Test");
                                bool inRange = CheckRange(); //check is enemy troop is in attack range; return bool

                                //if return true
                                if (inRange == true)
                                {
                                    tank = gameManager.SearchList(gameManager.BlueTeam, "Tank"); //search list to see if tank is alive

                                    //if tank is not null/is alive
                                    if (tank != null && enemySelectedTroop.tag != "Tank")
                                    {
                                        //play tank animation & ask player if we wants to swap
                                        //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                        UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                        awaitingInput = true; //waiting for players input.
                                    }

                                    //if tank is not alive
                                    else
                                    {
                                        //if target troop is knight
                                        if (enemySelectedTroop.tag == "Knight")
                                        {
                                            KnightBlock();
                                        }
                                        else
                                        {
                                            //if attacking troop is archer
                                            if (SelectedTroop.tag == "Archer")
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateArcherDamage(); //call archer unique attack and calculate damage
                                                                         //gameManager.ChangePhase();
                                            }
                                            else
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateDamage(); //call attack and calculate damage
                                                                   //gameManager.ChangePhase();
                                            }
                                        }
                                    }
                                }

                                //if not in range
                                else if (inRange == false)
                                {
                                    //Undeclare the enemy gameobject and enemy script
                                    enemySelectedTroop = null;
                                    enemyTroopScript = null;
                                }
                            }
                        }
                    }
                }
                break;

            case (Turn.Blue):
                //cast ray to mouse position
                enemyTroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (SelectedTroop.tag == "Priest")
                {
                    //if ray hits a blue troop
                    if (Physics.Raycast(enemyTroopRay, out enemyTroopHit, 100, BlueTroopMask))
                    {
                        //store enemy gameobject and enemy script
                        enemySelectedTroop = enemyTroopHit.transform.gameObject;
                        enemyTroopScript = enemySelectedTroop.GetComponent<TroopScript>();

                        if (enemySelectedTroop.tag != "Priest")
                        {
                            enemyTroopScript.ResetHealth();
                            animController.FaceEnemy(SelectedTroop, enemySelectedTroop);
                            enemyIndicator = false;
                        }
                    }
                }

                else
                {
                    //if ray hits a blue troop
                    if (Physics.Raycast(enemyTroopRay, out enemyTroopHit, 100, RedTroopMask))
                    {
                        //store enemy gameobject and enemy script
                        enemySelectedTroop = enemyTroopHit.transform.gameObject;
                        enemyTroopScript = enemySelectedTroop.GetComponent<TroopScript>();

                        if (SelectedTroop.tag == "Mage")
                        {
                            if (SelectedTroop.transform.position.x == enemySelectedTroop.transform.position.x || SelectedTroop.transform.position.z == enemySelectedTroop.transform.position.z)
                            {

                                if (enemySelectedTroop.tag == "King")
                                {
                                    Debug.Log("4.1 enemy selected troop: " + enemySelectedTroop);
                                    Debug.Log("4.1 enemy troop script: " + enemyTroopScript);
                                    //if more than 1 enemy pawn on board
                                    if (gameManager.RedPawnCount > 1)
                                    {
                                        //Debug.Log("cannot attack the king as the pawns are alive."); //change so message appears in UI
                                        //Undeclare the enemy gameobject and enemy script
                                        enemySelectedTroop = null;
                                        enemyTroopScript = null;
                                        Debug.Log("calling");
                                    }

                                    else if (gameManager.RedPawnCount <= 1)
                                    {
                                        tank = gameManager.SearchList(gameManager.RedTeam, "Tank"); //search list to see if tank is alive

                                        //if tank is not null/is alive
                                        if (tank != null && enemySelectedTroop.tag != "Tank")
                                        {
                                            //play tank animation & ask player if we wants to swap
                                            //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                            UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                            awaitingInput = true; //waiting for players input.
                                        }

                                        //if tank is not alive
                                        else
                                        {
                                            //attack animation either here or in calculate damage
                                            CalculateDamage(); //call attack and calculate damage
                                                               //gameManager.ChangePhase();
                                        }
                                    }
                                }

                                else
                                {
                                    tank = gameManager.SearchList(gameManager.RedTeam, "Tank"); //search list to see if tank is alive

                                    //if tank is not null/is alive
                                    if (tank != null && enemySelectedTroop.tag != "Tank")
                                    {
                                        //play tank animation & ask player if we wants to swap
                                        //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                        UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                        awaitingInput = true; //waiting for players input.
                                    }

                                    //if target troop is knight
                                    else if (enemySelectedTroop.tag == "Knight")
                                    {
                                        KnightBlock();
                                    }

                                    else
                                    {
                                        CalculateDamage();
                                    }
                                }
                            }

                            else if (SelectedTroop.transform.position.x != enemySelectedTroop.transform.position.x && SelectedTroop.transform.position.z != enemySelectedTroop.transform.position.z)
                            {
                                //Undeclare the enemy gameobject and enemy script
                                enemySelectedTroop = null;
                                enemyTroopScript = null;
                                Debug.Log("calling");
                            }
                        }
                        else
                        {
                            //if enemy is the king
                            if (enemySelectedTroop.tag == "King") //REMEMBER IS KING DIES THEN GAME OVER
                            {
                                //if more than 1 enemy pawn on board
                                if (gameManager.RedPawnCount > 1)
                                {
                                    Debug.Log("cannot attack the king as the pawns are alive."); //change so message appears in UI
                                                                                                 //Undeclare the enemy gameobject and enemy script
                                    enemySelectedTroop = null;
                                    enemyTroopScript = null;
                                    Debug.Log("calling");
                                }

                                //if 1 or less enemy pawn on the board
                                else if (gameManager.RedPawnCount <= 1)
                                {
                                    bool inRange = CheckRange(); //check is enemy troop if in attack range; return bool

                                    //if return true
                                    if (inRange == true)
                                    {
                                        tank = gameManager.SearchList(gameManager.RedTeam, "Tank"); //search list to see if tank is alive

                                        //if tank is not null/is alive
                                        if (tank != null && enemySelectedTroop.tag != "Tank")
                                        {
                                            //play tank animation & ask player if we wants to swap
                                            //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                            UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                            awaitingInput = true; //waiting for players input.
                                        }

                                        //if tank is not alive
                                        else
                                        {
                                            //if attacking troop is archer
                                            if (SelectedTroop.tag == "Archer")
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateArcherDamage(); //call archer unique attack and calculate damage
                                                                         //gameManager.ChangePhase();
                                            }
                                            else
                                            {
                                                //attack animation either here or in calculate damage
                                                CalculateDamage(); //call attack and calculate damage
                                                                   //gameManager.ChangePhase();
                                            }
                                        }
                                    }

                                    //if not in range
                                    else if (inRange == false)
                                    {
                                        //Undeclare the enemy gameobject and enemy script
                                        enemySelectedTroop = null;
                                        enemyTroopScript = null;
                                        Debug.Log("calling");
                                    }
                                }
                            }

                            //if king is not the target
                            else
                            {
                                Debug.Log("Test");
                                bool inRange = CheckRange(); //check if enemy troop is in attack range; return bool

                                //if return true
                                if (inRange == true)
                                {
                                    tank = gameManager.SearchList(gameManager.RedTeam, "Tank"); //search list to see if tank is alive

                                    //if tank is not null/is alive
                                    if (tank != null && enemySelectedTroop.tag != "Tank")
                                    {
                                        //play tank animation & ask player if we wants to swap
                                        //REMOVE Debug.Log("Do you want to swap your tanks position with target? Y/N"); //change so it appears un UI
                                        UIManager.TankQuestion(enemySelectedTroop.tag.ToString());
                                        awaitingInput = true; //waiting for players input.
                                    }

                                    //if tank is not alive
                                    else
                                    {
                                        //if target troop is knight
                                        if (enemySelectedTroop.tag == "Knight")
                                        {
                                            KnightBlock();
                                        }
                                        else
                                        {
                                            //if attacking troop is archer
                                            if (SelectedTroop.tag == "Archer")
                                            {
                                                CalculateArcherDamage(); //call archer unique attack and calculate damage
                                            }
                                            else
                                            {
                                                Debug.Log(SelectedTroop);
                                                Debug.Log(enemySelectedTroop);
                                                CalculateDamage(); //call attack and calculate damage
                                            }
                                        }
                                    }
                                }

                                //if not in range
                                else if (inRange == false)
                                {
                                    //Undeclare the enemy gameobject and enemy script
                                    enemySelectedTroop = null;
                                    enemyTroopScript = null;
                                    Debug.Log("calling");
                                }
                            }
                        }
                    }
                }
                break;

            default:
                break;
        }
    }
    //Select enemy function

    //Check range function
    bool CheckRange()
    {
        dist = 0; //reset dist

        //Declare troop pos variables
        int troopX = Mathf.RoundToInt(troopScript.Startx);
        int troopY = Mathf.RoundToInt(troopScript.Starty);

        //Declare enemy pos variables
        int enemyX = Mathf.RoundToInt(enemyTroopScript.Startx);
        int enemyY = Mathf.RoundToInt(enemyTroopScript.Starty);

        //calculate difference between variables
        int difX = troopX - enemyX;
        int difY = troopY - enemyY;

        //if difX is negative integer, make posative
        if (difX < 0)
        {
            difX = difX * -1;
        }

        //if difY is negative integer, make posative
        if (difY < 0)
        {
            difY = difY * -1;
        }

        //calculate distance
        dist = difX + difY;

        //distance is less than or equal to attack dist then return true
        if (dist <= troopScript.attackDist) { return true; }

        else { return false; }
    }
    //Check range function

    //Calculate damage function
    void CalculateDamage()
    {
        enemyIndicator = false;
        var health = enemyTroopScript.Health;
        var damage = troopScript.attackDamage;

        Debug.Log("Starting health: " + health);
        health = health - damage;
        enemyTroopScript.Health = health;
        Debug.Log("Finishing health: " + enemyTroopScript.Health);

        animController.FaceEnemy(SelectedTroop, enemySelectedTroop);
    }
    //Calculate damage function

    //Calculate archer damage function
    void CalculateArcherDamage()
    {
        enemyIndicator = false;
        var health = enemyTroopScript.Health; //get enemy health

        var missChance = 50 - (dist * 5); //multiple dist by miss% multiplier

        int Roll;
        int Damage = 0;
        Debug.Log(missChance); //test again
        //repeat loop 3 times, one for each archer shot
        for (int i = 0; i < 3; i++)
        {
            Roll = Random.Range(0, 100); //get random number

            if (Roll > missChance) //is the roll in the range
            {
                //maybe play animation between each roll
                Damage = Damage + troopScript.attackDamage; //if so increase damage by archers attack
            }
        }
        //calculate damage and set remaining health
        health = health - Damage; 
        enemyTroopScript.Health = health;

        animController.FaceEnemy(SelectedTroop, enemySelectedTroop);
    }
    //Calculate archer damage function

    //Tank ability
    public IEnumerator TankAbility()
    {
        var tankScript = tank.GetComponent<TroopScript>();
        tankScript.tileScript.PlaySmokeParticles();
        enemyTroopScript.tileScript.PlaySmokeParticles();

        yield return new WaitForSeconds(1f);

        //Store the enemy tank and the target troop positions
        Vector3 TankPos = new Vector3(tank.transform.position.x, enemySelectedTroop.transform.position.y, tank.transform.position.z);
        Vector3 TargetPos = new Vector3(enemySelectedTroop.transform.position.x, tank.transform.position.y, enemySelectedTroop.transform.position.z);
        //Swap the positions of the tank and the target troop
        tank.transform.position = TargetPos;
        enemySelectedTroop.transform.position = TankPos;
        tankScript.ReCallStartPos();
        enemyTroopScript.ReCallStartPos();

        //Set tank as new target troop and get its script
        enemySelectedTroop = tank.gameObject;
        enemyTroopScript = enemySelectedTroop.GetComponent<TroopScript>();

        //Calculate damage and chance phase
        CalculateDamage();
        awaitingInput = false;
        //gameManager.ChangePhase();
        
    }
    //Tank ability

    //Knight ability
    void KnightBlock()
    {
        int roll = Random.Range(0, 100); //roll for knights ability

        if (roll > KnightBlockChance)
        {
            //if attacking troop is archer
            if (SelectedTroop.tag == "Archer")
            {
                //attack animation either here or in calculate damage
                CalculateArcherDamage(); //call archer unique attack and calculate damage
                //gameManager.ChangePhase();
            }
            else
            {
                Debug.Log("Block Failed");
                //attack animation either here or in calculate damage
                CalculateDamage(); //call attack and calculate damage
                //gameManager.ChangePhase();
            }
        }
        else
        {
            //Play defence animation
            Debug.Log("Attack Blocked");
            gameManager.ChangePhase();
        }
    }
    //Knight ability
}
