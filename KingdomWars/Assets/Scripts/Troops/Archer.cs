using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : TroopScript
{
    bool initialize = false;

    //GameObject Manager;
    //GameManager gameManager;

    public Sprite archer1;
    public Sprite archer2;
    public Sprite archer3;

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
            attackDist = 5;
            attackDamage = 1;
            Health = 3;

            BackupHealth = 3;

            initialize = true;
        }

        base.Update(); //call parent update function

        if (Health >= 3)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = archer3;
        }
        else if (Health == 2)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = archer2;
        }
        else if (Health == 1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = archer1;
        }
    }
}