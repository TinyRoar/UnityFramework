using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakeScreenshots : EditorWindow
{
    private Vector2[] resolutions =
    {

        new Vector2(2048,2732),
                new Vector2(1242,2208),
                    new Vector2(640,960),


                        new Vector2(1080,1920),


    };

    private Vector2 _size = new Vector2(1920, 800);
    private Vector2 _pos = new Vector2(0, 10);

    private string filename = "Screenshot";

    private float m_LastEditorUpdateTime;
    private bool takeScreenshot = false;

    private float timer = 0;

    private bool pauseGame = false;
    private int resIndex = 0;

    [MenuItem("Tiny Roar/Screenshooter...")]
    static void Init()
    {
        MakeScreenshots window = (MakeScreenshots)(EditorWindow.GetWindow(typeof(MakeScreenshots)));
    } // Init()

    public static EditorWindow GetMainGameView()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        return (EditorWindow)Res;
    } // GetMainGameView()

    protected virtual void OnEnable()
    {
        #if UNITY_EDITOR
                m_LastEditorUpdateTime = Time.realtimeSinceStartup;
                EditorApplication.update += OnEditorUpdate;
        #endif
    }

    protected virtual void OnDisable()
    {
        #if UNITY_EDITOR
                EditorApplication.update -= OnEditorUpdate;
            takeScreenshot = false;
        #endif
    }

    private bool canvasSet = false;

    private void OnEditorUpdate()
    {

        Debug.Log(Time.timeScale);

        if (takeScreenshot)
        {
            timer = Time.realtimeSinceStartup - m_LastEditorUpdateTime;

            if (timer >= 2f && canvasSet)
            {
                foreach (Canvas canvas in GameObject.FindObjectsOfType<Canvas>())
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                }

                Debug.Log("set canvas");
                Debug.Log("saving " + filename + "_" + resolutions[resIndex].x + "x" + resolutions[resIndex].y + ".png");

                // make screenshot for resolution
                Application.CaptureScreenshot(filename + "_" + resolutions[resIndex].x + "x" + resolutions[resIndex].y + ".png", 4);
                canvasSet = false;
            }

            if (timer >= 4f)
            {
                if (resIndex < resolutions.Length-1)
                {
                    resIndex++;

                    SetWindowSize((int)resolutions[resIndex].x, (int)resolutions[resIndex].y);
                    m_LastEditorUpdateTime = Time.realtimeSinceStartup;

                    //takeScreenshot = false;
                }
                else
                {
                    takeScreenshot = false;
                }
            }
        }

        if (pauseGame)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }   



    void SetWindowSize(int width, int height)
    {
        foreach (Canvas canvas in GameObject.FindObjectsOfType<Canvas>())
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
        }

        EditorWindow gameView = GetMainGameView();
        Rect pos = gameView.position;

        gameView.maxSize = new Vector2(5000, 5000);

        pos.x = 0;
        pos.y = 0;
        pos.width = width / 4;
        pos.height = height / 4 + 17;
        gameView.position = pos;
        //gameView.ShowUtility();
        gameView.Repaint();
        canvasSet = true;
    }

    void OnGUI()
    {
        filename = EditorGUILayout.TextField("Filename", filename);

        EditorGUILayout.LabelField("Force Pause: " + pauseGame.ToString());

        if (GUILayout.Button("Pause Game"))
        {
            pauseGame = !pauseGame;
        }

        if (GUILayout.Button("Shoot"))
        {
            //reset timer
            m_LastEditorUpdateTime = Time.realtimeSinceStartup;

            resIndex = 0;
            SetWindowSize((int)resolutions[resIndex].x, (int)resolutions[resIndex].y);
            takeScreenshot = true;
        }

        if (takeScreenshot)
            EditorGUILayout.LabelField("Taking screenshots");

    }

}