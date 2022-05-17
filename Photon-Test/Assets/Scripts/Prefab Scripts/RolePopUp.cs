using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.Dark;

public class RolePopUp : MonoBehaviour
{

    private bool displaying = true;
    private UIDissolveEffect dissolve;
    public UIDissolveEffect blackDissolve;

    private Stopwatch stopwatch;

    public float fadeInSpeed = 1;
    public float fadeOutSpeed = 1;
    public float delay = 0;
    public GameObject canvas;
    public int role = -1;
    public GameObject witch;
    public GameObject gloomling;
    public GameObject miner;

    // Start is called before the first frame update
    void Start()
    {
        
        stopwatch = new Stopwatch();
        stopwatch.Reset();
        dissolve = this.gameObject.GetComponent<UIDissolveEffect>();
    }

    // Update is called once per frame
    void Update()
    {

        if (role != -1)
        {
            if (role == 0)
            {
                witch.SetActive(true);
                gloomling.SetActive(false);
                miner.SetActive(false);
            }
            else if (role == 1)
            {
                witch.SetActive(false);
                gloomling.SetActive(true);
                miner.SetActive(false);
            }
            else if (role == 2)
            {
                witch.SetActive(false);
                gloomling.SetActive(false);
                miner.SetActive(true);
            }
            stopwatch.Update();

            if (stopwatch.time > delay && displaying && dissolve.location > 0)
            {
                dissolve.location -= Time.deltaTime * fadeInSpeed;
                blackDissolve.location += Time.deltaTime * fadeInSpeed;

            }
            else if (!displaying && dissolve.location < 1)
                dissolve.location += Time.deltaTime * fadeOutSpeed;
            else if (!displaying)
                Destroy(canvas);

        }

    }

    public void Click()
    {
        displaying = false;
        foreach(Controller player in (Controller[])FindObjectsOfType(typeof(Controller))) {
            if (player.view.IsMine)
            {
                player.movement.showingRole = false;
            }
        }

    }
}
