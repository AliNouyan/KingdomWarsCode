using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    public GameObject ControlBox;
    public bool controlBox = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ShowControls()
    {
        ControlBox.SetActive(true);
        controlBox = true;
    }

    public void HideControls()
    {
        ControlBox.SetActive(false);
        controlBox = false;
    }
}
