//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class Statistician : Role
//{
//    public Statistician() : base()
//    {
//        name = "Statistician";
//    }

//    public override void EndNight(Controller player, Controller newInfectedPlayer)
//    {

//        // sometimes method is called twice???
//        try
//        {
//        //Put some code here
//        }
//        catch
//        {

//        }
//    }

//    public override void OnClick(Controller player)
//    {
//        RaycastHit2D currentBed = Physics2D.Raycast(player.transform.position, Vector2.down, 0.1f, (int)Layers.bed);
//        player.movement.frozen = true;
//        Debug.Log(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().GetInfected());
//        gameObjects.Add(currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().gameObject);
//        player.resultsScreen.SetActive(true);
//        player.resultsScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Testing " +
//            currentBed.collider.gameObject.GetComponent<BedScript>().getPlayer().view.Owner.NickName;
//    }

//    public override void StartNight(Controller player)
//    {
//        Debug.Log("startNight()");
//        player.roleText.GetComponent<Animator>().SetTrigger("Display");
//        player.roleText.GetComponentInChildren<TextMeshProUGUI>().text = "Role: Statistician - Pick a player to check for infection";

//    }
//}