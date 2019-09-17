using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour {

    bool initialize = false;

    GameManager gameManager;
    UIController UIManager;
    TroopAnimController animController;
    public TroopScript troopScript;
    Board board;

    public RaycastHit TroopHit;
    Ray TroopRay;
    RaycastHit TileHit;
    Ray TileRay;
    Vector3 moveTo;
    Vector3 storedPos;
    Color mageColor;

    public bool Dragging = false;

    int RedTroopMask = 1 << 9;
    int BlueTroopMask = 1 << 10;
    int TileMask = 1 << 11;

    public GameObject SelectedTile;
    public GameObject SelectedTroop;
    int startX;
    int startY;
    int tileX;
    int tileY;
    public int difX;
    public int difY;
    public int dif;
    public int pawnCount = 1;
    public int pawnMoveCount = 1;
    
    //public bool changePhase = false;

    public bool kingAbility = false;
    public bool spawningTroop = false;

    bool awaitingResponse = false;
    bool TroopCheck = false;

    public GameObject pawnOne;
    public GameObject pawnTwo;
    GameObject pawnThree;
    public int pawnSpawnCount = 0;

    Vector3 PawnPos;
    public GameObject spawnedRedPawn;
    public GameObject spawnedBluePawn;

    public Material redMageFullOpac;
    public Material redMageHalfOpac;
    public Material blueMageFullOpac;
    public Material blueMageHalfOpac;
    bool mageFullOpac;
    bool notChanged;

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
            board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();

            initialize = true;
        }

        if (gameManager.phase == Phase.Movement)
        {
            //if troop not already selected 
            if (Dragging == false && SelectedTroop == null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    difX = 0;
                    difY = 0;

                    SelectTroop();
                }
            }

            //if troop already selected
            else if (Dragging == true)
            {
                //left mouse click
                if (Input.GetMouseButtonDown(0))
                {
                    switch (gameManager.state)
                    {
                        case (State.TroopPlacement): //in the troop placement state
                            //if troop is in the range of the board
                            if ((SelectedTroop.transform.position.x >= board.Minx && SelectedTroop.transform.position.z >= board.Miny) && (SelectedTroop.transform.position.x <= board.Maxx && SelectedTroop.transform.position.z <= board.Maxy))
                            {
                                //if the troop is a pawn
                                if (SelectedTroop.tag == "Pawn")
                                {
                                    PlacePawn(); //call the place pawn function
                                }
                                //if troop is anything else  
                                else
                                {
                                    gameManager.PlaceTroop(); //call the place troop function
                                }
                            }
                            break;

                        case (State.Gameplay): //in the gameplay state
                            //if player has moved troops position
                            if (difX != 0 || difY != 0)
                            {
                                //if troop is a pawn
                                if (SelectedTroop.tag == "Pawn")
                                {
                                    //Maybe i can shorten this...
                                    
                                    //if red team turn and not all the pawns have been placed 
                                    if (gameManager.turn == Turn.Red && pawnMoveCount < gameManager.RedPawnCount)
                                    {
                                        SelectedTroop.GetComponent<Pawn>().moved = true;
                                        gameManager.PlaceTroop();
                                        pawnMoveCount++; //increase pawn moved count
                                    }
                                    //if red team turn and all the pawns have been placed
                                    else if (gameManager.turn == Turn.Red && pawnMoveCount == gameManager.RedPawnCount)
                                    {
                                        gameManager.PlaceTroop();
                                        for(int i = 0; i < gameManager.RedPawns.Count; i++)
                                        {
                                            gameManager.RedPawns[i].GetComponent<Pawn>().moved = false;
                                        }
                                        pawnMoveCount = 1; //reset count
                                        gameManager.ChangePhase();
                                    }
                                    //if blue team turn and not all the pawns have been placed 
                                    if (gameManager.turn == Turn.Blue && pawnMoveCount < gameManager.BluePawnCount)
                                    {
                                        SelectedTroop.GetComponent<Pawn>().moved = true;
                                        gameManager.PlaceTroop();
                                        pawnMoveCount++; //increase pawn moved count
                                    }
                                    //if blue team turn and all the pawns have been placed
                                    else if (gameManager.turn == Turn.Blue && pawnMoveCount == gameManager.BluePawnCount)
                                    {
                                        gameManager.PlaceTroop();
                                        for (int i = 0; i < gameManager.BluePawns.Count; i++)
                                        {
                                            gameManager.BluePawns[i].GetComponent<Pawn>().moved = false;
                                        }
                                        pawnMoveCount = 1; //reset count
                                        gameManager.ChangePhase();
                                    }
                                    
                                    //...Maybe i can shorten this
                                }
                                //if troop is a mage
                                else if (SelectedTroop.tag == "Mage" && dif > troopScript.moveDist)
                                {
                                    //if tile is out of his movement range
                                    if (dif > troopScript.moveDist)
                                    {
                                        awaitingResponse = true; //wait for players response
                                        Dragging = false; //stop moving the mage whilst waiting
                                    }
                                    //if tile is in his movement range
                                    else
                                    {
                                        //palce troop as normal and change phase
                                        gameManager.PlaceTroop();
                                        gameManager.ChangePhase();
                                    }
                                }
                                //if troop is anything else
                                else
                                {
                                    //place troop and change phase
                                    gameManager.PlaceTroop();
                                    gameManager.ChangePhase();
                                }
                            }

                            //if player hasn't moved troops position
                            else if (difX == 0 && difY == 0)
                            {
                                //place troop but dont chance phase
                                gameManager.PlaceTroop();
                            }
                            break;

                        default:

                            break;
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    SelectedTroop.transform.position = storedPos;
                    Dragging = false;
                    SelectedTroop = null;
                    troopScript = null;
                    Debug.Log("hi");
                }

                if(SelectedTroop != null)
                {
                    if (SelectedTroop.tag == "Mage")
                    {
                        SkinnedMeshRenderer renderer = SelectedTroop.GetComponentInChildren<SkinnedMeshRenderer>();
                        if (troopScript.team == Team.Red && mageFullOpac == true && renderer != redMageFullOpac)
                        {
                            notChanged = true;
                        }
                        else if (troopScript.team == Team.Red && mageFullOpac == false && renderer != redMageHalfOpac)
                        {
                            notChanged = true;
                        }
                        else if (troopScript.team == Team.Blue && mageFullOpac == true && renderer != blueMageFullOpac)
                        {
                            notChanged = true;
                        }
                        else if (troopScript.team == Team.Blue && mageFullOpac == false && renderer != blueMageHalfOpac)
                        {
                            notChanged = true;
                        }
                        else
                        {
                            notChanged = false;
                        }
                    }
                }
            }

            if (awaitingResponse == true)
            {
                //rewrite debug log to ask question in UI
                //Debug.Log("About to use mages ability, do you want to continue? Y/N");
                UIManager.MageQuestion();

                if (UIManager.YesNo == yesno.yes) //if player presses Y key activate mage ability
                {
                    mageFullOpac = true;
                    ChangeMageOpac();

                    //Move mage and go to checks 
                    //TroopHit.transform.position = moveTo;
                    gameManager.PlaceTroop();
                    gameManager.ChangePhase();
                    gameManager.ChangePhase();
                    awaitingResponse = false; //stop calling this
                    UIManager.HideQuestion();
                    UIManager.YesNo = yesno.notSelected;
                }
                else if (UIManager.YesNo == yesno.no) //if player presses N key
                {
                    //nothing happens, player chooses new tile for mage
                    Dragging = true;
                    awaitingResponse = false; //stop calling this
                    UIManager.HideQuestion();
                    UIManager.YesNo = yesno.notSelected;
                }
            }
        }

        else if (gameManager.phase == Phase.Checks)
        {
            if(Dragging == true)
            {
                //this switch method will allow me to easily add new abilities and checks
                switch (gameManager.CheckCount)
                {
                    case (0): //if priest ability

                        if (Input.GetMouseButtonDown(0))
                        {
                            //Debug.Log("Test1");
                            if(SelectedTroop.tag == "Pawn")
                            {
                                //set it so it spawns multiple pawns until there are 3 on the board
                                if (gameManager.turn == Turn.Red && gameManager.RedPawnCount < 2)
                                {
                                    GameObject RedPawn = Instantiate(gameManager.RedPawnPrefab, new Vector3(moveTo.x, 0.1f, moveTo.z), new Quaternion(0, 0, 0, 0)); //instantiate new gameobject, rotate to face red team

                                    if (pawnSpawnCount == 0) { pawnOne = RedPawn; }
                                    else if (pawnSpawnCount == 1) { pawnTwo = RedPawn; }
                                    //else if (pawnSpawnCount == 2) { pawnThree = RedPawn; }
                                    pawnSpawnCount++;

                                    RedPawn.transform.SetParent(GameObject.FindGameObjectWithTag("RedTeam").transform);//set its parent object to its team
                                    RedPawn.name = "RedPawn";//rename pawn
                                    RedPawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                    RedPawn.GetComponent<TroopScript>().Anim = RedPawn.GetComponent<Animator>();
                                    StartCoroutine(RedPawn.GetComponent<TroopScript>().PlayReviveAnim());
                                }
                                else if (gameManager.turn == Turn.Red && gameManager.RedPawnCount <= 2)
                                {
                                    gameManager.priestRedRevive = true;

                                    SelectedTroop.GetComponent<TroopScript>().ResetHealth();
                                    SelectedTroop.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                    StartCoroutine(animController.ReviveAnim(gameManager, SelectedTroop));
                                    gameManager.PlaceTroop();

                                    gameManager.PauseChecks = false;
                                    gameManager.CheckCount++;
                                    spawningTroop = false;
                                    StartCoroutine(gameManager.PerformChecks());
                                }
                                else if (gameManager.turn == Turn.Blue && gameManager.BluePawnCount < 2)
                                {
                                    GameObject BluePawn = Instantiate(gameManager.BluePawnPrefab, new Vector3(moveTo.x, 0.1f, moveTo.z), new Quaternion(0, 1, 0, 0)); //instantiate new gameobject, rotate to face red team

                                    if(pawnSpawnCount == 0) { pawnOne = BluePawn; }
                                    else if (pawnSpawnCount == 1) { pawnTwo = BluePawn; }
                                    //else if (pawnSpawnCount == 2) { pawnThree = BluePawn; }
                                    pawnSpawnCount++; //Debug.Log("Plus one in spawn count");

                                    BluePawn.transform.SetParent(GameObject.FindGameObjectWithTag("BlueTeam").transform);//set its parent object to its team
                                    BluePawn.name = "BluePawn";//rename pawn
                                    BluePawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                    BluePawn.GetComponent<TroopScript>().Anim = BluePawn.GetComponent<Animator>();
                                    StartCoroutine(BluePawn.GetComponent<TroopScript>().PlayReviveAnim());
                                }
                                else if (gameManager.turn == Turn.Blue && gameManager.BluePawnCount <= 2)
                                {
                                    Debug.Log("Test2");
                                    gameManager.priestBlueRevive = true;

                                    SelectedTroop.GetComponent<TroopScript>().ResetHealth();
                                    SelectedTroop.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                    StartCoroutine(animController.ReviveAnim(gameManager, SelectedTroop));
                                    gameManager.PlaceTroop();

                                    gameManager.PauseChecks = false;
                                    gameManager.CheckCount++;
                                    spawningTroop = false;
                                    StartCoroutine(gameManager.PerformChecks());
                                }
                            }
                            else
                            {
                                Debug.Log("Test3");
                                SelectedTroop.GetComponent<TroopScript>().ResetHealth();
                                SelectedTroop.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                StartCoroutine(animController.ReviveAnim(gameManager, SelectedTroop));
                                gameManager.PlaceTroop();
                                spawningTroop = false;

                                if (gameManager.turn == Turn.Red)
                                {
                                    gameManager.priestRedRevive = true;
                                }
                                else if (gameManager.turn == Turn.Blue)
                                {
                                    gameManager.priestBlueRevive = true;
                                }
                            }
                        }

                        if (Input.GetMouseButtonDown(1))
                        {
                            Debug.Log("hi");
                            if(SelectedTroop.tag == "Pawn" && pawnSpawnCount >= 1)
                            {
                                //if (pawnSpawnCount == 1) { Destroy (pawnOne); }
                                //else if (pawnSpawnCount == 2) { Destroy(pawnOne); Destroy(pawnTwo); }
                                if(pawnOne != null)
                                {
                                    Destroy(pawnOne);
                                }
                                //else if (pawnSpawnCount == 3) { Destroy(pawnOne); Destroy(pawnTwo); Destroy(pawnThree); }
                            }

                            Dragging = false;

                            var MoveTroop = new Vector3(SelectedTroop.transform.position.x, -5, SelectedTroop.transform.position.x);
                            SelectedTroop.transform.position = MoveTroop;
                            spawningTroop = false;
                            SelectedTroop = null;
                            troopScript = null;

                            UIManager.RevealDeadTroops();
                        }
                        break;

                    default:
                        break;
                }
            }

            if (kingAbility == true)
            {
                switch (gameManager.turn)
                {
                    case (Turn.Red):
                        if (spawnedRedPawn.transform.position == moveTo)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                PlacePawn();
                            }
                        }
                        break;

                    case (Turn.Blue):
                        if (spawnedBluePawn.transform.position == moveTo)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                PlacePawn();
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //if player is dragging
        if (Dragging == true)
        {
            //cast a ray to mouses position on screen
            TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if ray hits a tile
            if (Physics.Raycast(TileRay, out TileHit, 100, TileMask))
            {
                switch (gameManager.state)
                {
                    case State.TroopPlacement: //troop placement state
                        //change below line to make the troop hover
                        moveTo = new Vector3(TileHit.transform.position.x, 0.1f, TileHit.transform.position.z); //declare vector3 based on tile ray hits

                        TroopCheck = CheckTroopOnTile(); //Check if troop is already on tile, return bool

                        //if troop not on tile
                        if (TroopCheck == false)
                        {
                            switch (gameManager.turn)
                            {
                                case (Turn.Red): //red team turn
                                    //if selected tile is within the red teams troop placement range
                                    if (moveTo.z >= 0 && moveTo.z <= 2)
                                    {
                                        SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
                                    }
                                    break;

                                case (Turn.Blue): //blue team turn
                                    //if selected tile is within the blue teams troop placement range
                                    if (moveTo.z >= 7 && moveTo.z <= 9)
                                    {
                                        SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;

                    case State.Gameplay: //gameplay state
                        if(gameManager.phase == Phase.Movement)
                        {
                            //change below line to make the troop hover
                            moveTo = new Vector3(TileHit.transform.position.x, 0.1f, TileHit.transform.position.z); //declare vector3 based on tile ray hits

                            //Delcare start pos variables
                            startX = Mathf.RoundToInt(troopScript.Startx);
                            startY = Mathf.RoundToInt(troopScript.Starty);

                            //Constantly update current pos variables
                            tileX = Mathf.RoundToInt(TileHit.transform.position.x);
                            tileY = Mathf.RoundToInt(TileHit.transform.position.z);

                            //Calculate difference between two variables
                            difX = startX - tileX;
                            difY = startY - tileY;

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

                            dif = difX + difY; //calculate total difference

                            if (SelectedTroop.tag == "Mage")
                            {
                                if (dif <= troopScript.moveDist)
                                {
                                    SkinnedMeshRenderer[] renderer = new SkinnedMeshRenderer[5];
                                    renderer = SelectedTroop.GetComponentsInChildren<SkinnedMeshRenderer>();

                                    mageFullOpac = true;

                                    if(notChanged == true)
                                    {
                                        ChangeMageOpac();
                                    }

                                    TroopCheck = CheckTroopOnTile(); //Check if troop is already on tile, return bool

                                    //if troop not on tile
                                    if (TroopCheck == false)
                                    {
                                        SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
                                    }
                                }
                                else
                                {
                                    SkinnedMeshRenderer[] renderer = new SkinnedMeshRenderer[5]; 
                                    renderer = SelectedTroop.GetComponentsInChildren<SkinnedMeshRenderer>();

                                    mageFullOpac = false;

                                    if (notChanged == true)
                                    {
                                        ChangeMageOpac();
                                    }

                                    TroopCheck = CheckTroopOnTile();

                                    //if troop not on tile
                                    if (TroopCheck == false)
                                    {
                                        SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
                                    }
                                }
                            }

                            else if (dif <= troopScript.moveDist)
                            {
                                TroopCheck = CheckTroopOnTile(); //Check if troop is already on tile, return bool

                                //if troop not on tile
                                if (TroopCheck == false)
                                {
                                    SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
                                }
                            }
                        }
                       
                        else if(gameManager.phase == Phase.Checks)
                        {
                            //change below line to make the troop hover
                            moveTo = new Vector3(TileHit.transform.position.x, 0.1f, TileHit.transform.position.z); //declare vector3 based on tile ray hits

                            TroopCheck = CheckTroopOnTile(); //Check if troop is already on tile, return bool

                            //if troop not on tile
                            if (TroopCheck == false)
                            {
                                switch (gameManager.turn)
                                {
                                    case (Turn.Red): //red team turn
                                                     //if selected tile is within the red teams troop placement range
                                        if (moveTo.z >= 0 && moveTo.z <= 2)
                                        {
                                            SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile

                                        }
                                        break;

                                    case (Turn.Blue): //blue team turn
                                                      //if selected tile is within the blue teams troop placement range
                                        if (moveTo.z >= 7 && moveTo.z <= 9)
                                        {
                                            SelectedTroop.transform.position = moveTo; //move the selected troop to hover over tile
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
        }

        //if pawn ability has been activated
        if (kingAbility == true)
        {
            //cast a ray to mouses position on screen
            TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if the ray hit a tile
            if (Physics.Raycast(TileRay, out TileHit, 100, TileMask))
            {
                moveTo = new Vector3(TileHit.transform.position.x, 0.1f, TileHit.transform.position.z); //set tiles position

                TroopCheck = CheckTroopOnTile(); //check if troop on tile

                //if troop not on tile
                if (TroopCheck == false)
                {
                    switch (gameManager.turn)
                    {
                        case (Turn.Red): //red team turn
                                         //if selected tile is within the red teams troop placement range
                            if (moveTo.z >= 0 && moveTo.z <= 2)
                            {
                                spawnedRedPawn.transform.position = moveTo;
                            }
                            break;

                        case (Turn.Blue): //blue team turn
                                          //if selected tile is within the blue teams troop placement range
                            if (moveTo.z >= 7 && moveTo.z <= 9)
                            {
                                spawnedBluePawn.transform.position = moveTo;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }

    //Select troop function
    void SelectTroop()
    {
        //cast a ray to mouses position on screen
        TroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch (gameManager.turn)
        {
            case (Turn.Red): //red team turn
                             //if ray hits a red troop
                if (Physics.Raycast(TroopRay, out TroopHit, 100, RedTroopMask))
                {
                    //Declare troop gameobject and script
                    SelectedTroop = TroopHit.transform.gameObject;
                    troopScript = SelectedTroop.GetComponent<TroopScript>();

                    storedPos = SelectedTroop.transform.position;

                    //if player can drag selected troop
                    if (troopScript.canDrag == true && SelectedTroop.tag == "Pawn")
                    {
                        //if troop placement state and the troop is pawn, set count to 1
                        if (gameManager.state == State.TroopPlacement)
                        {
                            pawnCount = 1;
                        }

                        if (SelectedTroop.GetComponent<Pawn>().moved == false)
                        {
                            Dragging = true;//enable dragging
                        }

                        else if (SelectedTroop.GetComponent<Pawn>().moved == true)
                        {
                            SelectedTroop = null; //deselect troop
                            troopScript = null; //deselect troop
                        }
                    }

                    else if(gameManager.state == State.TroopPlacement && SelectedTroop.tag == "King")
                    {
                        SelectedTroop = null; //deselect troop
                        troopScript = null; //deselect troop
                    }

                    else if (troopScript.canDrag == true && SelectedTroop.tag != "Pawn")
                    {
                        Dragging = true;//enable dragging
                    }

                    //if player can not drag selected troop
                    else if (troopScript.canDrag == false)
                    {
                        SelectedTroop = null; //deselect troop
                        troopScript = null; //deselect troop
                    }
                }
                break;

            case (Turn.Blue): //blue team turn
                              //if ray hits a blue troop
                if (Physics.Raycast(TroopRay, out TroopHit, 100, BlueTroopMask))
                {
                    //Debug.Log(TroopHit.transform.gameObject);
                    //Declare troop gameobject and script
                    SelectedTroop = TroopHit.transform.gameObject;
                    troopScript = SelectedTroop.GetComponent<TroopScript>();

                    storedPos = SelectedTroop.transform.position;

                    //if player can drag selected troop
                    if (troopScript.canDrag == true && SelectedTroop.tag == "Pawn")
                    {
                        //if troop placement state and the troop is pawn, set count to 1
                        if (gameManager.state == State.TroopPlacement && SelectedTroop.tag == "Pawn")
                        {
                            pawnCount = 1;
                        }

                        if (SelectedTroop.GetComponent<Pawn>().moved == false)
                        {
                            Dragging = true;//enable dragging
                        }
                        else if (SelectedTroop.GetComponent<Pawn>().moved == true)
                        {
                            SelectedTroop = null; //deselect troop
                            troopScript = null; //deselect troop
                        }
                    }

                    else if (gameManager.state == State.TroopPlacement && SelectedTroop.tag == "King")
                    {
                        SelectedTroop = null; //deselect troop
                        troopScript = null; //deselect troop
                    }

                    else if (troopScript.canDrag == true && SelectedTroop.tag != "Pawn")
                    {
                        Dragging = true;//enable dragging
                    }

                    //if player can not drag selected troop
                    else if (troopScript.canDrag == false)
                    {
                        SelectedTroop = null; //deselect troop
                        troopScript = null; //deselect troop
                    }
                }
                break;

            default:
                break;
        }
    }
    //Select troop function

    //Check if troop is on tile function
    bool CheckTroopOnTile()
    {
        SelectedTile = TileHit.transform.gameObject; //get tile gameobject

        //Get script and retrieve troop on tile variable; and IF true
        if (SelectedTile.GetComponent<Tile>().troopOnTile == true) 
        {
            return true; //return true
        }
        //if retrievd variable is false
        else
        {
            return false; //return false
        }
    }
    //Check if troop is on tile function

    //Place pawn function
    public void PlacePawn()
    {
        //if troop placement state
        if (gameManager.state == State.TroopPlacement) 
        {
            switch (gameManager.turn)
            {
                case (Turn.Red): //red team turn
                    //if pawn count is less than 3
                    if (pawnCount < 3)
                    {
                        //if selected tile is within the red teams troop placement range
                        if (moveTo.z >= 0 && moveTo.z <= 2)
                        {
                            if(PawnPos != moveTo)
                            {
                                //Spawn pawn
                                GameObject RedPawn = Instantiate(gameManager.RedPawnPrefab, new Vector3(moveTo.x, 0.1f, moveTo.z), new Quaternion()); //instantiate new gameobject
                                RedPawn.transform.SetParent(GameObject.FindGameObjectWithTag("RedTeam").transform); //set its parent object to its team
                                RedPawn.name = "RedPawn" + gameManager.RedPawnCount; //rename pawn
                                RedPawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                PawnPos = moveTo;
                                gameManager.RedPawns.Add(RedPawn);
                                pawnCount++; //increase pawn count
                            }
                            
                        }
                    }
                    //if pawn count is 3
                    else if (pawnCount == 3)
                    {
                        //PawnPos = new Vector3(0,0,0);
                        gameManager.RedPawns.Add(SelectedTroop);
                        gameManager.PlaceTroop(); //place the troop
                    }

                    
                    break;

                case (Turn.Blue): //blue team turn
                    //if pawn count is less than 3
                    if (pawnCount < 3)
                    {
                        //if selected tile is within the red teams troop placement range
                        if (moveTo.z >= 7 && moveTo.z <= 9)
                        {
                            if(PawnPos != moveTo)
                            {
                                //Spawn pawn
                                GameObject BluePawn = Instantiate(gameManager.BluePawnPrefab, new Vector3(moveTo.x, 0.1f, moveTo.z), new Quaternion(0, 1, 0, 0)); //instantiate new gameobject, rotate to face red team
                                BluePawn.transform.SetParent(GameObject.FindGameObjectWithTag("BlueTeam").transform);//set its parent object to its team
                                BluePawn.name = "BluePawn" + gameManager.BluePawnCount;//rename pawn
                                BluePawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                                PawnPos = moveTo;
                                gameManager.BluePawns.Add(BluePawn);
                                pawnCount++; //increase pawn count
                            }
                        }
                    }
                    //if pawn count is 3
                    else if (pawnCount == 3)
                    {
                        gameManager.BluePawns.Add(SelectedTroop);
                        gameManager.PlaceTroop(); //place the troop
                    }
                    break;

                default:
                    break;
            }
        }

        //if pawn ability has been activated
        if (gameManager.state == State.Gameplay && kingAbility == true) 
        {
            switch (gameManager.turn)
            {
                case (Turn.Red): //red team turn
                    //Spawn pawn
                    //GameObject RedPawn = Instantiate(gameManager.RedPawnPrefab, moveTo, new Quaternion()); //instantiate new gameobject
                    spawnedRedPawn.transform.SetParent(GameObject.FindGameObjectWithTag("RedTeam").transform); //set its parent object to its team
                    spawnedRedPawn.name = "RedPawn" + gameManager.RedPawnCount; //rename pawn
                    spawnedRedPawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                    spawnedRedPawn.GetComponent<TroopScript>().ReCallStartPos();
                    gameManager.RedPawns.Add(spawnedRedPawn);
                    StartCoroutine(animController.ReviveAnim(gameManager, spawnedRedPawn));
                    spawnedRedPawn = null;
                    kingAbility = false; //turn off ability
                    break;

                case (Turn.Blue): //blue team turn
                    //Spawn pawn
                    //GameObject BluePawn = Instantiate(gameManager.BluePawnPrefab, moveTo, new Quaternion(0, 1, 0, 0)); //instantiate new gameobject, rotate to face red team
                    spawnedBluePawn.transform.SetParent(GameObject.FindGameObjectWithTag("BlueTeam").transform);//set its parent object to its team
                    spawnedBluePawn.name = "BluePawn" + gameManager.BluePawnCount;//rename pawn
                    spawnedBluePawn.GetComponent<TroopScript>().troopState = TroopState.Alive;
                    spawnedBluePawn.GetComponent<TroopScript>().ReCallStartPos();
                    gameManager.BluePawns.Add(spawnedBluePawn);
                    StartCoroutine(animController.ReviveAnim(gameManager, spawnedBluePawn));
                    spawnedBluePawn = null;
                    kingAbility = false; //turn off ability
                    break;

                default:
                    break;
            }
        }
    }
    //Place pawn function

    //Revive
    public void ReviveTroop(GameObject chosenTroop)
    {
        pawnSpawnCount = 0;
        SelectedTroop = chosenTroop;
        troopScript = SelectedTroop.GetComponent<TroopScript>();
        Dragging = true;
        spawningTroop = true;
        //UIManager.reviveUI = false;
        if(SelectedTroop.tag == "Pawn")
        {
            if (gameManager.turn == Turn.Red) { gameManager.RedPawns.Add(SelectedTroop); }
            else if (gameManager.turn == Turn.Blue) { gameManager.BluePawns.Add(SelectedTroop); }
        }
        Debug.Log("ReviveTroopFunction");
    }
    //Revive

    void ChangeMageOpac()
    {
        SkinnedMeshRenderer[] renderer = new SkinnedMeshRenderer[5];
        renderer = SelectedTroop.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (mageFullOpac == true)
        {
            for (int i = 0; i < 5; i++)
            {
                if (troopScript.team == Team.Red)
                {
                    renderer[i].material = redMageFullOpac;
                }
                else if (troopScript.team == Team.Blue)
                {
                    renderer[i].material = blueMageFullOpac;
                }
            }
        }

        if (mageFullOpac == false)
        {
            for (int i = 0; i < 5; i++)
            {
                if (troopScript.team == Team.Red)
                {
                    renderer[i].material = redMageHalfOpac;
                }
                else if (troopScript.team == Team.Blue)
                {
                    renderer[i].material = blueMageHalfOpac;
                }
            }
        }
    }
}

enum MageColour { full, half };