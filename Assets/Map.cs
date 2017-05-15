using UnityEngine;
using System.Collections;
using System;

public class Map : MonoBehaviour {

    public int width, length;

    Grid sea;

    public Transform seaPref;

    public static MapAction defaultSeaAction = new CalmSea();
    public bool generateMap = false;

    private void Awake() {
        // make grid
        Transform[] t = new Transform[width * length];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                int i = y * width + x;
                t[i] = Instantiate(seaPref);
                t[i].position = new Vector3(x, 0, y);
                t[i].parent = transform;
            }
        }
        // make sea
        sea = new Grid(width, length, t);

    }
    
    internal bool IsTaken(int x, int y) {
        return sea.slots[x, y].target != null;
    }

    internal bool CanMoveTo(int xTo, int yTo) {
        if (!IsTaken(xTo, yTo)) {
            return true;
        }
        return false;
    }

    internal bool CanShootX(int xFrom, int yFrom, int xDir) {
        return !AnyObstaclesX(xFrom, yFrom, 3, xDir);            
    }

    internal bool CanShootY(int xFrom, int yFrom, int yDir) {
        return !AnyObstaclesY(xFrom, yFrom, 3, yDir);
    }

    internal bool Do(Ship ship) {
        
        return ship != null &&
            sea.slots[ship.xInGrid, ship.yInGrid].onStay.Do(ship);
    }

    /// <summary>
    /// Finds if there are obstacles on path, excluding from
    /// </summary>
    /// <param name="xFrom"></param>
    /// <param name="yFrom"></param>
    /// <param name="distance">positive count</param>
    /// <param name="xDir">1 for right or -1 for left, step</param>
    /// <returns></returns>
    internal bool AnyObstaclesX(int xFrom, int yFrom, int distance, int xDir=1) {
        // assumed they are on sameline
        bool obstacles = false;
        for (int i = 0; i < distance; i++) {
            xFrom += xDir;
            if (xFrom > 0 && xFrom <width) {
                if (sea.slots[xFrom, yFrom].target != null) {
                    obstacles = true;
                    break;
                }
            }
        }
        return obstacles;
    }

    internal bool AnyObstaclesY(int xFrom, int yFrom, int distance, int yDir) {
        // assumed they are on sameline
        bool obstacles = false;
        for (int i = 0; i < distance; i++) {
            yFrom += yDir;
            if (yFrom > 0 && yFrom < length) {
                if (sea.slots[xFrom, yFrom].target != null) {
                    obstacles = true;
                    break;
                }
            }
        }
        return obstacles;
    }
}

[Serializable]
class Grid {

    public Slot[,] slots;

    public Grid(int width, int length, Transform[] objects) {
        if (width == 0 || length == 0) Debug.LogWarning("Set width and height");
        // TODO, create slots, assign them actions
        slots = new Slot[width, length];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                slots[i, j] = new Slot(Map.defaultSeaAction);
                int index = j * width + i;
                slots[i, j].reference = objects[index];
            }
        }
    }
}
class Slot {
    // What to do when something is above it, or arrives on it.
    public MapAction onStay;

    /// <summary>
    /// Reference to instance.
    /// </summary>
    internal Transform reference;

    /// <summary>
    /// Which ship/rock/obstacle occupies this slot.
    /// </summary>
    internal MonoBehaviour target;

    MapAction onEnter;

    //MapAction onLeave;

    public Slot(params MapAction[] actions) {
        onStay = actions[0];
    }
}