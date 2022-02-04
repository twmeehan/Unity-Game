/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alien : Role
{
    public Alien() : base()
    {
        name = "Alien";
    }
    public override void checkForInteractables(PlayerScript player)
    {

        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, player.layers.bedLayer);
        if (currentBed.collider != null && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != player
            && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != null)
        {

            player.Objects.button.interactable = true;
            player.setButtonType(4);
            //button.image.sprite=...

        }
        else
        {
            player.Objects.button.interactable = false;
            player.setButtonType(0);
        }
    }

    public override void endNight(PlayerScript player, PlayerScript newInfectedPlayer)
    {
        gameObjects.Clear();

    }

    public override void onClick(PlayerScript player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, player.layers.bedLayer);
        player.setFrozen(true);
        gameObjects.Add(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().gameObject);

    }

    public override void startNight(PlayerScript player)
    {
        player.Objects.roleDisplay.GetComponent<Animator>().SetTrigger("Display");
        player.Objects.roleDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "Role: Alien - Pick a player to infect";
    }
}
*/