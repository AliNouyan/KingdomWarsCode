using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    bool initialize = false;

    public DragAndDrop drag;
    Attack attack;
    CameraMovement camMove;
    UIController UIManager;
    public TroopScript troopScript;
    public GameObject RedCurPiece;
    public GameObject BlueCurPiece;
    public int BlueCount = 0;
    public int RedCount = 0;

    int i = 1;

    public State state = State.TroopPlacement;
    public Phase phase = Phase.Movement;
    public Turn turn = Turn.Red;

    public List<GameObject> RedTeam = new List<GameObject>();
    public List<GameObject> BlueTeam = new List<GameObject>();

    public List<GameObject> RedPawns = new List<GameObject>();
    public List<GameObject> BluePawns = new List<GameObject>();

    public List<GameObject> deadRedTeam = new List<GameObject>();
    public List<GameObject> deadBlueTeam = new List<GameObject>();

    public GameObject RedPawnPrefab;
    public GameObject BluePawnPrefab;

    public int RedPawnCount;
    public int BluePawnCount;

    GameObject newpawn;
    int arri = 0;
    float id;

    bool CheckingChecks = false;
    public bool AwaitingResponse = false;
    public bool PauseChecks = false;
    public int CheckCount = 0;

    //ability fuinctions
    public int RedKingAbilityCount = 0;
    public bool priestRedRevive = false;
    public int BlueKingAbilityCount = 0;
    public bool priestBlueRevive = false;
    public revive ReviveTroop;
    public GameObject chosenTroop;

    bool resetPawn = true;

    bool effectsToggled = false;
    public GameObject effects;

    // Use this for initialization
    void Start () {


        //RedCurPiece = RedTeam[RedCount];
        //BlueCurPiece = BlueTeam[BlueCount];
    }
	
	// Update is called once per frame
	void Update () {
        if (initialize == false)
        {        
            //Get other script componenets
            drag = GetComponent<DragAndDrop>();
            attack = GetComponent<Attack>();
            camMove = GetComponent<CameraMovement>();
            UIManager = GetComponent<UIController>();

            initialize = true;
        }

        //Get the number of pawns in the game
        RedPawnCount = GameObject.FindGameObjectsWithTag("Pawn").Where(x => x.GetComponent<Pawn>().team == Team.Red).Where(x => x.GetComponent<Pawn>().troopState == TroopState.Alive).Count<GameObject>();
        BluePawnCount = GameObject.FindGameObjectsWithTag("Pawn").Where(x => x.GetComponent<Pawn>().team == Team.Blue).Where(x => x.GetComponent<Pawn>().troopState == TroopState.Alive).Count<GameObject>();

        switch (state)
        {
            case (State.TroopPlacement):
                //Start the game once all the troops have been placed
                if(BlueTeam.Count == 6)
                {
                    StartGame();
                }

                break;

            case (State.Gameplay):
                //The teams current piece is the count
                RedCurPiece = RedTeam[RedCount];

                if(BlueTeam[BlueCount] != null)
                {
                    BlueCurPiece = BlueTeam[BlueCount];
                }

                if (resetPawn == true)
                {
                    StartCoroutine(ResetDeadPawn());
                }

                if(turn == Turn.Red)
                {
                    troopScript = RedCurPiece.GetComponent<TroopScript>();
                }
                else if(turn == Turn.Blue)
                {
                    troopScript = BlueCurPiece.GetComponent<TroopScript>();
                }

                //Change phase if middle mouse button is placed
                if (Input.GetMouseButtonDown(2) && phase != Phase.Checks)
                {
                    ChangePhase();
                }

                if (AwaitingResponse == true)
                {
                    //if player presses N key
                    if (UIManager.YesNo == yesno.no)
                    {
                        AwaitingResponse = false;
                        CheckCount++;
                        StartCoroutine(PerformChecks());
                        UIManager.YesNo = yesno.notSelected;
                        UIManager.HideQuestion();
                    }
                    //if players presses Y key
                    else if (UIManager.YesNo == yesno.yes)
                    {
                        PauseChecks = true; //pause checks
                        AwaitingResponse = false; //stop waiting for a response
                        UIManager.YesNo = yesno.notSelected;
                        UIManager.HideQuestion();

                        //this switch method will allow me to easily add new abilities and checks
                        switch (CheckCount)
                        {
                            case (0): //if priest ability
                                Debug.Log("Calling Revive Function");
                                var priest = SearchList(RedTeam, "Priest"); //find priest
                                UIManager.RevealDeadTroops();
                                //UIManager.HideQuestion();
                                break;

                            case (1):
                                Debug.Log("Spawning Pawn");
                                drag.kingAbility = true;
                                if (turn == Turn.Red)
                                {
                                    RedKingAbilityCount = 0; //reset the count
                                    drag.spawnedRedPawn = Instantiate(RedPawnPrefab, new Vector3(0, 0.1f, 0), new Quaternion()); //instantiate new gameobject
                                }
                                else if (turn == Turn.Blue)
                                {
                                    BlueKingAbilityCount = 0; //reset the count
                                    drag.spawnedBluePawn = Instantiate(BluePawnPrefab, new Vector3(0, 0.1f, 0), new Quaternion(0, 1, 0, 0)); //instantiate new gameobject, rotate to face red team
                                }
                                break;

                            default:
                                break;
                        }
                    }
                }
                break;

            default:

                break;
        }
    }

    //Start game function
    public void StartGame()
    {
        Debug.Log("Starting Game");
        camMove.ResetCamera();
        RedCount = 0;
        Debug.Log("count: " + BlueTeam.Count);
        BlueCount = 5;
        //foreach (GameObject p in GameObject.FindGameObjectsWithTag("Pawn").Where(x => x.GetComponent<Pawn>().team == Team.Red)) { RedPawns.Add(p); }
        //foreach (GameObject p in GameObject.FindGameObjectsWithTag("Pawn").Where(x => x.GetComponent<Pawn>().team == Team.Blue)) { BluePawns.Add(p); }
        state = State.Gameplay;
    }
    //Start game function

    //Change turn function
    public void ChangeTurn()
    {
        switch (state)
        {
            case (State.TroopPlacement): //Troop placing state
                //If reds turn, change to blue turn
                if (turn == Turn.Red)
                {
                    turn = Turn.Blue;
                }
                //if blues turn, change to red turn
                else if (turn == Turn.Blue)
                {
                    turn = Turn.Red;
                }
                break;

            case (State.Gameplay): //Gameplay state
                //if reds turn
                if (turn == Turn.Red)
                {
                    //if count matches number of elements in list
                    if (RedCount < (RedTeam.Count - 1))
                    {
                        RedCount++; //add one to count
                    }
                    //if count is same as or higher than number of elements in list
                    else if (RedCount >= (RedTeam.Count - 1))
                    {
                        RedCount = 0; //reset count
                    }

                    //If the phase is not set to movement
                    if (phase != Phase.Movement)
                    {
                        phase = Phase.Movement; //set it to movement
                    }

                    turn = Turn.Blue; //change turn to blue
                }
                //if blues turn
                else if (turn == Turn.Blue)
                {
                    //if count matches number of elements in list
                    if (BlueCount < (BlueTeam.Count - 1))
                    {
                        BlueCount++; //add one to count
                    }
                    //if count is same as or higher than number of elements in list
                    else if (BlueCount >= (BlueTeam.Count -1))
                    {
                        BlueCount = 0; //reset count
                    }

                    //If the phase is not set to movement
                    if (phase != Phase.Movement)
                    {
                        phase = Phase.Movement; //set it to movement 
                    }

                    turn = Turn.Red; //change turn to red
                }
                break;

            default:
                break;
        }
    }
    //Change turn function

    //Change phase function
    public void ChangePhase()
    {
        switch (phase)
        {
            case (Phase.Movement): //if phase = movement
                this.phase = Phase.Attack; //change to attack phase
                break;

            case (Phase.Attack): //if phase = attack
                //if attacking troop is selected
                if (attack.troopSelected == true)
                {
                    attack.SelectedTroop = null; //Deselect troop
                    attack.troopScript = null; //Deselect script
                    attack.troopSelected = false;
                    attack.enemyIndicator = false;
                }
                //if enemy troop is selected
                if(attack.enemySelectedTroop != null)
                {
                    attack.enemySelectedTroop = null; //Deselect enemy troop
                    attack.enemyTroopScript = null; //Deselect enemy script
                }

                CheckCount = 0; //reset checks count
                StartCoroutine(PerformChecks()); //call coroutine

                this.phase = Phase.Checks; //change phase to checks
                break;

            case (Phase.Checks): //if phase = checks
                UIManager.resetStats();
                camMove.ResetCamera(); //reset camera position
                break;

            default:

                break;
        }
    }
    //Change phase function

    //Performing the final turn checks
    public IEnumerator PerformChecks()
    {
        yield return null;

        GameObject priest;
        bool isEmpty;

        switch (turn)
        {
            case (Turn.Red): //if red turn
                switch (CheckCount)
                {
                    case (0): //priest revive check second
                        isEmpty = deadRedTeam.Count == 0; //is the dead team array empty
                        priest = SearchList(RedTeam, "Priest"); //search red tean for priest

                        //if priest is alive, his ability hasnt been activated and if dead team array is empty
                        if (priest != null && priestRedRevive == false && isEmpty == false)
                        {
                            //REMOVE Debug.Log("Do You want to revive any dead troops? Y/N"); //Replace with message on UI
                            UIManager.PriestQuestion(); //Ask if player wants to revive anyone
                            AwaitingResponse = true; //awaiting for response
                            yield return new WaitForSeconds(5f); //wait 5 seconds

                            //if the player doesn't response carry on without reviving
                            if (PauseChecks == false)
                            {
                                AwaitingResponse = false; //stop waiting for response
                                UIManager.HideQuestion(); //Hide question 
                                CheckCount++; //1+ to checks count
                                StartCoroutine(PerformChecks()); //recall the coroutine
                            }
                        }

                        //if conditions are not met
                        else
                        {
                            CheckCount++; //1+ to checks count
                            StartCoroutine(PerformChecks()); //recall the coroutine
                        }
                        break;

                    case (1): //pawns check first
                        if (RedKingAbilityCount >= 4) //every third turn
                        {
                            UIManager.SpawnPawnQuestion();
                            AwaitingResponse = true;
                            yield return new WaitForSeconds(5f);

                            //if the player doesn't response carry on without spawning troop
                            if (PauseChecks == false)
                            {
                                AwaitingResponse = false; //stop waiting for response
                                UIManager.HideQuestion(); //Hide question 
                                CheckCount++; //1+ to checks count
                                StartCoroutine(PerformChecks()); //recall the coroutine
                            }
                        }
                        else
                        {
                            RedKingAbilityCount++; //+1 to ability count
                            CheckCount++; //1+ to the checks count
                            StartCoroutine(PerformChecks()); //recall the coroutinte
                        }
                        break;

                    default: //all checks are finished
                        ChangePhase(); //call function
                        break;
                }
                break;

            case (Turn.Blue):
                switch (CheckCount)
                {
                    case (0):
                        Debug.Log(deadBlueTeam.Count);
                        isEmpty = deadBlueTeam.Count == 0; //is the dead team array empty
                        priest = SearchList(BlueTeam, "Priest"); //search blue team for priest

                        //if priest is alive, his ability hasnt been activated and if dead team array is empty
                        if (priest != null && priestBlueRevive == false && isEmpty == false)
                        {
                            //REMOVE Debug.Log("Do You want to revive any dead troops? Y/N"); //Replace with message on UI
                            UIManager.PriestQuestion(); //Ask if player wants to revive anyone
                            AwaitingResponse = true; //awaiting for response
                            yield return new WaitForSeconds(5f); //wait 5 seconds

                            //if the player doesn't response carry on without reviving
                            if (PauseChecks == false)
                            {
                                AwaitingResponse = false; //stop waiting for response
                                UIManager.HideQuestion(); //Hide question 
                                CheckCount++; //1+ to checks count
                                StartCoroutine(PerformChecks()); //recall the coroutine
                            }
                        }

                        //if conditions are not met
                        else
                        {
                            CheckCount++; //1+ to checks count
                            StartCoroutine(PerformChecks()); //recall the coroutine
                        }
                        break;

                    case (1): //pawns check first
                        if (BlueKingAbilityCount >= 4) //every third turn
                        {
                            UIManager.SpawnPawnQuestion();
                            AwaitingResponse = true;
                            yield return new WaitForSeconds(5f);

                            //if the player doesn't response carry on without spawning troop
                            if (PauseChecks == false)
                            {
                                AwaitingResponse = false; //stop waiting for response
                                UIManager.HideQuestion(); //Hide question 
                                CheckCount++; //1+ to checks count
                                StartCoroutine(PerformChecks()); //recall the coroutine
                            }
                        }
                        else
                        {
                            BlueKingAbilityCount++; //+1 to count
                            CheckCount++; //1+ to the checks count
                            StartCoroutine(PerformChecks()); //recall the coroutinte
                        }
                        break;

                    case (2):
                        ChangePhase();
                        break;

                    default: //all checks are finished
                        ChangePhase(); //call function
                        break;
                }
                break;

            default:
                break;
        }
    }
    //Performing the final turn checks

    //Search through list function
    public GameObject SearchList(List<GameObject> teamList, string name)
    {
        foreach (GameObject i in teamList) //search list loop
        {
            if (i.tag == name) //check if game object tag matches string
            {
                return i; //return game object
            }
        }

        return null; 
    }
    //Search through list function

    //Place troop function
    public void PlaceTroop()
    {
        //if in troop placement phase
        if (this.state == State.TroopPlacement)
        {
            AddTroopToList(); //add troop to the list
            drag.troopScript.troopState = TroopState.Alive; //change the troop state to alive
            UIManager.resetStats();
            camMove.ResetCamera(); //reset camera rotation
        }
        
        drag.troopScript.ReCallStartPos(); //reset the start x and y of the troop
        drag.Dragging = false; //turn off dragging
        drag.SelectedTroop = null; //deselect selected troop
        drag.troopScript = null; //deselect troopscript
        /* if doesnt break, delete this
        if (this.state == State.TroopPlacement) //idk why i have the checks option here, test and get rid of
        {
            UIManager.resetStats();
            camMove.ResetCamera(); //reset camera rotation
        }
        */
    }
    //Place troop function

    //Add troop to list function
    public void AddTroopToList()
    {
        switch (turn)
        {
            case (Turn.Red): //red turn
                RedTeam.Add(drag.SelectedTroop); //add to list
                break;

            case (Turn.Blue)://blue turn
                BlueTeam.Add(drag.SelectedTroop); //add to list
                i++; //i believe i dont need this, remove
                break;

            default:
                break;
        }
    }
    //Add troop to list function

    //Kill player function
    public void KillPlayer(GameObject DyingTroop, Team team)
    {
        //if the dying troop is a pawn
        if (DyingTroop.gameObject.tag == "Pawn")
        {
            switch (team)
            {
                case Team.Red: //dying troop in red team
                    RedPawns.Remove(DyingTroop);

                    //if theres more than one pawn in same team
                    if (RedPawnCount > 1)
                    {
                        var pawns = GameObject.FindGameObjectsWithTag("Pawn"); //store all pawns in game

                        //loop for each pawn in game
                        foreach (GameObject x in pawns)
                        {
                            var pawnScript = x.GetComponent<TroopScript>();

                            //if troop is alive, in the correct team and isnt the troop currently dying
                            if (pawnScript.team == Team.Red && pawnScript.troopState == TroopState.Alive && x != DyingTroop)
                            {
                                newpawn = x; //store pawn
                            }
                        }

                        arri = 0; 
                        //loop for each object in red team list
                        foreach (GameObject i in RedTeam)
                        {
                            //if we find a pawn gameobejct
                            if (i.tag == "Pawn")
                            {
                                RedTeam[arri] = newpawn; //replace its position in the list

                                var SearchPawn = SearchList(deadRedTeam, "Pawn");
                                if(SearchPawn == null)
                                {
                                    deadRedTeam.Add(DyingTroop);
                                }
                                else
                                {
                                    Destroy(DyingTroop);
                                }
                            }

                            arri++; 
                        }
                    }
                    //if theres one or less pawn in same team
                    else if (RedPawnCount <= 1)
                    {
                        RedTeam.Remove(DyingTroop); //just remove the troop from list
                    }
                    break;

                case Team.Blue: //dying troop in red team
                    BluePawns.Remove(DyingTroop);
                    //if theres more than one pawn in same team
                    if (BluePawnCount > 1)
                    {
                        var pawns = GameObject.FindGameObjectsWithTag("Pawn"); //store all pawns in game

                        //loop for each pawn in game
                        foreach (GameObject x in pawns)
                        {
                            var pawnScript = x.GetComponent<TroopScript>();

                            //if troop is alive, in the correct team and isnt the troop currently dying
                            if (pawnScript.team == Team.Blue && pawnScript.troopState == TroopState.Alive && x != DyingTroop)
                            {
                                newpawn = x; //store pawn
                            }
                        }

                        //loop for each object in blue team list
                        arri = 0;
                        foreach (GameObject i in BlueTeam)
                        {
                            //if we find a pawn gameobejct
                            if (i.tag == "Pawn")
                            {
                                BlueTeam[arri] = newpawn; //replace its position in the list
                                var SearchPawn = SearchList(deadBlueTeam, "Pawn");
                                if (SearchPawn == null)
                                {
                                    deadBlueTeam.Add(DyingTroop);
                                }
                                else
                                {
                                    Destroy(DyingTroop);
                                }
                            }
                            arri++; 
                        }
                    }

                    //if theres one or less pawn in same team
                    else if (BluePawnCount <= 1)
                    {
                        BlueTeam.Remove(DyingTroop); //just remove the troop from list
                    }

                    break;

                default:
                    break;
            }
        }

        else
        {
            switch (team)
            {
                case Team.Red:
                    if (RedCount == RedTeam.Count - 1)
                    {
                        if (RedTeam[RedCount] == DyingTroop) { RedCount = 0; }
                        else if (RedTeam[RedCount] != DyingTroop) { RedCount -= 1; }
                    }

                    else if (RedCount != RedTeam.Count - 1)
                    {
                        for (int i = 0; i < RedTeam.Count; i++)
                        {
                            if (RedTeam[i].gameObject.tag == DyingTroop.gameObject.tag) { id = i; }
                        }

                        if (id >= RedCount) { }
                        else if (id < RedCount) { RedCount -= 1; }
                    }

                    RedTeam.Remove(DyingTroop); //remove troop from lis
                    deadRedTeam.Add(DyingTroop);
                    break;

                case Team.Blue:
                    if (BlueCount == BlueTeam.Count - 1)
                    {
                        if (BlueTeam[BlueCount] == DyingTroop) { BlueCount = 0; Debug.Log("2.1"); }
                        else if (BlueTeam[BlueCount] != DyingTroop) { BlueCount -= 1; Debug.Log("2.2"); }
                    }

                    else if (BlueCount != BlueTeam.Count - 1)
                    { 
                        for (int i = 0; i < BlueTeam.Count; i++)
                        {
                            if(BlueTeam[i].gameObject.tag == DyingTroop.gameObject.tag) { id = i; }
                        }

                        if (id >= BlueCount) { }
                        else if (id < BlueCount) { BlueCount -= 1; }
                    }

                    BlueTeam.Remove(DyingTroop); //remove troop from list
                    deadBlueTeam.Add(DyingTroop);
                    break;

                default:
                    break;
            }
        }

        var MoveTroop = new Vector3(DyingTroop.transform.position.x, -5, DyingTroop.transform.position.x);
        DyingTroop.transform.position = MoveTroop;
    }
    //Kill player function

    //Choose Revive player
    public void ReviveSelectedTroop()
    {
        if (turn == Turn.Red)
        {
            chosenTroop = SearchList(deadRedTeam, ReviveTroop.ToString());
            if (chosenTroop != null)
            {
                Debug.Log(ReviveTroop.ToString());
                drag.ReviveTroop(chosenTroop);
                UIManager.HideDeadTroops();
            }
        }
        else if (turn == Turn.Blue)
        {
            chosenTroop = SearchList(deadBlueTeam, ReviveTroop.ToString());
            if (chosenTroop != null)
            {
                Debug.Log(ReviveTroop.ToString());
                drag.ReviveTroop(chosenTroop);
                UIManager.HideDeadTroops();
            }
        }
    }
    //Choose Revive player

    IEnumerator ResetDeadPawn()
    {
        resetPawn = false;
        if (BluePawnCount >= 3 && deadBlueTeam.Count > 0)
        {
            for (int i = 0; i < deadBlueTeam.Count; i++)
            {
                Debug.Log("hi");
                if (deadBlueTeam[i].tag == "Pawn")
                {
                    Debug.Log("hi");
                    deadBlueTeam.Remove(deadBlueTeam[i]);
                }
            }
        }

        if (RedPawnCount >= 3 && deadRedTeam.Count > 0)
        {
            for (int i = 0; i < deadRedTeam.Count; i++)
            {
                if(deadRedTeam[i].tag == "Pawn")
                {
                    deadRedTeam.Remove(deadRedTeam[i]);
                }
            }
        }

        yield return new WaitForSeconds(1f);
        resetPawn = true;
    }

    public void ToggleEffects()
    {
        if (effectsToggled == false)
        {
            effects.SetActive(true);
            var light = GameObject.FindGameObjectWithTag("Light");
            light.GetComponent<Light>().intensity = 0.3f;
            effectsToggled = true;
        }
        else if (effectsToggled == true)
        {
            effects.SetActive(false);
            var light = GameObject.FindGameObjectWithTag("Light");
            light.GetComponent<Light>().intensity = 1f;
            effectsToggled = false;
        }
    }
}

public enum State
{
    TroopPlacement, Gameplay
};

public enum Phase
{
    Movement, Attack, Checks
};

public enum Turn
{
    Red, Blue
};

public enum revive
{
    Archer, Knight, Mage, Pawn, Tank
};
