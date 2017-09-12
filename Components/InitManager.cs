using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using System.Threading;
using UnityEngine.Events;

namespace TinyRoar.Framework
{
    public class InitManager : MonoSingleton<InitManager>
    {
        [SerializeField]
        private SaveMethod SaveMethod = SaveMethod.BinaryCrypt;

        [SerializeField]
        private int FPS = 60;

        [SerializeField]
        private GameplayStatus DefaultGameplayStatus = GameplayStatus.None;

        [SerializeField]
        private GameEnvironment DefaultEnvironment;

        [SerializeField]
        public List<LayerEntry> LayerEntries;

        public static bool StaticDebug = false;
        public bool Debug = false;

        public bool SoundDebug = false;

        public bool UseAnalytics = false;

        public bool CloseAtEsc = false;

        public static Thread MainThread = null;

        public bool UseViewManager;

        public UnityEvent AfterInitialized;

        protected override void Awake()
        {
            base.Awake();

            StaticDebug = Debug;

            // set FPS + disable vsync
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = FPS;

            // set encrytion method
            if (SaveMethod != SaveMethod.None)
            {
                GameConfig.Instance.UseSaveMethod = SaveMethod;
            }

            // if android set to binary crypt because xml is not working at the moment on android
            if (Application.platform == RuntimePlatform.Android)
            {
                GameConfig.Instance.UseSaveMethod = SaveMethod.BinaryCrypt;
            }

            MainThread = System.Threading.Thread.CurrentThread;

            // initialize Encrypt once to prevent first usage is not on main thread / deviceID would throw exception
            new Encrypt();

        }

        void Start()
        {
            UIManager.Instance.Init();

            // show layers
            ShowLayerEntries();

            Inputs.Instance.OnKeyDown += OnKeyDown;

            if (UseViewManager)
                ViewManager.Instance.Init();

            AfterInitialized.Invoke();
        }

        /// <summary>
        /// show some layers by default if needed
        /// </summary>
        private void ShowLayerEntries()
        {
            // execute next frame
            Updater.Instance.ExecuteNextFrame(delegate
            {
                Events.Instance.GameplayStatus = DefaultGameplayStatus;
                UIManager.Instance.Switch(DefaultEnvironment, 0);

                if (LayerEntries.Count == 0)
                    return;

                int count = LayerEntries.Count;
                for (var i = 0; i < count; i++)
                {
                    LayerEntry layerEntry = LayerEntries[i];

                    if (layerEntry.LayerConfig == null || layerEntry.Action == UIAction.None)
                        continue;

                    UIManager.Instance.Switch(layerEntry.LayerConfig, layerEntry.Action);
                }
            });
        }

        private void OnKeyDown()
        {
            if (Input.GetKeyDown("escape") && CloseAtEsc)
                Application.Quit();

            if (Input.GetKeyDown(KeyCode.F1) && Application.isEditor)
                Time.timeScale = 0.25f;

            if (Input.GetKeyDown(KeyCode.F2) && Application.isEditor)
                Time.timeScale = 1f;

            if (Input.GetKeyDown(KeyCode.F3) && Application.isEditor)
                Time.timeScale = 100f;
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            //Does it work to do in BaseManagement instead of here?
            DataManagement.Instance.ForceSaving();
        }

    }
}
