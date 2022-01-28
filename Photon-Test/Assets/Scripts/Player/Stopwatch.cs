using UnityEngine;

/*
 * Class Timer - owned by Timer.cs in order to start a stopwatch
 * clock. Has all the methods to create a clock that counts
 * upwards until paused or reset.
 */
public class Stopwatch
{

    #region public variables

    // time currently on the stopwatch
    public float time = 0;

    public bool paused = true;

    #endregion

    // Constructor Stopwatch() - creates Stopwatch that begins running immediately
    public Stopwatch()
    {
        time = 0;
        paused = false;
    }

    // Constructor Stopwatch() - creates Stopwatch object
    public Stopwatch(bool paused)
    {
        time = 0;
        this.paused = paused;
    }

    // Method Update() - because Stopwatch does not extend MonoBehavior 
    // Update() MUST BE CALLED MANUELLY
    public void Update()
    {
        if (!paused)
            time += Time.deltaTime;
    }

    // Method Set() - sets to specified time
    public void Set(float time)
    {
        this.time = time;
    }

    // Method Reset() - sets the clock to 0 and unpauses it
    public void Reset()
    {
        time = 0;
        paused = false;
    }

    // Method Pause() - stops the time from increasing
    public void Pause()
    {
        paused = true;
    }

    // Method Start() - allows the time to continue increasing
    public void Start()
    {
        paused = false;
    }

    // Method ToString() - returns the time on the stopwatch in -:-- form
    public override string ToString()
    {
        return Mathf.FloorToInt(time / 60)
            + ":" + (time % 60).ToString("00");
    }
}
