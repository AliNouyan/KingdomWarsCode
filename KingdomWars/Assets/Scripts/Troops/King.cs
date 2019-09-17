using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : TroopScript {

    bool initialize = false;

    //GameObject Manager;
    //GameManager gameManager;

    public Sprite king2;
    public Sprite king1;

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
            attackDist = 0;
            attackDamage = 0;
            Health = 2;

            BackupHealth = 2;
            initialize = true;
        }

        base.Update(); //call parent update function

        canDrag = true;

        if (Health >= 2)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = king2;
        }
        else if (Health == 1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = king1;
        }
    }
}
