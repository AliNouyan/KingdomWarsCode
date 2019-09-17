using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : TroopScript {

    bool initialize = false;
    //GameObject Manager;
    //GameManager gameManager;

    public Sprite Tank1;
    public Sprite Tank2;
    public Sprite Tank3;
    public Sprite Tank4;
    public Sprite Tank5;
    public Sprite Tank6;
    public Sprite Tank7;
    public Sprite Tank8;

    // Use this for initialization
    void Start()
    {


        //Manager = GameObject.FindGameObjectWithTag("GameController");
        //gameManager = Manager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (initialize == false)
        {
            base.Start(); //call parent start function

            //set troop variables

            moveDist = 3;
            attackDist = 2;
            attackDamage = 2;
            Health = 8;

            BackupHealth = 8;
            initialize = true;
        }

        base.Update(); //call parent update function

        if (Health >= 8)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank8;
        } 
        else if (Health == 7)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank7;
        }
        else if (Health == 6)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank6;
        }
        else if (Health == 5)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank5;
        }
        else if (Health == 4)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank4;
        }
        else if (Health == 3)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank3;
        }
        else if (Health == 2)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank2;
        }
        else if (Health == 1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = Tank1;
        }
    }
    /*
    public IEnumerator PlayDefenceAnim()
    {
        yield return new WaitForSeconds(0f);
        Anim.SetBool("Attacked", true);
    }

    public IEnumerator PlayAttackAnim()
    {
        yield return new WaitForSeconds(0f);
        Anim.SetBool("Attacked", true);
    }
    */
}
