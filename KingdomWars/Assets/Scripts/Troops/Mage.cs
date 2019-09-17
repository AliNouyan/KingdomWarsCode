using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : TroopScript {

    bool initialize = false;
    //GameObject Manager;
    //GameManager gameManager;

    public Sprite mage3;
    public Sprite mage2;
    public Sprite mage1;

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

            moveDist = 2;
            attackDist = 0; //straight line
            attackDamage = 2;
            Health = 3;

            BackupHealth = 3;

            initialize = true;
        }

        base.Update(); //call parent update function

        if (Health >= 3)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = mage3;
        }
        else if (Health == 2)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = mage2;
        }
        else if (Health == 1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = mage1;
        }
    }
}
