using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace TinyRoar.Framework
{
    public class Inputs : Singleton<Inputs>
    {

        // private
        private Vector2 _leftMousePos;
        private Vector2 _rightMousePos;

        // Key Events
        public delegate void InputHandler();
        public event InputHandler OnKey;
        public event InputHandler OnKeyDown;
        public event InputHandler OnLeftMouseDown;
        public event InputHandler OnLeftMouseMove;
        public event InputHandler OnLeftMouseMoveLate;
        public event InputHandler OnLeftMouseUp;
        public event InputHandler OnRightMouseDown;
        public event InputHandler OnRightMouseMove;
        public event InputHandler OnRightMouseUp;
        public event InputHandler OnTouchMove;

        public delegate void KeyUpHandler(KeyCode key);
        public event KeyUpHandler OnKeyUp;

        private List<KeyCode> keyList = new List<KeyCode>();
        private KeyCode[] specialKeys;
        private bool useSpecialKeys = true;

        public Inputs()
        {
            Updater.Instance.OnUpdate += DoUpdate;
            Updater.Instance.OnLateUpdate += DoLateUpdate;
            _instance = this;

            specialKeys = new KeyCode[] {
                        /*KeyCode.UpArrow,
                        KeyCode.DownArrow,
                        KeyCode.LeftArrow,
                        KeyCode.RightArrow,
                        KeyCode.LeftControl,
                        KeyCode.LeftShift,
                        KeyCode.LeftAlt,*/
                        KeyCode.Escape
                    };
        }

        void OnDestroy()
        {
            Updater.Instance.OnUpdate -= DoUpdate;
            Updater.Instance.OnUpdate -= DoLateUpdate;
        }

        void DoUpdate()
        {

            // Check if new Key was pressed, Special Keys are not possible with these check
            if (useSpecialKeys == true)
            {
                string str = Input.inputString;
                if (str.Trim() != "")
                {
                    str = str.ToUpper();
                    foreach (char c in str)
                    {
                        try
                        {
                            KeyCode key = (KeyCode)System.Enum.Parse(typeof(KeyCode), c.ToString());
                            if (!keyList.Contains(key))
                            {
                                keyList.Add(key);
                            }
                        }
                        catch
                        {
                            // Key not an input
                        }
                    }
                }
            }

            // Check Key still pressed
            if (OnKey != null)
                if (Input.anyKey)
                    OnKey();

            // Check Key down
            if (OnKeyDown != null)
                if (Input.anyKeyDown)
                    OnKeyDown();

            // Check Key Up
            for (int i = keyList.Count - 1; i >= 0; i--)
            {
                KeyCode key = keyList[i];
                if (!Input.GetKey(key))
                {
                    if (OnKeyUp != null)
                    {
                        OnKeyUp(key);
                    }
                    keyList.Remove(key);
                }
            }

            // Check Arrow Key and Special Key Up
            if (useSpecialKeys == true)
            {
                foreach (var key in specialKeys)
                {
                    if (Input.GetKeyUp(key))
                    {
                        if (OnKeyUp != null)
                        {
                            OnKeyUp(key);
                        }
                    }
                }
            }

            // Left Mouse Down, Move, Up
            if (OnLeftMouseDown != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    OnLeftMouseDown();
                    _leftMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            if (OnLeftMouseMove != null)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 curPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (Vector2.Distance(curPos, _leftMousePos) >= 1)
                    {
                        _leftMousePos = curPos;
                        OnLeftMouseMove();
                    }
                }
            }

            if (OnLeftMouseUp != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnLeftMouseUp();
                }
            }

            // Right Mouse Down, Move, Up
            if (OnRightMouseDown != null)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    OnRightMouseDown();
                    _rightMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }

            if (OnRightMouseMove != null)
            {
                if (Input.GetMouseButton(1))
                {
                    Vector2 curPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (Vector2.Distance(curPos, _rightMousePos) >= 1)
                    {
                        _rightMousePos = curPos;
                        OnRightMouseMove();
                    }
                }
            }

            if (OnRightMouseUp != null)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    OnRightMouseUp();
                }
            }

            if(OnTouchMove != null)
            {
                if (Input.touchCount >= 1 && (Input.GetTouch(0).phase == TouchPhase.Moved || (Input.touchCount >= 2 && Input.GetTouch(1).phase == TouchPhase.Moved)))
                {
                    OnTouchMove();
                }
            }

        }

        void DoLateUpdate()
        {

            if (OnLeftMouseMoveLate != null)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 curPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (Vector2.Distance(curPos, _leftMousePos) >= 1)
                    {
                        _leftMousePos = curPos;
                        OnLeftMouseMoveLate();
                    }
                }
            }

        }

    }


}