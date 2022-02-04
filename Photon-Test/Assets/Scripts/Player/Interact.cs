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
    public Transform transfrom;

    void Start()
    {
        button = this.gameObject.GetComponent<Button>();
    }

    void Update()
    {
        
    }

    // Method CalculateButtonType() - called by Update() every frame to check which object the player
    // is near which changes the buttonState
    public bool CalculateButtonType()
    {
        RaycastHit2D bed = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, (int) Layers.bed);
        RaycastHit2D heal = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, (int) Layers.healing);

        // if touching a bed run TouchingBed()
        if (bed.collider != null)
        {

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

        if (!sleeping)
        {

            RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layers.bedLayer);
            if (currentBed.collider != null && day)
            {

                Objects.button.interactable = true;
                buttonType = GET_INTO_BED;
                //button.image.sprite=...

            }

        }
        else if (day) // runs if sleeping
        {

            RaycastHit2D currentBed = Physics2D.Raycast(transform.position, Vector2.up, 0.1f, layers.bedLayer);
            if (currentBed.collider != null && !currentBed.collider.gameObject.GetComponent<BedScript>().player.Equals(this.gameObject.GetComponent<PhotonView>().Owner.UserId))
                KickFromBed();
        }
    }

    public void TouchingHealingMachine()
    {

        if (!sleeping)
        {

            RaycastHit2D nearHealingMachine = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layers.HealingMachine);
            if (nearHealingMachine.collider != null && day)
            {

                Objects.button.interactable = true;
                buttonType = Heal_Self;
                //button.image.sprite=...

            }
        }
    }
}
