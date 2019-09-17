
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    bool initialize = false;

    GameObject gameManager;
    GameManager manager;
    DragAndDrop drag;
    Attack attack;

    public float x;
    public float y;

    RaycastHit troopCheckHit;
    Ray troopCheckRay;

    public bool troopOnTile;
    public GameObject troop;
    public TroopScript troopScript;

    int troopX;
    int troopY;
    bool calc = false;
    bool attackCalc = false;
    bool pawnLight = false;

    int troopMask = ((1 << 9) | (1 <<10));

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (initialize == false)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController");
            manager = gameManager.GetComponent<GameManager>();
            drag = gameManager.GetComponent<DragAndDrop>();
            attack = gameManager.GetComponent<Attack>();

            x = gameObject.transform.position.x;
            y = gameObject.transform.position.z;

            initialize = true;
        }

        //cast ray up from middle of tile
        //if hits a troop
        if (Physics.Raycast(transform.position, Vector3.up, out troopCheckHit, 1f, troopMask))
        {
            troopOnTile = true; //set true
            //if(troop != troopCheckHit.transform.gameObject)
            //{
                troop = troopCheckHit.transform.gameObject;
                troopScript = troop.GetComponent<TroopScript>();
                troopScript.tile = this.gameObject;
                troopScript.tileScript = this;
            //}
        }
        //if troop not hit
        else
        {
            troopOnTile = false; //set false
            troop = null;
            troopScript = null;
        }

        
        if (drag.Dragging == true && calc == false)
        {
            floorCalculator();
            calc = true;
        }
        else if (drag.Dragging == false && calc == true)
        {
            StopFloorIndicators();
        }

        if (troopOnTile == true)
        {
            if (attack.troopSelected == false && troop.tag == "Pawn" && pawnLight == false && manager.phase == Phase.Attack)
            {
                if(manager.turn == Turn.Red && manager.RedCurPiece.tag == "Pawn" && troopScript.team == Team.Red)
                {
                    PlayRedAttackIndicator();
                    pawnLight = true;
                }
                else if(manager.turn == Turn.Blue && manager.BlueCurPiece.tag == "Pawn" && troopScript.team == Team.Blue)
                {
                    PlayBlueAttackIndicator();
                    pawnLight = true;
                }
            }
            else if(pawnLight == true)
            {
                StopAttackIndicator();
                pawnLight = false;
            }
            if (attack.troopSelected == true && attack.enemyIndicator == true && attackCalc == false)
            {
                AttackIndicator();
                attackCalc = true;
            }
            else if (attack.enemyIndicator == false && attackCalc == true)
            {
                StopAttackIndicator();
            }
        }
    }

    public void PlaySmokeParticles()
    {
        var smoke = transform.GetChild(0);
        var particle1 = smoke.transform.GetChild(0).GetComponent<ParticleSystem>();
        var particle2 = smoke.transform.GetChild(1).GetComponent<ParticleSystem>();
        var particle3 = smoke.transform.GetChild(2).GetComponent<ParticleSystem>();

        particle1.Play();
        particle2.Play();
        particle3.Play();
    }

    public void PlayFireAttack()
    {
        var mageAttack = transform.GetChild(1);
        var particle1 = mageAttack.transform.GetChild(0).GetComponent<ParticleSystem>();
        var particle2 = mageAttack.transform.GetChild(1).GetComponent<ParticleSystem>();
        var particle3 = mageAttack.transform.GetChild(2).GetComponent<ParticleSystem>();
        var particle4 = mageAttack.transform.GetChild(3).GetComponent<ParticleSystem>();

        particle1.Play();
        particle2.Play();
        particle3.Play();
        particle4.Play();
    }

    public void PlayPriestAttack()
    {
        var priestAttack = transform.GetChild(2);
        var particle1 = priestAttack.transform.GetChild(0).GetComponent<ParticleSystem>();
        var particle2 = priestAttack.transform.GetChild(1).GetComponent<ParticleSystem>();
        var particle3 = priestAttack.transform.GetChild(2).GetComponent<ParticleSystem>();

        particle1.Play();
        particle2.Play();
        particle3.Play();
    }

    void floorCalculator()
    {
        if (manager.state == State.TroopPlacement || drag.spawningTroop == true)
        {
            if(troopOnTile == false || troopOnTile == true && troop.tag == drag.SelectedTroop.tag)
            {
                switch (manager.turn)
                {
                    case (Turn.Red): //red team turn
                                     //if selected tile is within the red teams troop placement range
                        if (this.transform.position.z >= 0 && this.transform.position.z <= 2)
                        {
                            PlayRedIndicator();
                        }
                        break;

                    case (Turn.Blue): //blue team turn
                                      //if selected tile is within the blue teams troop placement range
                        if (this.transform.position.z >= 7 && this.transform.position.z <= 9)
                        {
                            PlayBlueIndicator();
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        else if (manager.state == State.Gameplay)
        {
            troopX = Mathf.RoundToInt(drag.troopScript.Startx);
            troopY = Mathf.RoundToInt(drag.troopScript.Starty);
            var maxMove = drag.troopScript.moveDist;

            var difX = troopX - this.transform.position.x;
            var difY = troopY - this.transform.position.z;

            if (difX < 0)
            {
                difX = difX * -1;
            }

            //if difY is negative integer, make posative
            if (difY < 0)
            {
                difY = difY * -1;
            }

            var dif = difX + difY; //calculate total difference

            if (drag.troopScript.team == Team.Red)
            {
                if (dif == 0)
                {
                    PlayBlueIndicator();
                }
                
                else if (dif <= maxMove && (troopOnTile == false || troopOnTile == true && troop == drag.SelectedTroop))
                {
                    PlayRedIndicator();
                }
            }
            else if (drag.troopScript.team == Team.Blue)
            {
                if (dif == 0)
                {
                    PlayRedIndicator();
                }   
                else if (dif <= maxMove && (troopOnTile == false || troopOnTile == true && troop == drag.SelectedTroop))
                {
                    PlayBlueIndicator();
                }
            }
        }
    }

    void PlayRedIndicator()
    {
        var indicator = transform.GetChild(3);
        var particle1 = indicator.transform.GetChild(1).GetComponent<ParticleSystem>();
        particle1.Play();
    }

    void PlayBlueIndicator()
    {
        var indicator = transform.GetChild(3);
        var particle1 = indicator.transform.GetChild(0).GetComponent<ParticleSystem>();
        particle1.Play();
    }

    public void StopFloorIndicators()
    {
        calc = false;

        var indicator = transform.GetChild(3);
        var particle1 = indicator.transform.GetChild(0).GetComponent<ParticleSystem>();
        var particle2 = indicator.transform.GetChild(1).GetComponent<ParticleSystem>();
        particle1.Stop();
        particle2.Stop();
    }

    void AttackIndicator()
    {
        if(attack.SelectedTroop.tag == "Mage")
        {
            if(attack.SelectedTroop.transform.position.x == this.transform.position.x || attack.SelectedTroop.transform.position.z == this.transform.position.z)
            {
                if (attack.troopScript.team == Team.Red)
                {
                    if (troopScript.team == Team.Blue)
                    {
                        PlayRedAttackIndicator();
                    }
                }
                else if (attack.troopScript.team == Team.Blue)
                {
                    if (troopScript.team == Team.Red)
                    {
                        PlayBlueAttackIndicator();
                    }
                }
            }
        }
        else
        {
            troopX = Mathf.RoundToInt(attack.troopScript.Startx);
            troopY = Mathf.RoundToInt(attack.troopScript.Starty);
            var maxRange = attack.troopScript.attackDist;

            var difX = troopX - this.transform.position.x;
            var difY = troopY - this.transform.position.z;

            if (difX < 0)
            {
                difX = difX * -1;
            }

            //if difY is negative integer, make posative
            if (difY < 0)
            {
                difY = difY * -1;
            }

            var dif = difX + difY; //calculate total difference

            if (attack.troopScript.team == Team.Red)
            {
                if (attack.SelectedTroop.tag != "Priest" && dif <= maxRange && troopScript.team == Team.Blue)
                {
                    PlayRedAttackIndicator();
                }
                else if (attack.SelectedTroop.tag == "Priest" && dif <= maxRange && troopScript.team == Team.Red && attack.SelectedTroop != troop)
                {
                    PlayRedAttackIndicator();
                }
            }
            else if (attack.troopScript.team == Team.Blue)
            {
                if (dif <= maxRange && troopScript.team == Team.Red)
                {
                    PlayBlueAttackIndicator();
                }
                else if (attack.SelectedTroop.tag == "Priest" && dif <= maxRange && troopScript.team == Team.Blue && attack.SelectedTroop != troop)
                {
                    PlayBlueAttackIndicator();
                }
            }
        }
    }

    void PlayBlueAttackIndicator()
    {
        var indicator = transform.GetChild(4);
        var particle1 = indicator.transform.GetChild(0).GetComponent<ParticleSystem>();
        particle1.Play();
    }

    void PlayRedAttackIndicator()
    {
        var indicator = transform.GetChild(4);
        var particle1 = indicator.transform.GetChild(1).GetComponent<ParticleSystem>();
        particle1.Play();
    }
    public void StopAttackIndicator()
    {
        attackCalc = false;
        //attack.enemyIndicator = true;

        var indicator = transform.GetChild(4);
        var particle1 = indicator.transform.GetChild(0).GetComponent<ParticleSystem>();
        var particle2 = indicator.transform.GetChild(1).GetComponent<ParticleSystem>();
        particle1.Stop();
        particle2.Stop();
    }
}
