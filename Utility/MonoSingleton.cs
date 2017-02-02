using System;
using UnityEngine;

namespace TinyRoar.Framework
{

    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T _instance;
        private static bool isApplicationExit = false;

        /// <summary>
        /// get / set Instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (isApplicationExit)
                    return null;

                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                bool isMainThread = false;
                if(InitManager.MainThread != null)
                    isMainThread = InitManager.MainThread.Equals(System.Threading.Thread.CurrentThread);
                if (isMainThread && FindObjectsOfType<T>().Length > 1)
                {
                    Type typeOfAction = typeof(T);
                    Debug.LogError(+FindObjectsOfType<T>().Length + " GameObjects with script '" + typeOfAction.Name + "' in scene!!!");
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T).ToString();
                    Debug.LogWarning(singleton.name + " created");
                }

                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }

        /// <summary>
        /// Initialisation
        /// </summary>
        public virtual void Awake ()
        {
	        if (_instance == null)
                _instance = this as T;

            isApplicationExit = false;
        }


        /// <summary>
        /// Destroy at ApplicationQuit
        /// </summary>
        public virtual void OnDestroy()
        {
            isApplicationExit = true;
            _instance = null;
        }

    }

}