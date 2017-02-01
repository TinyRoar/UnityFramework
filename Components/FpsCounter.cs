using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TinyRoar.Framework;
//using TinyRoar.TinyLytics;
using System;

/*
 * Add this Component to an Object with Text-component
 * For de/activating in runtime, use:
 *   - FpsCounter.Instance.Activated = true/false;
 * For debug: Press "G" Key for de/activating FpsCounter
 */
public class FpsCounter : MonoSingleton<FpsCounter>
{
    public float updateInterval = 1.0f; // Set interval in Seconds (how often update text of fps-value)
    public string fpsText = "fps";

    private float accum; // FPS accumulated over the interval
    private int frames; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private Text textObj;

    private bool _activated = true;
    public bool Activated
    {
        get
        {
            return _activated;
        }
        set
        {
            if (_activated != value)
            {
                Updater.Instance.OnUpdate -= UpdateTimer;
                if (_activated)
                    Updater.Instance.OnUpdate += UpdateTimer;
            }
            _activated = value;
        }
    }

    void Start()
    {
        textObj = this.GetComponent<Text>();
        if (!textObj)
        {
            Debug.LogWarning("FpsCounter Object or Component not found!");
            Activated = false;
        }

        if (!Activated)
            return;

        Updater.Instance.OnUpdate += UpdateTimer;
    }

    public override void OnDestroy()
    {
        //Updater.Instance.OnUpdate -= UpdateTimer;
    }

    void UpdateTimer()
    {

        if (Input.GetKeyDown(KeyCode.G))
            this.Activated = !this.Activated;

        if (!this.Activated)
            return;

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        frames++;

        // Interval ended - update GUI text and start new interval
        if (timeleft > 0)
            return;

        // display two fractional digits (f2 format)
        float fps = accum / frames;
        string format = System.String.Format("{0:F1} " + fpsText, fps );
        textObj.text = format;

        // show colored
        if (fps < 10)
            textObj.color = Color.red;
        else if (fps < 30)
            textObj.color = Color.yellow;
        else if (fps < 60)
            textObj.color = new Color(0, 0.6f, 0);
        else
            textObj.color = new Color(0, 0.4f, 0);
        timeleft = updateInterval;
        accum = 0.0F;
        frames = 0;

        //// Analytics
        //if(fps < 25 && TinyLytics.Instance.IsRunning)
        //    TinyLytics.Instance.GameplayEvent("fps", Convert.ToInt32(fps));

    }

}
