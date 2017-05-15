using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {
    public Ship combat;
    public PlayerUi ui;

    private void Awake() {
        ui.transform.SetParent(null, false);
    }
}
