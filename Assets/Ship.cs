using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ship : MonoBehaviour {

    internal int xInGrid;
    /// <summary>
    /// Y in grid is z in world position.
    /// </summary>
    internal int yInGrid;

    public Player targetPlayer;

    // (4 guns, 1 movement) * 4
    ShipAction[] decisions = new ShipAction[5*Gameplay.numberOfDecisions];
    
    internal void ChangeDecision(int i, ShipAction decision) {
        decisions[i] = decision;
    }

    public Queue<Stage> stages = new Queue<Stage>();

    internal void ClearActions() {
        stages.Clear();
    }

    internal void AddStage(int activeMove, int[] activeCannons) {
        List<GameAction> actions = new List<GameAction>();
        // first big(move actions), then guns
        actions.Add(new MoveAction(MoveAction.Convert(activeMove)
            , Visuals.Rules));

        // outer first, then inner guns
        actions.Add(new ShootAction(ShootAction.Convert(activeCannons[1], activeCannons[3])));
        actions.Add(new ShootAction(ShootAction.Convert(activeCannons[0], activeCannons[2])));

        stages.Enqueue(new Stage(actions));
    }

    
}
public class Stage {
    public List<GameAction> actions;
    public Stage(List<GameAction> actions) {
        this.actions = actions;
    }

    internal static Stage DefaultStage() {
        List<GameAction> ga = new List<GameAction>();
        ga.Add(null);
        ga.Add(null);
        ga.Add(null);
        return new Stage(ga);
    }
}

public abstract class ShipAction : GameAction { }

public class MoveAction :ShipAction{

    protected Vector3 direction;
    protected Visuals noRules;

    public MoveAction(Vector3 direction, Visuals noRules) {
        this.direction = direction;
        this.noRules = noRules;
    }

    /// <summary>
    /// Converts Ui data to direction
    /// </summary>
    /// <param name="activeMove"></param>
    /// <returns></returns>
    internal static Vector3 Convert(int activeMove) {
        //see: PlayerUI, none, left, forward, right  | in that order
        switch (activeMove) {
            case 0: return Vector3.zero;
            case 1: return Vector3.left;
            case 2: return Vector3.forward;
            case 3: return Vector3.right;
            default: break;
        }
        Debug.Log("Unknown move id " + activeMove + ".");
        return Vector3.zero;
    }

    internal override bool Do(Ship ship) {
        if (direction == Vector3.zero) {
            return false;
        }
        // apply rotations, starting from right, with right priority
        Vector3 rotation = ship.transform.forward;// ship.forward
        int goX = (int)direction.x;
        int goY = (int)direction.y;// make back/forward move

        /*if (rotation == Vector3.right) 
            result = Vector3.right + Vector3.back * goX;
        else if (rotation == Vector3.back) 
            result = Vector3.back + Vector3.left * goX;
        else if (rotation == Vector3.left)
            result = -Vector3.right - Vector3.back * goX;
        else if(rotation == Vector3.forward)
            result = -Vector3.back - Vector3.left * goX;*/

        //if (goY == 1 || goY == -1)
        //if (rotation == Vector3.right) result = Vector3.right * 2 * goY;
        // gets direction to new forward position

        Vector3 frontMove = rotation.x * Vector3.right * 2 * goY
            + rotation.z * Vector3.forward * 2 * goY;

        // gets new left/right direction
        Vector3 sideMove =
            (rotation.x * Vector3.back * goX
            + rotation.z * Vector3.right * goX);

        // gets new position
        Vector3 resultMove = rotation + direction.x * sideMove
            + direction.y * frontMove;

        /*ship.xInGrid = ship.xInGrid + (int)direction.x;
        ship.yInGrid = ship.yInGrid + (int)direction.z;*/
        ship.xInGrid = ship.xInGrid + (int)resultMove.x;
        ship.yInGrid = ship.yInGrid + (int)resultMove.z;
        // todo: fix rotation
        ship.transform.position = new Vector3(ship.xInGrid, 0, ship.yInGrid);
        ship.transform.rotation.SetLookRotation(ship.transform.right, Vector3.up);

        Debug.Log("Ship is moving and rotating");

        return true;
    }
}

class ShootAction : ShipAction {
    private Sides activatedCannons;

    public ShootAction(Sides sides) {
        this.activatedCannons = sides;
    }

    public enum Sides {
        None,
        Left,
        Right,
        Both
    }

    internal static Sides Convert(int shootLeft, int shootRight) {
        //reference: PlayerUi  0: don't fire 1: fire
        // return 0: none, 1: l 2:r 3: both
        return shootLeft == 0 ? 
              shootRight == 0 ? Sides.None : Sides.Right
            : shootRight == 0 ? Sides.Left : Sides.Both;
    }
    
    internal override bool Do(Ship ship) {
        //ship.Fire(dir)
        // shoot in direction local to ship's
        switch (activatedCannons) {
            case Sides.None:
                return false;
            case Sides.Left:
                Debug.Log("firing gun/s");
                //ship.FireLeft();
                break;
            case Sides.Right:
                Debug.Log("firing gun/s");
                //ship.FireRight();
                break;
            case Sides.Both:
                Debug.Log("firing gun/s");
                //ship.FireRight();
                //ship.FireLeft();
                break;
            default:
                break;
        }
        return true;
    }
}

