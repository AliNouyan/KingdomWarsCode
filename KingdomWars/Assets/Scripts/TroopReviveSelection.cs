using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopReviveSelection : MonoBehaviour {

    public revive Value;
    public GameManager manager;

    public void reviveFunc()
    {
        manager.ReviveTroop = Value;
        manager.ReviveSelectedTroop();
    }
}
