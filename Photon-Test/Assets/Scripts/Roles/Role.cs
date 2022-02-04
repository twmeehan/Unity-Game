using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Role : MonoBehaviour
{

    public string name;
    public List<GameObject> gameObjects = new List<GameObject>();
    public Role()
    {
        this.name = name;
    }
    public abstract void checkForInteractables(Controller player);
    public abstract void startNight(Controller player);
    public abstract void onClick(Controller player);
    public abstract void endNight(Controller player, Controller newInfectedPlayer);

}
