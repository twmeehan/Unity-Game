using UnityEngine;
using UnityEngine.UI;

/* 
 * Class Interact() - attached to a gameObject that has a Button to control
 * what the button does based on what the player is near 
 */
public class Interact : MonoBehaviour
{

    [Space(10)]
    [Header("Required")]
    // transform of the player's character
    public Transform t;
    public Controller controller;

    [Space(10)]
    [Header("Not Required")]
    public int buttonState = 0;

    // button in lower right hand corner of player's screen
    public Button button;

    // Method Start() - called during the first frame to get reference to button
    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
    }

    // Method Update() - runs every frame and checks for interactable objects during the day
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

            // if this bed no longer recognizes this player as its sleeper then the player is kicked out
            // such as when another player sleeps in this same bed
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
    public void DisableButton()
    {

        button.interactable = false;
        buttonState = (int)Buttons.disabled;

    }
    // Method KickFromBed() - wakes up this character (should ONLY be called by line 49) 
    private void KickFromBed()
    {
        // only run on controller's client

        controller.SetSleeping(false);
        controller.movement.frozen = false;

    }

    // Method TouchingBed() - runs if player is touching a bed to enable the "sleep/wakeup" button
    private void TouchingBed()
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

    // Method TouchingHealingMachine() - runs if player is touching a healing machine to enable the "heal" button
    private void TouchingHealingMachine()
    {

        if (!controller.GetSleeping())
        {


             button.interactable = true;
             buttonState = (int) Buttons.heal;

        }
    }

    // Method ButtonPressed() - called when button is clicked
    public void ButtonPressed()
    {

        // check the button's current state and map it to its appropriate action
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
                controller.role.OnClick(controller);
                break;
        }
    }

    // Method WakeUp() - called when the player presses the button to leave bed
    public void WakeUp()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(t.position, Vector2.up, 0.1f, (int) Layers.bed);
        currentBed.collider.gameObject.GetComponent<BedScript>().LeaveBed();

        controller.movement.frozen = true;

    }

    // Method Heal() - called when the player clicks the button while near the healing pod
    public void Heal()
    {
        RaycastHit2D heal = Physics2D.Raycast(t.position, Vector2.up, 0.1f, (int) Layers.healing);
        heal.collider.gameObject.GetComponent<HealingMachineScript>().enterHealingMachine(controller);
    }

    // Method Sleep() - called when the player presses the button to enter bed
    public void Sleep()
    {

        RaycastHit2D currentBed = Physics2D.Raycast(t.position, Vector2.down, 0.1f, (int) Layers.bed);
        currentBed.collider.gameObject.GetComponent<BedScript>().EnterBed(controller);

    }

}
