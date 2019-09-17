using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopAnimController : MonoBehaviour {

    bool initialize = false;

    GameManager manager;
    float attackTiming;
    float attackOffset;
    float wait;
    float deathTime;
    float deathOffset;
    float reviveTiming;

    GameObject attackTroop;
    Transform attackTransform;
    GameObject defendTroop;
    Transform defendTransform;
    GameObject reviveTroop;

    bool playAnim;
    bool faceEachother = false;
    bool faceForward = false;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (initialize == false)
        {
            manager = GetComponent<GameManager>();

            initialize = true;
        }
    }

    private void FixedUpdate()
    {
        if (defendTroop != null && attackTroop != null)
        {
            if (faceEachother == true)
            {
                Quaternion FaceDefendTroop = Quaternion.LookRotation(defendTransform.position - attackTransform.position);
                attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, FaceDefendTroop, 4f * Time.deltaTime);

                Quaternion FaceAttackTroop = Quaternion.LookRotation(attackTransform.position - defendTransform.position);
                defendTroop.transform.rotation = Quaternion.Slerp(defendTransform.rotation, FaceAttackTroop, 4f * Time.deltaTime);

                if (playAnim == false)
                {
                    StartCoroutine(FightAnim());
                    faceForward = true;
                    playAnim = true;
                }
            }

            else if (faceEachother == false && faceForward == true)
            {
                if (attackTroop.GetComponent<TroopScript>().team == Team.Red)
                {
                    if(defendTroop.tag == "King")
                    {
                        Quaternion RedFaceForward = Quaternion.Euler(0, 0, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, RedFaceForward, 4f * Time.deltaTime);
                    }
                    else if(attackTroop.tag == "Priest")
                    {
                        Quaternion RedFaceForward = Quaternion.Euler(0, 0, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, RedFaceForward, 4f * Time.deltaTime);

                        Quaternion BlueFaceForward = Quaternion.Euler(0, 0, 0);
                        defendTroop.transform.rotation = Quaternion.Slerp(defendTransform.rotation, BlueFaceForward, 4f * Time.deltaTime);
                    }
                    else
                    {
                        Quaternion RedFaceForward = Quaternion.Euler(0, 0, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, RedFaceForward, 4f * Time.deltaTime);

                        Quaternion BlueFaceForward = Quaternion.Euler(0, 180, 0);
                        defendTroop.transform.rotation = Quaternion.Slerp(defendTransform.rotation, BlueFaceForward, 4f * Time.deltaTime);
                    }
                }

                if (attackTroop.GetComponent<TroopScript>().team == Team.Blue)
                {
                    if (defendTroop.tag == "King")
                    {
                        Quaternion RedFaceForward = Quaternion.Euler(0, 180, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, RedFaceForward, 4f * Time.deltaTime);
                    }
                    else if (attackTroop.tag == "Priest")
                    {
                        Quaternion RedFaceForward = Quaternion.Euler(0, 180, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, RedFaceForward, 4f * Time.deltaTime);

                        Quaternion BlueFaceForward = Quaternion.Euler(0, 180, 0);
                        defendTroop.transform.rotation = Quaternion.Slerp(defendTransform.rotation, BlueFaceForward, 4f * Time.deltaTime);
                    }
                    else
                    {
                        Quaternion BlueFaceForward = Quaternion.Euler(0, 180, 0);
                        attackTroop.transform.rotation = Quaternion.Slerp(attackTransform.rotation, BlueFaceForward, 4f * Time.deltaTime);

                        Quaternion RedFaceForward = Quaternion.Euler(0, 0, 0);
                        defendTroop.transform.rotation = Quaternion.Slerp(defendTransform.rotation, RedFaceForward, 4f * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void FaceEnemy(GameObject AttackTroop, GameObject DefendTroop)
    {
        faceForward = false;
       
        attackTroop = AttackTroop;
        attackTransform = attackTroop.transform;
        defendTroop = DefendTroop;
        defendTransform = defendTroop.transform;

        playAnim = false;
        faceEachother = true;
    }

    public IEnumerator FightAnim()
    {
        attackOffset = 0;
        deathOffset = 0;
        switch (attackTroop.tag)
        {
            case ("Knight"):
                attackTiming = 1.3f;
                wait = 2.6f;
                break;

            case ("Archer"):
                attackTiming = 6f;
                wait = 6.7f;
                break;

            case ("Mage"):
                attackTiming = 1.5f;
                wait = 5.7f;
                break;

            case ("Pawn"):
                attackTiming = 2f;
                wait = 4.6f;
                break;

            case ("Priest"):
                attackTiming = 2.5f;
                wait = 3.9f;
                break;

            case ("Tank"):
                attackTiming = 1.2f;
                wait = 2.2f;
                break;

            case ("King"):
                attackTiming = 1.3f;
                wait = 2.8f;
                break;
        }

        switch (defendTroop.tag)
        {
            case ("Knight"):
                attackOffset = 1;
                deathOffset = 0.2f;

                if (wait < 5.3f)
                {
                    wait = 5.3f;
                }

                deathTime = 3.3f;
                break;

            case ("Archer"):
                deathOffset = 0f;

                if (wait < 1.6f)
                {
                    wait = 1.6f;
                }

                deathTime = 3.7f;
                break;

            case ("Mage"):
                attackOffset = 1.6f;
                deathOffset = 0f;

                if (wait < 4.1f)
                {
                    wait = 4.1f;
                }

                deathTime = 4.4f;
                break;

            case ("Pawn"):
                attackOffset = 1.25f;
                deathOffset = 0.2f;

                if (wait < 4.3f)
                {
                    wait = 4.3f;
                }

                deathTime = 4f;
                break;

            case ("Priest"):
                deathOffset = 0.1f;

                if (wait < 3.3f)
                {
                    wait = 3.3f;
                }

                deathTime = 4.2f;
                break;

            case ("Tank"):
                deathOffset = 0.1f;

                if (wait < 1.3f)
                {
                    wait = 1.3f;
                }

                deathTime = 3.5f;
                break;

            case ("King"):

                deathOffset = 0.5f;

                if (wait < 3.7f)
                {
                    wait = 3.7f;
                }

                deathTime = 3.7f;
                break;
        }

        if (defendTroop.GetComponent<TroopScript>().Health <= 0)
        {
            attackTiming = attackTiming - deathOffset;

            if (wait < deathTime)
            {
                wait = deathTime;
            }

            if(defendTroop.tag != "King")
            {
                StartCoroutine(defendTroop.GetComponent<TroopScript>().KillTroop(wait));
            }
        }

        else
        {
            attackTiming = attackTiming - attackOffset;
        }

        if(attackTroop.tag == "Mage")
        {
            StartCoroutine(MageAttack());
        }

        StartCoroutine(FaceForward());

        if (attackTroop.tag == "Priest")
        {
            PriestHeal();
        }

        else
        {
            if (attackTiming >= 0f)
            {
                StartCoroutine(attackTroop.GetComponent<TroopScript>().PlayAttackAnim());
                yield return new WaitForSeconds(attackTiming);
                StartCoroutine(defendTroop.GetComponent<TroopScript>().PlayDefenceAnim());
            }
            else if (attackTiming < 0)
            {
                StartCoroutine(defendTroop.GetComponent<TroopScript>().PlayDefenceAnim());
                attackTiming *= -1;
                yield return new WaitForSeconds(attackTiming);
                StartCoroutine(attackTroop.GetComponent<TroopScript>().PlayAttackAnim());
            }
        }

       
    }

    IEnumerator MageAttack()
    {
        yield return new WaitForSeconds(1.5f);

        defendTroop.GetComponent<TroopScript>().tileScript.PlayFireAttack(); 
    }

    public void PriestHeal()
    {
        StartCoroutine(attackTroop.GetComponent<TroopScript>().PlayAttackAnim());
        defendTroop.GetComponent<TroopScript>().tileScript.PlayPriestAttack();
    }


    IEnumerator FaceForward()
    {
        yield return new WaitForSeconds(wait - 0.5f);

        faceEachother = false;

        yield return new WaitForSeconds(0.5f);

        if (defendTroop.tag != "King" || defendTroop == null)
        {
            CallChangePhase();
        }
        else if (defendTroop.tag == "King")
        {
            StartCoroutine(defendTroop.GetComponent<TroopScript>().KillTroop(2f));
        }
    }

    public IEnumerator ReviveAnim(GameManager manager, GameObject troop)
    {
        Debug.Log(troop);
        reviveTroop = troop;
        switch (reviveTroop.tag)
        {
            case ("Knight"):
                reviveTiming = 4.6f;
                break;

            case ("Archer"):
                reviveTiming = 4.3f;
                break;

            case ("Mage"):
                reviveTiming = 3.8f;
                break;

            case ("Pawn"):
                reviveTiming = 5.2f;
                break;

            case ("Tank"):
                reviveTiming = 4.7f;
                break;
        }

        StartCoroutine(reviveTroop.GetComponent<TroopScript>().PlayReviveAnim());

        yield return new WaitForSeconds(reviveTiming);

        if(manager.drag.kingAbility == true)
        {
            manager.drag.spawnedBluePawn = null;
            manager.drag.spawnedRedPawn = null;
            manager.drag.kingAbility = false; //turn off ability
        }

        manager.PauseChecks = false;
        manager.CheckCount++;
        StartCoroutine(manager.PerformChecks());
    }

    public void win()
    {
        if(manager.turn == Turn.Red)
        {
            for (int i = 0; i < manager.RedTeam.Count; i++)
            {
                manager.RedTeam[i].GetComponent<TroopScript>().WinAnim();
            }
        }
    }

    void CallChangePhase()
    {
        manager.ChangePhase();
    }
}
