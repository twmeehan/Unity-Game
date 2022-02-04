using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 
 * Class Interact() - attached to a gameObject that has a Button to control
 * what the button does based on what the player is near 
 */
public class Interact : MonoBehaviour
{

    private int buttonState = 0;

    private Button button;

    // transform of the player's character
    public Transform t;

    public Controller controller;

    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
    }

    void Update()
    {

        // if the time is day then the player can interact with objects
        if (controller.GetDay())
            CalculateButtonType();
    }

    // Method CalculateButtonType() - called by Update() every frame to check which object the player
    // is near which changes the buttonState
    public bool CalculateButtonType()
    {
        RaycastHit2D bed = Physics2D.Raycast(t.position, Vector2.down, 0.1f, (int) Layers.bed);
        RaycastHit2D heal = Physics2D.Raycast(t.position, Vector2.down, 0.1f, (int) Layers.healing);

        // if touching a bed run TouchingBed()
        if (bed.collider != null)
        {
            if (!bed.collider.gameObject.GetComponent<BedScript>().player.Equals(controller.view.Owner.UserId))
                KickFromBed();

            TouchingBed();
            return true;

        }

        // if touching the healing pod run TouchingHealingMachine()
        else if (heal.collider != null && heal.collider.GetComponent<HealingMachineScript>().available)
        {

            TouchingHealingMachine();
            return true;

        }

        // if touching nothing then disable button
        else
        {

            button.interactable = false;
            buttonState = (int) Buttons.disabled;

        }

        return false;

    }
    public void TouchingBed()
    {

        // if player is touching a bed but is awake
        if (!controller.GetSleeping())
        {

            // button allows user to enter bed
             button.interactable = true;
             buttonState = (int) Buttons.getIntoBed;

        }

        // if player is touching a bed but is sleeping
        else
        {

            // button allows user to leave bed
            button.interactable = true;
            buttonState = (int)Buttons.leaveBed;

        }

    }

    public void TouchingHealingMachine()
    {

        if (!controller.GetSleeping())
        {


             button.interactable = true;
             buttonState = (int) Buttons.heal;

        }
    }
    public void ButtonPressed()
    {
        switch (buttonState)
        {
            case 0:
                Debug.Log("Error");
                break;
            case 1:
                Sleep();
                break;
            case 2:
                WakeUp();
                break;
            case 3:
                Heal();
                break;
            case 4:
                controller.role.onClick(controller);
                break;
        }
    }
    [PunRPC]
    public void JoinBedRPC(object[] objectArray)
    {

        controller.SetSleeping(true);
        if (controller.view.IsMine)
            controller.movement.frozen = true;

        // move player to the center of bed
        t.position = new Vector2((float)objectArray[0], (float)objectArray[1]);

    }
    public void JoinBed(Transform t)
    {
        Debug.Log("Sending RPC");
        object[] objectArray = { t.position.x, t.position.y };
        controller.view.RPC("JoinBedRPC", RpcTarget.All, objectArray as object);

    }
    public void KickFromBed()
    {
        // only run on controller's client

        controller.SetSleeping(false);
        controller.movement.frozen = false;
    }

    // Called when the player presses the button to leave bed
    public void WakeUp()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(t.position, Vector2.up, 0.1f, (int) Layers.bed);
        currentBed.collider.gameObject.GetComponent<BedScript>().LeaveBed();

        controller.movement.frozen = true;

    }

    public void Heal()
    {
        RaycastHit2D heal = Physics2D.Raycast(t.position, Vector2.up, 0.1f, (int) Layers.healing);
        heal.collider.gameObject.GetComponent<HealingMachineScript>().enterHealingMachine(controller);
    }

    // Called when the player presses the button to enter bed
    public void Sleep()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(t.position, Vector2.down, 0.1f, (int) Layers.bed);
        currentBed.collider.gameObject.GetComponent<BedScript>().EnterBed(controller);

    }
}
