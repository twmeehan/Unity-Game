using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Role : MonoBehaviour
{

    public string name;
    public Role(string name)
    {
        this.name = name;
    }
    public abstract void checkForInteractables(PlayerScript player);
    public abstract void startNight(PlayerScript player);
    public abstract void onClick(PlayerScript player);
    public abstract void endNight(PlayerScript player, PlayerScript newInfectedPlayer);

}
