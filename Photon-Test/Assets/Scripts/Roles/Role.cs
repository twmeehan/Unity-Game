using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class Role - abstract class for each of the character roles (ex. alien, doctor, etc.)
 * has all abstract functions needed to preform night actions. Each character's Controller has a
 * Role object that can be set to any of the roles. Controller then uses the abstract methods to
 * preform night time actions.
 */
public abstract class Role : MonoBehaviour
{

    // name of role
    public string name;

    // used to store any information that the player collects during each night
    public List<GameObject> gameObjects = new List<GameObject>();

    // name of each role will be set in child constructor
    public Role()
    {

    }

    // Method CalculateButtonType() - runs every frame during the night to check if player is near a interactable object
    public abstract void CalculateButtonType(Controller player);

    // Method StartNight() - runs at the beginning of each night
    public abstract void StartNight(Controller player);

    // Method OnClick() - runs when the player presses the button
    public abstract void OnClick(Controller player);

    // Method EndNight() - runs at the end of the night to display results
    public abstract void EndNight(Controller player, Controller newInfectedPlayer);

}
