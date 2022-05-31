using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Info : MonoBehaviour
{

    private TextMeshProUGUI text;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        text = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.down, 0.1f, (int)Layers.player))
        {
            text.text = "Player \nName: " + Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                Vector2.down, 0.1f, (int)Layers.player).collider.gameObject.
                GetComponent<Controller>().view.Owner.NickName + "\nSleeping: " +
                Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                Vector2.down, 0.1f, (int)Layers.player).collider.gameObject.
                GetComponent<Controller>().GetSleeping().ToString() + "\nDay: " +
                Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                Vector2.down, 0.1f, (int)Layers.player).collider.gameObject.
                GetComponent<Controller>().GetDay().ToString() + "\nInfected: " +
                Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                Vector2.down, 0.1f, (int)Layers.player).collider.gameObject.
                GetComponent<Controller>().GetInfected().ToString() + "\nState: " +
                Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                Vector2.down, 0.1f, (int)Layers.player).collider.gameObject.
                GetComponent<Controller>().transitionState.ToString();
        }
        else if (Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.down, 0.1f, (int)Layers.collider))
        {
            try
            {
                text.text = "Bed \nPlayer: " + Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.down, 0.1f, (int)Layers.collider).collider.gameObject.
                    GetComponent<BedScript>().getPlayer().view.Owner.NickName.ToString();
            } catch
            {
                text.text = "Bed \nPlayer: Null";
            }
        }
        else
        {
            text.text = "null";
        }
    }
}
