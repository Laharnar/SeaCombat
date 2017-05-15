using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Stores all ui changes in usable format.
/// </summary>
public class PlayerUi : MonoBehaviour {

    public Player player;
    public Text uiTimer;

    UiActions[] actions = new UiActions[numOfUiGroups];

    const int numOfUiGroups = 4;
    const int cannonTypes = 2;
    const int numOfShots = 4;
    const int numOfPlayerMoves = 4;//none, left, forward, right  | in that order

    private void Awake() {
        for (int i = 0; i < numOfUiGroups; i++) {
            actions[i] = new UiActions();
        }
    }

    public void ChangeState(ActionData n) {
        if (!actions[n.id].ChangeState(n.action)) {
            // TODO: request moves. ??
            Debug.LogWarning("Unknown player ui state, id: "+n.id + " action:" + n.action);
        }
    }

    /// <summary>
    /// Player's input is received.
    /// </summary>
    public void EndInput() {
        player.combat.ClearActions();
        for (int i = 0; i < actions.Length; i++) {
            player.combat.AddStage(actions[i].activeMove, actions[i].activeCannons);
        }
        player.ui.ClearData();
    }

    private void ClearData() {
        for (int i = 0; i < numOfUiGroups; i++) {
            actions[i] = new UiActions();
        }
    }

    class UiActions {
        // an action
        internal int activeMove = 0;
        // right inner cannon, right outer, left inner, left outer  | in that order
        internal int[] activeCannons = new int[numOfShots]; // 0: don't fire 1: fire

        /// <summary>
        /// Sets current state to next.
        /// </summary>
        /// <param name="next"></param>
        internal bool ChangeState(string next) {
            string[] data = next.Split(' ');
            if (data[0] == "move") {
                activeMove = (activeMove + 1) % numOfPlayerMoves;
                Debug.Log("Setting move to: " + activeMove + " - none, left, forward, right");
                return true;
            }

            if (data[0] == "cannon") {
                if (data.Length < 2) Debug.LogError("Incorrect cannon toggle parameters, expected 2.");

                int cannonIndex = int.Parse(data[1]);
                activeCannons[cannonIndex] = (activeCannons[cannonIndex] + 1) % cannonTypes;
                Debug.Log("Activating cannon " + cannonIndex + " active: " + activeCannons[cannonIndex]);
                return true;
            }

            return false;
        }
    }
}

