using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : TroopScript {

    bool initialize = false;
    //GameObject Manager;
    //GameManager gameManager;

    public Sprite knight1;
    public Sprite knight2;
    public Sprite knight3;
    public Sprite knight4;
    public Sprite knight5;

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

            moveDist = 4;
            attackDist = 1;
            attackDamage = 3;
            Health = 5;

            BackupHealth = 5;

            initialize = true;
        }

        base.Update(); //call parent update function

        if (Health >= 5)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = knight5;
        }
        else if (Health == 4)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = knight4;
        }
        else if (Health == 3)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = knight3;
        }
        else if (Health == 2)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = knight2;
        }
        else if (Health == 1)
        {
            sprite.GetComponent<SpriteRenderer>().sprite = knight1;
        }
    }
}
