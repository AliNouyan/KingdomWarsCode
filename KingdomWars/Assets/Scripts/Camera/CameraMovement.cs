using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    bool initialize = false;

    GameManager gameManager;

    public GameObject CamTarget;
    public RotationDir Dir;

    float dist = 0;
    float originX = 0;
    float newx;

    bool rotating = false;
    public bool resetCamera = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (initialize == false)
        {
            gameManager = GetComponent<GameManager>();

            initialize = true;
        }

        //if gameplay state
        if (gameManager.state == State.Gameplay)
        {
            //whilst holding right mouse button
            if (Input.GetMouseButton(1) && resetCamera == false)
            {
                rotating = true; //allow camera to rotate

                newx = Input.mousePosition.x; //constantly update the mouse x position

                //if origin mouse x position isnt set
                if (originX == 0)
                {
                    originX = newx; //set origin mouse x position
                }
                dist = newx - originX; //calculate how far mouse has moved

                //if player moves mouse left
                if (newx < originX)
                {
                    Dir = RotationDir.left; //Set direction enum left
                }
                //if player moves mouse right
                else if (newx > originX)
                {
                    Dir = RotationDir.right; //Set direction enum right
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //if gameplay state
        //if (gameManager.state == State.Gameplay)
        //{
            if (rotating == true)
            {
                CamTarget.transform.Rotate(new Vector3(0, dist / 300, 0));
            }

            if (Input.GetMouseButtonUp(1))
            {
                rotating = false;
                originX = 0;
            }
        //}
        
        if (resetCamera == true)
        {
            if (gameManager.turn == Turn.Red) //if red turn
            {
                Vector3 rotateTarget = new Vector3(0, 180, 0); //set target rotation
                var dif = Vector3.Distance(CamTarget.transform.eulerAngles, rotateTarget); //calculate difference between camera rotation and target rotation

                //if diference higher than value of 1.5
                if (dif > 0.5f)
                {
                    //rotate the camera
                    Quaternion RotateTarget = Quaternion.Euler(rotateTarget);
                    CamTarget.transform.rotation = Quaternion.Slerp(CamTarget.transform.rotation, RotateTarget, Time.deltaTime);
                }
                //else stop rotating camera and change turn
                else if (dif < 0.5f)
                {
                    resetCamera = false;
                    gameManager.ChangeTurn();
                }

            }
            else if (gameManager.turn == Turn.Blue) //if blue turn
            {
                Vector3 rotateTarget = new Vector3(0, 0, 0); //set target rotation
                var dif = Vector3.Distance(rotateTarget, CamTarget.transform.eulerAngles); //calculate difference between camera rotation and target rotation

                //if dif is high then check dif to be less than 358.5 or if dif low then check dif to be higher than 1.5
                if ((dif > 180 && dif < 359.75f) || (dif <= 180 && dif > 0.25f))
                {
                    //rotate the camera
                    Quaternion RotateTarget = Quaternion.Euler(rotateTarget);
                    CamTarget.transform.rotation = Quaternion.Slerp(CamTarget.transform.rotation, RotateTarget, Time.deltaTime);
                }
                //else stop rotating and chance turn
                else
                {
                    resetCamera = false;
                    gameManager.ChangeTurn();
                }
            }
        }
        
    }

    //reset camera function
    public void ResetCamera()
    {
        resetCamera = true;
    }
    //reset camera function
}

public enum RotationDir
{
    left, right, none
};

