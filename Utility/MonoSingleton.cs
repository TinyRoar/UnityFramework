using System;
using UnityEngine;

namespace TinyRoar.Framework
{

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T _instance;
        private static bool _isApplicationExit = false;

        /// <summary>
        /// get / set Instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_isApplicationExit)
                    return null;

                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T);
                    Debug.LogWarning(singleton.name + " created");
                }

                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }

        private static void CheckMultipleScripts()
        {
            var isMainThread = false;
            if (InitManager.MainThread != null)
                isMainThread = InitManager.MainThread.Equals(System.Threading.Thread.CurrentThread);
            if (!isMainThread)
                return;

            var objectCount = FindObjectsOfType<T>().Length;
            if (objectCount <= 1)
                return;

            var typeOfAction = typeof(T);
            Debug.LogError(objectCount + " GameObjects with script '" + typeOfAction.Name + "' in scene!!!");
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        protected virtual void Awake()
        {
            if (_instance == null)
                _instance = this as T;
            _isApplicationExit = false;

            if (InitManager.StaticDebug)
                CheckMultipleScripts();
        }


        /// <summary>
        /// Kill instance at OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            _instance = null;
        }

        /// <summary>
        /// Destroy at ApplicationQuit
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            _isApplicationExit = true;
        }

    }

}