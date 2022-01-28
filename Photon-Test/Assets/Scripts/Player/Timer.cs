using Photon.Pun;
using TMPro;
using UnityEngine;

/*
 * Class Timer - attach to a Text(TMP) object to make the text
 * into a clock. Timer contains the methods to start a networked
 * timer and also contains a Stopwatch clock. User can choose
 * to display the timer, stopwatch, or neither on the Text(TMP) object.
 * Other classes may use this script in order to execute code
 * after a set amount of time.
 */
public class Timer : MonoBehaviour
{
    #region private variables

    // the PhotonNetwork.Time when the timer starts
    private double startingTime = 0;
    // the number of seconds on the clock when the timer starts
    private float startingSeconds = 0;
    // is true if the timer still has time left
    private bool running = false;
    // reference to the text that is counting down (or up)
    private TextMeshProUGUI clock;
    #endregion

    /* choose: 
     * None - Clock shows nothing
     * Timer - Clock shows the coundown timer
     * Stopwatch - Clock shows the increasing stopwatch.time
     */
    public enum clocks {None,Timer,Stopwatch};
    [Tooltip("Choose which clock to display")]
    public clocks display;

    public Stopwatch stopwatch = new Stopwatch();

    // Method Start() - runs at the start of the game to obtain reference to Text(TMP) clock
    private void Start()
    {
        clock = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Method Update() - runs at the start of every frame
    void Update()
    {

        // Stopwatch does not extend Monobehavior so Update() must be called manually
        stopwatch.Update();

        // If timer runs out Timer.running must be false
        if (running && TimeRemaining() < 0)
            running = false;

        try
        {

            // display time on clock
            if (display == clocks.Timer)
                clock.text = this.ToString();
            else if (display == clocks.Stopwatch)
                clock.text = stopwatch.ToString();
            else
                clock.text = "";

        } catch {

        }

    }

    // Method IsRunning() - returns whether countdown timer still has time left
    public bool IsRunning()
    {
        return running;
    }

    // Method TimeRemaining() - returns the remaining time on the timer
    public float TimeRemaining()
    {
        return startingSeconds - (float) (PhotonNetwork.Time - startingTime);
    }

    // Method SetTimer() - start a timer with specified number of seconds.
    // startTime must be the PhotonNetwork.Time that you want the clock to start from
    public void SetTimer(float seconds, double startTime)
    {
        this.startingSeconds = seconds;
        this.startingTime = startTime;
        running = true;
    }

    // Method ToString() - returns the remaining time on the timer in -:-- form
    public override string ToString()
    {
        if (running)
            return Mathf.FloorToInt(Mathf.FloorToInt(TimeRemaining()) / 60)
                    + ":" + (Mathf.FloorToInt(TimeRemaining()) % 60).ToString("00");
        return "0:00";
    }
}
