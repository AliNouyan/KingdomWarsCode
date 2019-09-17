using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : TroopScript {

    bool initialize = false;
    //GameObject Manager;
    //GameManager gameManager;
    public bool moved = false;

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
            attackDist = 1;
            attackDamage = 2;
            Health = 1;

            BackupHealth = 1;

            initialize = true;
        }

        base.Update(); //call parent update function
    }
}
