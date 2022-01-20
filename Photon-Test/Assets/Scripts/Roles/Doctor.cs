using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Doctor : Role
{
    public Doctor(string name) : base(name)
    {

    }

    public override void checkForInteractables(PlayerScript player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, player.layers.bedLayer);
        if (currentBed.collider != null && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != null)
        {

            player.Objects.button.interactable = true;
            player.setButtonType(4);
            //button.image.sprite=...

        } else
        {
            player.Objects.button.interactable = false;
            player.setButtonType(0);
        }
    }

    public override void endNight(PlayerScript player, PlayerScript newInfectedPlayer)
    {
        newInfectedPlayer.setInfected(true);

    }

    public override void onClick(PlayerScript player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, player.layers.bedLayer);
        player.setFrozen(true);
        Debug.Log(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().GetComponent<PlayerScript>().getInfected());
        
    }

    public override void startNight(PlayerScript player)
    {
        Debug.Log("startNight()");
        player.Objects.roleDisplay.GetComponent<Animator>().SetTrigger("Display");
        player.Objects.roleDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "Role: Doctor - Pick a player to check for infection";

    }
}
