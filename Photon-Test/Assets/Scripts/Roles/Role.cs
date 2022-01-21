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
    public abstract void checkForInteractables(PlayerScript player);
    public abstract void startNight(PlayerScript player);
    public abstract void onClick(PlayerScript player);
    public abstract void endNight(PlayerScript player, PlayerScript newInfectedPlayer);

}
