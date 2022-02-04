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

    public List<BedScript> getBeds()
    {
        return beds;
    }
    public List<Controller> getPlayers()
    {
        List<Controller> players = new List<Controller>();
        foreach (BedScript bed in beds)
        {
            if (bed.player != null)
            {
                players.Add(bed.getPlayer().GetComponent<Controller>());
            }
        }
        return players;
    }
}
