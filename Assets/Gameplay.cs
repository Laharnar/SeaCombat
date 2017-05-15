using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour {

    // supports 20 players/ai
    const int maxShipLimit = 20;
    public const int numberOfDecisions = 4;
    Ship[] ships = new Ship[maxShipLimit];

    public Transform combatPlayerPref;
    public Map sea;
    bool[] shipsHadAction = new bool[maxShipLimit];

    const float timePerRound = 10;

    // Use this for initialization
    void Start() {
        // create players
        ships[0] = Instantiate(combatPlayerPref).GetComponent<Ship>();


        StartCoroutine(RunCombat());
    }
    Stage[] stages  =new Stage[maxShipLimit];

    IEnumerator RunCombat() {
        while (true) {
            // wait for end of turn
            float t = Time.time + timePerRound;
            while (t > Time.time) {
                for (int i = 0; i < maxShipLimit; i++) {
                    if (ships[i] == null) continue;
                    ships[i].targetPlayer.ui.uiTimer.text = "" + (t - Time.time);
                }
                yield return null;
            }

            // start execting actions, with sea actions on top
            yield return RunRound();
        }
    }

    IEnumerator RunRound() {

        // consume 1 action, apply sea action
        for (int i = 0; i < ships.Length; i++) {
            if (ships[i])
                ships[i].targetPlayer.ui.EndInput();
        }

        for (int decision = 0; decision < numberOfDecisions; decision++) {
            // load data
            for (int i = 0; i < ships.Length; i++) {
                if (ships[i]) {
                    stages[i] = ships[i].stages.Dequeue();
                } else stages[i] = Stage.DefaultStage();
            }
            // execute actions for the number or actions first ships has
            while (stages[0].actions.Count > 0) {
                bool anyEvents = false;
                // do ship actions
                // TODO: !!!! error?. should it be stages[smth]?
                for (int i = 0; i < ships.Length; i++) {
                    GameAction shipAction = stages[i].actions[0];
                    if (shipAction != null) {
                        bool pass = shipAction.Do(ships[i]);
                        if (pass) {
                            Debug.Log("found "+i);
                            anyEvents = true;
                            shipsHadAction[i] = true; // save, so sea knowns where to execute
                        }
                    }
                    stages[i].actions.RemoveAt(0);
                }
                // wait for action to finish
                if (anyEvents) {
                    Debug.Log("Ship action, waiting 3 secs.");
                    yield return new WaitForSeconds(3);
                    anyEvents = false;
                }
                // do sea action on every ship
                for (int i = 0; i < ships.Length; i++) {
                    Ship ship = ships[i];
                    if (ship != null && shipsHadAction[i]) {
                        if (sea.Do(ship)) {
                            anyEvents = true;
                            shipsHadAction[i] = false;// just reset
                        }
                    }
                }
                // wait sea actions
                if (anyEvents) {
                    Debug.Log("Sea, waiting 2 secs.");
                    yield return new WaitForSeconds(2);
                }
                Debug.Log("The move is done.");
            }

            // mark end of stage/decision
            Debug.Log("One decision ended.");
            yield return new WaitForSeconds(5);
        }
        Debug.Log("Round over.");
    }
}

public enum Visuals {
    Rules,
    NoRules
}
public class Wind : MapAction {
    public Vector3 direction;

    
    internal override bool Do(Ship ship) {
        Debug.Log("wind");
        return new MoveAction(direction, Visuals.NoRules).Do(ship);
    }
}
public abstract class MapAction : GameAction { }
public abstract class GameAction {
    internal abstract bool Do(Ship info);
}

public class CalmSea : MapAction {
    internal override bool Do(Ship info) {
        Debug.Log("Calm sea...");
        return true;
    }
}