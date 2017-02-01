using TinyRoar.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * TIMER

 * About: (c) 2015 by Tiny Roar | Dario D. Müller
 * Desc: Timer is threaded and event is fired next frame in Update method
 *
 * 1. Create an event which will be notified by the timer
 *   - void EndTimerEvent() {}
 * 2. Call Timer with:
 *   - Timer.Instance.Add(1.0f, EndTimerEvent);
 */

public class Timer : Singleton<Timer>
{
    // internal
    public Dictionary<int, Action> EndTimer
    {
        get;
        private set;
    }
    private int nextTimerId;
    private static bool isUpdate = false;
    private static List<int> expiredIndexList = new List<int>();

    // Constructor
    public Timer()
    {
        EndTimer = new Dictionary<int, Action>();
    }

    void OnDestroy()
    {
        Updater.Instance.OnUpdate -= DoUpdate;
    }

    public void Add(float time, Action endEvent)
    {
        if(time == 0)
        {
            endEvent();
            return;
        }

        // insert into list
        this.EndTimer.Add(nextTimerId, endEvent);

        // Setup timer
        System.Timers.Timer aTimer = new System.Timers.Timer(time * 1000);
        int zahl = new Int32();
        zahl = nextTimerId;
        aTimer.Elapsed += (sender, args) => KeepAliveElapsed(sender, zahl);
        aTimer.Start();

        nextTimerId++;
    }

    public static void KeepAliveElapsed(object source, int timerIndex)
    {
        // stop timer
        ((System.Timers.Timer)source).Stop();

        if(isUpdate == false)
        {
            // enable Update
            isUpdate = true;
            Updater.Instance.OnUpdate += DoUpdate;
        }

        // add index to list of expired timer
        expiredIndexList.Add(timerIndex);

    }

    static void DoUpdate()
    {
        // disable Update
        isUpdate = false;
        Updater.Instance.OnUpdate -= DoUpdate;

        // loop
        foreach (int timerIndex in expiredIndexList.ToList())
        {
            // get action
            Action action = null;
            Timer.Instance.EndTimer.TryGetValue(timerIndex, out action);
            if (action != null)
            {
                // exec TimerEnd event
                action();
                // insert into list
                Timer.Instance.EndTimer.Remove(timerIndex);
            }
            // clear value of expired timer
            expiredIndexList.Remove(timerIndex);
        }

    }

}
