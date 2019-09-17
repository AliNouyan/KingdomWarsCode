using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {

    bool initialize = false;

    GameManager gameManager;
    DragAndDrop drag;

    public GameObject YesButton;
    public GameObject NoButton;
    public GameObject QuestionBox;
    public GameObject revivebox;
    public Text Question;
    public Texture redQbox;
    public Texture blueQBox;

    public Button yesButton;
    public Button noButton;
    public yesno YesNo;
    //calledFunction CalledFunction;

    bool awaitingResponse = false;
    public bool reviveUI = false;

    public GameObject redLabel;
    public GameObject blueLabel;
    public Text stats;

    public GameObject currentTroop;

    public Sprite redKing;
    public Sprite redPawn;
    public Sprite redKnight;
    public Sprite redArcher;
    public Sprite redTank;
    public Sprite redPriest;
    public Sprite redMage;

    public Sprite blueKing;
    public Sprite bluePawn;
    public Sprite blueKnight;
    public Sprite blueArcher;
    public Sprite blueTank;
    public Sprite bluePriest;
    public Sprite blueMage;

    public GameObject phase;
    public Sprite movement;
    public Sprite attack;
    public GameObject skipPhase;

    public GameObject redRevive;
    public GameObject blueRevive;
    public Button reviveRedMage;
    public Button reviveRedArcher;
    public Button reviveRedTank;
    public Button reviveRedPawn;
    public Button reviveRedKnight;
    public Button reviveBlueMage;
    public Button reviveBlueArcher;
    public Button reviveBlueTank;
    public Button reviveBluePawn;
    public Button reviveBlueKnight;
    Button[] _redRevive = new Button[5];
    Button[] _blueRevive = new Button[5];

    bool gameOver;

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update () {
        if(initialize == false)
        {
            gameManager = GetComponent<GameManager>();
            drag = GetComponent<DragAndDrop>();

            HideQuestion();
            YesNo = yesno.notSelected;
            //CalledFunction = calledFunction.none;
            yesButton.onClick.AddListener(yesClick);
            noButton.onClick.AddListener(noClick);
            resetStats();
            _redRevive[0] = reviveRedMage;
            _redRevive[1] = reviveRedArcher;
            _redRevive[2] = reviveRedTank;
            _redRevive[3] = reviveRedPawn;
            _redRevive[4] = reviveRedKnight;
            _blueRevive[0] = reviveBlueMage;
            _blueRevive[1] = reviveBlueArcher;
            _blueRevive[2] = reviveBlueTank;
            _blueRevive[3] = reviveBluePawn;
            _blueRevive[4] = reviveBlueKnight;

            initialize = true;
        }

        if(reviveUI == true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("hi");
                gameManager.AwaitingResponse = false;
                gameManager.CheckCount++;
                StartCoroutine(gameManager.PerformChecks());
                YesNo = yesno.notSelected;
                HideDeadTroops();
            }
        }
        if(gameManager.turn == Turn.Red)
        {
            redLabel.SetActive(true);
            blueLabel.SetActive(false);

            if(gameManager.state == State.TroopPlacement && drag.troopScript != null)
            {
                stats.text = (drag.troopScript.Health + "\n" + drag.troopScript.attackDamage + "\n" + drag.troopScript.attackDist + "\n" + drag.troopScript.moveDist);

                if (drag.troopScript.gameObject.tag == "King")
                {
                    currentTroop.GetComponent<Image>().sprite = redKing;
                }
                else if (drag.troopScript.gameObject.tag == "Pawn")
                {
                    currentTroop.GetComponent<Image>().sprite = redPawn;
                }
                else if (drag.troopScript.gameObject.tag == "Knight")
                {
                    currentTroop.GetComponent<Image>().sprite = redKnight;
                }
                else if (drag.troopScript.gameObject.tag == "Archer")
                {
                    currentTroop.GetComponent<Image>().sprite = redArcher;
                }
                else if (drag.troopScript.gameObject.tag == "Mage")
                {
                    currentTroop.GetComponent<Image>().sprite = redMage;
                }
                else if (drag.troopScript.gameObject.tag == "Tank")
                {
                    currentTroop.GetComponent<Image>().sprite = redTank;
                }
                else if (drag.troopScript.gameObject.tag == "Priest")
                {
                    currentTroop.GetComponent<Image>().sprite = redPriest;
                }
            }

            else if(gameManager.state == State.Gameplay && gameManager.troopScript != null)
            {
                stats.text = (gameManager.troopScript.Health + "\n" + gameManager.troopScript.attackDamage + "\n" + gameManager.troopScript.attackDist + "\n" + gameManager.troopScript.moveDist);

                if(gameManager.troopScript.gameObject.tag == "King")
                {
                    currentTroop.GetComponent<Image>().sprite = redKing;
                }
                else if (gameManager.troopScript.gameObject.tag == "Pawn")
                {
                    currentTroop.GetComponent<Image>().sprite = redPawn;
                }
                else if (gameManager.troopScript.gameObject.tag == "Knight")
                {
                    currentTroop.GetComponent<Image>().sprite = redKnight;
                }
                else if (gameManager.troopScript.gameObject.tag == "Archer")
                {
                    currentTroop.GetComponent<Image>().sprite = redArcher;
                }
                else if (gameManager.troopScript.gameObject.tag == "Mage")
                {
                    currentTroop.GetComponent<Image>().sprite = redMage;
                }
                else if (gameManager.troopScript.gameObject.tag == "Tank")
                {
                    currentTroop.GetComponent<Image>().sprite = redTank;
                }
                else if (gameManager.troopScript.gameObject.tag == "Priest")
                {
                    currentTroop.GetComponent<Image>().sprite = redPriest;
                }
            }
        }
        else if(gameManager.turn == Turn.Blue)
        {
            redLabel.SetActive(false);
            blueLabel.SetActive(true);

            if (gameManager.state == State.TroopPlacement && gameManager.phase != Phase.Checks && drag.troopScript != null)
            {
                stats.text = (drag.troopScript.Health + "\n" + drag.troopScript.attackDamage + "\n" + drag.troopScript.attackDist + "\n" + drag.troopScript.moveDist);

                if (drag.troopScript.gameObject.tag == "King")
                {
                    currentTroop.GetComponent<Image>().sprite = blueKing;
                }
                else if (drag.troopScript.gameObject.tag == "Pawn")
                {
                    currentTroop.GetComponent<Image>().sprite = bluePawn;
                }
                else if (drag.troopScript.gameObject.tag == "Knight")
                {
                    currentTroop.GetComponent<Image>().sprite = blueKnight;
                }
                else if (drag.troopScript.gameObject.tag == "Archer")
                {
                    currentTroop.GetComponent<Image>().sprite = blueArcher;
                }
                else if (drag.troopScript.gameObject.tag == "Mage")
                {
                    currentTroop.GetComponent<Image>().sprite = blueMage;
                }
                else if (drag.troopScript.gameObject.tag == "Tank")
                {
                    currentTroop.GetComponent<Image>().sprite = blueTank;
                }
                else if (drag.troopScript.gameObject.tag == "Priest")
                {
                    currentTroop.GetComponent<Image>().sprite = bluePriest;
                }
            }

            else if (gameManager.state == State.Gameplay && gameManager.phase != Phase.Checks && gameManager.troopScript != null)
            {
                stats.text = (gameManager.troopScript.Health + "\n" + gameManager.troopScript.attackDamage + "\n" + gameManager.troopScript.attackDist + "\n" + gameManager.troopScript.moveDist);

                if (gameManager.troopScript.gameObject.tag == "King")
                {
                    currentTroop.GetComponent<Image>().sprite = blueKing;
                }
                else if (gameManager.troopScript.gameObject.tag == "Pawn")
                {
                    currentTroop.GetComponent<Image>().sprite = bluePawn;
                }
                else if (gameManager.troopScript.gameObject.tag == "Knight")
                {
                    currentTroop.GetComponent<Image>().sprite = blueKnight;
                }
                else if (gameManager.troopScript.gameObject.tag == "Archer")
                {
                    currentTroop.GetComponent<Image>().sprite = blueArcher;
                }
                else if (gameManager.troopScript.gameObject.tag == "Mage")
                {
                    currentTroop.GetComponent<Image>().sprite = blueMage;
                }
                else if (gameManager.troopScript.gameObject.tag == "Tank")
                {
                    currentTroop.GetComponent<Image>().sprite = blueTank;
                }
                else if (gameManager.troopScript.gameObject.tag == "Priest")
                {
                    currentTroop.GetComponent<Image>().sprite = bluePriest;
                }
            }
        }

        if(gameManager.state == State.TroopPlacement || gameManager.phase == Phase.Checks)
        {
            phase.GetComponent<Image>().enabled = false;
            skipPhase.SetActive(false);
        }
        else if(gameManager.phase == Phase.Movement)
        {
            phase.GetComponent<Image>().enabled = true;
            phase.GetComponent<Image>().sprite = movement;
            skipPhase.SetActive(true);
        }
        else if (gameManager.phase == Phase.Attack)
        {
            phase.GetComponent<Image>().enabled = true;
            phase.GetComponent<Image>().sprite = attack;
            skipPhase.SetActive(true);
        }

        if(gameOver == true && YesNo == yesno.yes)
        {
            SceneManager.LoadScene(1);
        }
        else if (gameOver == true && YesNo == yesno.no)
        {
            SceneManager.LoadScene(0);
        }
	}

    public void yesClick()
    {
        YesNo = yesno.yes;
    }

    public void noClick()
    {
        YesNo = yesno.no;
    }

    public void resetStats()
    {
        stats.text = " ";
        currentTroop.GetComponent<Image>().sprite = null;
    }
    
    /*
    IEnumerator WaitForClick()
    {
        awaitingResponse = true;
        yield return new WaitForSeconds(5f);
        awaitingResponse = false;
    }
    */
    public void HideQuestion()
    {
        YesButton.SetActive(false);
        NoButton.SetActive(false);
        QuestionBox.SetActive(false);
        Question.text = " ";
    }

    public void SpawnPawnQuestion()
    {
        YesButton.SetActive(true);
        NoButton.SetActive(true);
        QuestionBox.SetActive(true);
        Question.text = "Do you wish to spawn your free pawn?";

        if (gameManager.turn == Turn.Red)
        {
            QuestionBox.GetComponent<RawImage>().texture = redQbox;
        }
        else if (gameManager.turn == Turn.Blue)
        {
            QuestionBox.GetComponent<RawImage>().texture = blueQBox;
        }
    }

    public void PriestQuestion()
    {
        YesButton.SetActive(true);
        NoButton.SetActive(true);
        QuestionBox.SetActive(true);
        Question.text = "Do you wish to revive any fallen troops?";
        //WaitForClick();

        if (gameManager.turn == Turn.Red)
        {
            QuestionBox.GetComponent<RawImage>().texture = redQbox;
        }
        else if (gameManager.turn == Turn.Blue)
        {
            QuestionBox.GetComponent<RawImage>().texture = blueQBox;
        }
    }

    public void TankQuestion(string SelectedTroop)
    {
        YesButton.SetActive(true);
        NoButton.SetActive(true);
        QuestionBox.SetActive(true);
        Question.text = "Do you wish to defend " + SelectedTroop + " by switching with your Tank?";
        //WaitForClick();

        if (gameManager.turn == Turn.Red)
        {
            QuestionBox.GetComponent<RawImage>().texture = blueQBox;
        }
        else if (gameManager.turn == Turn.Blue)
        {
            QuestionBox.GetComponent<RawImage>().texture = redQbox;
        }
    }

    public void MageQuestion()
    {
        YesButton.SetActive(true);
        NoButton.SetActive(true);
        QuestionBox.SetActive(true);
        Question.text = "Do you wish to teleport?";
        //WaitForClick();

        if (gameManager.turn == Turn.Red)
        {
            QuestionBox.GetComponent<RawImage>().texture = redQbox;
        }
        else if (gameManager.turn == Turn.Blue)
        {
            QuestionBox.GetComponent<RawImage>().texture = blueQBox;
        }
    }

    public void RevealDeadTroops()
    {
        reviveUI = true;
        revivebox.SetActive(true);
        if(gameManager.turn == Turn.Red)
        {
            redRevive.SetActive(true);
            blueRevive.SetActive(false);
            for(int i = 0; i < gameManager.deadRedTeam.Count; i++)
            {
                for(int x = 0; x < 5; x++)
                {
                    if(_redRevive[x].gameObject.name == gameManager.deadRedTeam[i].tag)
                    {
                        _redRevive[x].interactable = true;
                    }
                }
            }
        }

        else if (gameManager.turn == Turn.Blue)
        {

            blueRevive.SetActive(true);
            redRevive.SetActive(false);
            for (int i = 0; i < gameManager.deadBlueTeam.Count; i++)
            {
                for (int x = 0; x < 5; x++)
                {
                    if (_blueRevive[x].gameObject.name == gameManager.deadBlueTeam[i].tag)
                    {
                        _blueRevive[x].interactable = true;
                    }
                }
            }
        }
    }

    public void HideDeadTroops()
    {
        reviveUI = false;
        revivebox.SetActive(false);

        for (int x = 0; x < 5; x++)
        {
            _blueRevive[x].interactable = false;
            _redRevive[x].interactable = false;
        }
    }

    public void EndGameScreen(GameObject deadKing)
    {
        QuestionBox.SetActive(true);

        if (deadKing.transform.parent.tag == "RedTeam")
        {
            QuestionBox.GetComponent<RawImage>().texture = redQbox;
            Question.text = "Blue Team Wins. Play Again?";
            YesButton.SetActive(true);
            NoButton.SetActive(true);
            gameOver = true;
        }
        if (deadKing.transform.parent.tag == "BlueTeam")
        {
            QuestionBox.GetComponent<RawImage>().texture = blueQBox;
            Question.text = "Red Team Wins. Play Again?";
            YesButton.SetActive(true);
            NoButton.SetActive(true);
            gameOver = true;
        }
    }
}

public enum yesno
{
    yes, no, notSelected
};



