using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FailingMedStudent : Role
{
    public FailingMedStudent() : base()
    {
        name = "FailingMedStudent";
    }


    public override void CalculateButtonType(Controller player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, (int)Layers.bed);
        if (currentBed.collider != null && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != player
            && currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer() != null && gameObjects.Count == 0)
        {

            player.interact.button.interactable = true;
            player.interact.buttonState = (int)Buttons.test;
            //button.image.sprite=...

        }
        else
        {
            player.interact.DisableButton();
        }
    }

    public override void EndNight(Controller player, Controller newInfectedPlayer)
    {

        // sometimes method is called twice???
        try
        {

            newInfectedPlayer.SetInfected(true);
            if (gameObjects[0].GetComponent<Controller>().GetInfected())
            {
                player.resultsScreen.GetComponentInChildren<TextMeshProUGUI>().text =
                    gameObjects[0].GetComponent<Controller>().view.Owner.NickName + " is not infected";
            }
            else
            {
                player.resultsScreen.GetComponentInChildren<TextMeshProUGUI>().text =
                    gameObjects[0].GetComponent<Controller>().view.Owner.NickName + " is infected";
            }
            gameObjects.Clear();
        }
        catch
        {

        }

    }

    public override void OnClick(Controller player)
    {
        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, (int)Layers.bed);
        player.movement.frozen = true;
        Debug.Log(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().GetInfected());
        gameObjects.Add(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().gameObject);
        player.resultsScreen.SetActive(true);
        player.resultsScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Testing " +
            currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().view.Owner.NickName;
    }

    public override void StartNight(Controller player)
    {
        Debug.Log("startNight()");
        player.roleText.GetComponent<Animator>().SetTrigger("Display");
        player.roleText.GetComponentInChildren<TextMeshProUGUI>().text = "Role: MedStudent - Pick a player to check for infection";

    }
}
