using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using System.Threading;

public class InitManager : MonoSingleton<InitManager>
{
    [SerializeField]
    private SaveMethod SaveMethod = SaveMethod.BinaryCrypt;

    [SerializeField]
    private int FPS = 30;

    [SerializeField]
    private GameplayStatus DefaultGameplayStatus = GameplayStatus.None;

    [SerializeField]
    private GameEnvironment DefaultEnvironment;

    [SerializeField]
    public List<LayerEntry> LayerEntries;

    public bool Debug = false;

    public bool UseAnalytics = false;

    public bool CloseAtEsc = false;

    public static Thread MainThread = null;



    public override void Awake()
    {
        base.Awake();

        // set encrytion method
        if (SaveMethod != SaveMethod.None)
        {
            GameConfig.Instance.UseSaveMethod = SaveMethod;
        }

        MainThread = System.Threading.Thread.CurrentThread;

    }

    void Start ()
    {
        UIManager.Instance.Init();

        // set FPS
        Application.targetFrameRate = FPS;

        // execute next frame
        Updater.Instance.ExecuteNextFrame(delegate 
        {
            Events.GameplayStatus = DefaultGameplayStatus;
            UIManager.Instance.Switch(DefaultEnvironment, 0);

            int count = LayerEntries.Count;
            for (var i = 0; i < count; i++)
            {
                LayerEntry layerEntry = LayerEntries[i];

                if (layerEntry.Layer == Layer.None || layerEntry.Action == UIAction.None)
                    continue;

                UIManager.Instance.Switch(layerEntry.Layer, layerEntry.Action);
            }
        });

        if(CloseAtEsc)
            Inputs.Instance.OnKeyDown += OnKeyDown;  
    }

    private void OnKeyDown()
    {
        if (Input.GetKeyDown("escape"))
            Application.Quit();
    }

    void OnApplicationQuit()
    {

        DataManagement.Instance.ForceSaving();
        //TileManagement.Instance.ForceSaving();
        //GroveManagement.Instance.ForceSaving();

        // Analytics
        Analytics.Instance.UserEvent("EndedAfterSeconds", Time.time.ToString());

    }

}
