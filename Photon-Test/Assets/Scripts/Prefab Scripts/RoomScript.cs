using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomScript : MonoBehaviour
{

    private List<BedScript> beds = new List<BedScript>();
    public LayerMask roomLayer;

    // Start is called before the first frame update
    void Start()
    {
        foreach (BedScript bed in FindObjectsOfType<BedScript>())
        {
            Debug.Log("bed found");
            try
            {
                if (Physics2D.Raycast(bed.gameObject.transform.position, Vector2.up, 0.1f, roomLayer).collider.gameObject == this.gameObject)
                {
                    Debug.Log("Bed added");
                    beds.Add(bed);
                }
            } catch
            {

            }
            
        }
    }

    public List<BedScript> GetBeds()
    {
        return beds;
    }
    public List<Controller> GetPlayers()
    {
        List<Controller> players = new List<Controller>();
        foreach (BedScript bed in beds)
        {
            // doesn't get a player if bed is empty
            try
            {
                players.Add(bed.getPlayer().GetComponent<Controller>());
            } catch
            {

            }
        }
        return players;
    }
}
