using Photon.Pun;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    #region private variables

    private TextMeshProUGUI clock;
    public float time;
    public GameObject circle;

    #endregion

    // Method Start() - runs at the start of the game to obtain reference to Text(TMP) clock
    private void Start()
    {
        clock = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Method Update() - runs at the start of every frame
    void Update()
    {

        time -= Time.deltaTime;
        clock.text = this.ToString();

    }
    public void Destroy()
    {
        Destroy(circle);
    }

    // Method ToString() - returns the remaining time on the timer in -:-- form
    public override string ToString()
    {
        if (time > 0)
            return Mathf.CeilToInt(time).ToString();
        return "R";
    }
}
