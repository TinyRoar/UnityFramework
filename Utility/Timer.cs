using TinyRoar.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * TIMER
 * About: (c) 2015 by Tiny Roar | Dario D.Müller
 * Desc: Timer is threaded and event is fired in the next frame in Unity's Update loop
 * Usage: Timer.Instance.Add(1.0f, () => { Debug.Log("Timer ended"); });
 */
namespace TinyRoar.Framework
{
    public class Timer : Singleton<Timer>
    {
        /// <summary>
        /// list with timer-actions
        /// </summary>
        private Dictionary<int, Action> _endTimer { get; set; }

        /// <summary>
        /// internal id for identifying the timer
        /// </summary>
        private int nextTimerId;

        /// <summary>
        /// flag if a nextFrame Update is signedIn
        /// </summary>
        private static bool isUpdate;

        /// <summary>
        /// list of index-of-actions which timer is ended and should be fired in next frame
        /// </summary>
        private static List<int> expiredIndexList;

        /// <summary>
        /// Constructor
        /// </summary>
        public Timer()
        {
            _endTimer = new Dictionary<int, Action>();
            isUpdate = false;
            expiredIndexList = new List<int>();
        }

        /// <summary>
        /// Start a new Timer
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <param name="endEvent">delegate event that should be called after timer is over</param>
        /// <returns>timerID - used if you want to stop the timer</returns>
        public int Add(float time, Action endEvent)
        {
            if (time == 0)
            {
                endEvent();
                return -1;
            }

            // insert into list
            this._endTimer.Add(nextTimerId, endEvent);

            // Setup timer
            System.Timers.Timer aTimer = new System.Timers.Timer(time * 1000);
            int timerID = new Int32();
            timerID = nextTimerId;
            aTimer.Elapsed += (sender, args) => KeepAliveElapsed(sender, timerID);
            aTimer.Start();

            nextTimerId++;
            return timerID;
        }

        /// <summary>
        /// Stop the timer by giving it's timerID
        /// </summary>
        /// <param name="timerID">timerID that was returned after Add() method</param>
        /// <returns>Status if timer was stopped or not</returns>
        public bool Stop(int timerID)
        {
            if (!_endTimer.ContainsKey(timerID))
                return false;

            _endTimer.Remove(timerID);
            return true;
        }

        /// <summary>
        /// Callback from System.Timer used to call the custom callback event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="timerIndex"></param>
        public static void KeepAliveElapsed(object source, int timerIndex)
        {
            // stop timer
            ((System.Timers.Timer)source).Stop();

            // add index to list of expired timer
            expiredIndexList.Add(timerIndex);

            // check is update-method is not already signed in
            if (isUpdate)
                return;

            // enable Update method
            isUpdate = true;
            Updater.Instance.ExecuteNextFrame(DoUpdate);

        }

        /// <summary>
        /// Custom callback event will be fired in the next frame
        /// </summary>
        static void DoUpdate()
        {
            // disable Update variable
            isUpdate = false;

            // loop
            foreach (int timerIndex in expiredIndexList.ToList())
            {
                // get action
                Action action = null;
                Timer.Instance._endTimer.TryGetValue(timerIndex, out action);

                // check is action is valid
                if (action != null)
                {
                    // exec TimerEnd event
                    action();

                    // insert into list
                    Timer.Instance._endTimer.Remove(timerIndex);
                }
                // clear value of expired timer
                expiredIndexList.Remove(timerIndex);
            }

        }

    }
}
