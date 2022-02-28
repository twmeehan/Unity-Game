using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Alien : Role
{
    public Alien() : base()
    {
        name = "Alien";
    }
    public override void CalculateButtonType(Controller player)
    {

        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, (int) Layers.bed);
        if (currentBed.collider != null && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != player
            && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != null && gameObjects.Count == 0)
        {

            player.interact.button.interactable = true;
            player.interact.buttonState = (int)Buttons.infect;
            //button.image.sprite=...

        }
        else
        {
            player.interact.DisableButton();
        }

        // may throw error on first night beacuse timer is not yet set
        try
        {
            if (player.timer.TimeRemaining() <= 0)
            {

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                object[] content = new object[] { this.gameObjects[0].GetComponent<PhotonView>().Owner.UserId };
                PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);

            }
        }
        catch { }

    }

    public override void EndNight(Controller player, Controller newInfectedPlayer)
    {
        newInfectedPlayer.SetInfected(true);
        gameObjects.Clear();

    }

    public override void OnClick(Controller player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, (int)Layers.bed);
        player.movement.frozen = true;
        gameObjects.Add(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().gameObject);

    }

    public override void StartNight(Controller player)
    {
        player.roleText.GetComponent<Animator>().SetTrigger("Display");
        player.roleText.GetComponentInChildren<TextMeshProUGUI>().text = "Role: Alien - Pick a player to infect";
    }
}
