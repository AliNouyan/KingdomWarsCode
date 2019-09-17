using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopScript : MonoBehaviour {

    UIController UI;
    GameManager gameManager;
    TroopAnimController anim;
    public GameObject tile;
    public Tile tileScript;
    public GameObject sprite;
    public Team team;
    public TroopState troopState;

    public float Startx;
    public float Starty;
    public float Curx;
    public float Cury;

    public float moveDist;
    public float attackDist;
    public int attackDamage;
    public int Health;

    public int BackupHealth;

    public bool canDrag = true; //Change when it gets to that point

    RaycastHit TroopHit;
    Ray TroopRay;

    public Animator Anim;

    bool setKing = false;

    // Use this for initialization
    public void Start()
    { 
        Startx = gameObject.transform.position.x;
        Starty = gameObject.transform.position.z;

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        anim = gameManager.GetComponent<TroopAnimController>();
        UI = gameManager.GetComponent<UIController>();

        sprite = this.transform.GetChild(0).gameObject;
        sprite.SetActive(false);

        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update()
    {
        if(this.gameObject.tag == "King" && setKing == false)
        {
            troopState = TroopState.Alive;
            setKing = true;
        }

        Anim.SetFloat("Health", this.Health);

        Curx = gameObject.transform.position.x;
        Cury = gameObject.transform.position.z;

        TroopRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(TroopRay, out TroopHit, 100))
        {
            if (TroopHit.transform.gameObject == this.gameObject)
            {
                sprite.SetActive(true);
            }
            else
            {
                sprite.SetActive(false);
            }
        }

        switch (gameManager.state)
        {
            case (State.TroopPlacement): //if troop placement state
                //if troop is in blue team
                if (this.team == Team.Blue)
                {
                    //if the troop hasn't been placed on board
                    if (gameManager.turn == Turn.Blue && this.troopState == TroopState.NotPlaced)
                    {
                        canDrag = true; //allow player to move
                    }
                    else
                    {
                        canDrag = false; //player cant move
                    }
                }
                //if troop is in red team
                if (this.team == Team.Red)
                {
                    //if the troop hasn't been placed on board
                    if (gameManager.turn == Turn.Red && this.troopState == TroopState.NotPlaced)
                    {
                        canDrag = true; //allow player to move
                    }
                    else
                    {
                        canDrag = false; //player cant move
                    }
                }
                break;

            case (State.Gameplay): //if gameplay state
                if(troopState == TroopState.Alive)
                {
                    //if troop is in blue team
                    if (this.team == Team.Blue)
                    {
                        if (this.troopState == TroopState.Alive)
                        {
                            var check = gameManager.SearchList(gameManager.deadBlueTeam, this.tag.ToString());
                            if (check != null)
                            {
                                gameManager.deadBlueTeam.Remove(this.gameObject);
                            }
                        }

                        //if blue teams turn
                        if (gameManager.turn == Turn.Blue)
                        {
                            if(gameManager.BlueCurPiece != null)
                            {
                                if (this.tag == "King")
                                {
                                    canDrag = true;
                                }
                                //if this troops turn to move
                                else if (this.tag == gameManager.BlueCurPiece.tag)
                                {
                                    canDrag = true; //allow player to move
                                }
                                else
                                {
                                    canDrag = false; //player cant move
                                }
                            }
                        }
                        //if not blue teams turn
                        else
                        {
                            canDrag = false; //player cant move
                        }
                    }
                    //if troop is in red team
                    else if (this.team == Team.Red)
                    {
                        if (this.troopState == TroopState.Alive)
                        {
                            var check = gameManager.SearchList(gameManager.deadRedTeam, this.tag.ToString());
                            if (check != null)
                            {
                                gameManager.deadRedTeam.Remove(this.gameObject);
                            }
                        }

                        //if red teams turn
                        if (gameManager.turn == Turn.Red)
                        {
                            if(gameManager.RedCurPiece != null)
                            {

                                //if this troops turn to move
                                if (this.tag == gameManager.RedCurPiece.tag)
                                {
                                    canDrag = true; //allow player to move
                                }
                                else
                                {
                                    canDrag = false; //player cant move
                                }
                            }
                        }
                        //if not red teams turn
                        else
                        {
                            canDrag = false; //player cant move
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    public void ReCallStartPos()
    {
        Startx = gameObject.transform.position.x;
        Starty = gameObject.transform.position.z;
    }

    public void ResetHealth()
    {
        Health = BackupHealth;
    }

    public IEnumerator PlayDefenceAnim()
    {
        Anim.SetBool("Attacked", true);
        yield return new WaitForSeconds(0.2f);
        Anim.SetBool("Attacked", false);
    }

    public IEnumerator PlayAttackAnim()
    {
        Anim.SetBool("Attack", true);
        yield return new WaitForSeconds(0.2f);
        Anim.SetBool("Attack", false);
    }

    public IEnumerator PlayReviveAnim()
    {
        Anim = GetComponent<Animator>();

        Anim.SetBool("Revive", true);
        yield return new WaitForSeconds(0.2f);
        Anim.SetBool("Revive", false);
    }
    
    public IEnumerator KillTroop(float waitTime)
    {
        if (Health <= 0 & this.troopState == TroopState.Alive)
        {
            Debug.Log(this.gameObject);
            yield return new WaitForSeconds(waitTime);
            troopState = TroopState.Dead; //set troop to dead
            if(this.gameObject.tag != "King")
            {
                Debug.Log("HI");
                gameManager.KillPlayer(this.gameObject, this.team); //kill the player
            }

            else if (this.gameObject.tag == "King")
            {
                //end the game
                Debug.Log("hey");
                anim.win();
                yield return new WaitForSeconds(4f);
                UI.EndGameScreen(this.gameObject);
            }
        }
    }

    public void WinAnim()
    {
        Anim.SetBool("Win", true);
    }
}

public enum Team
{
    Red, Blue
};

public enum TroopState
{
    NotPlaced, Alive, Dead
};

