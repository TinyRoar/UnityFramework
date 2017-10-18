using System;
using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

namespace TinyRoar.Framework
{
    public class CamConfig : MonoBehaviour
    {
        [SerializeField]
        private bool IsEnabled = true;

        private static Action<bool> _enableEvent;

        public static void SetEnable(bool status)
        {
            if (_enableEvent != null)
                _enableEvent(status);
        }

        private static GameObject _activeCamConfig;
        public static GameObject GetActive()
        {
            return _activeCamConfig;
        }

        void Awake()
        {
            _enableEvent += Enable;
        }

        void Start()
        {
            OnChange();
        }

        void OnEnable()
        {
            _activeCamConfig = this.gameObject;
        }

        private void OnChange()
        {
            ICam[] comps = this.GetComponents<ICam>();
            foreach (var comp in comps)
            {
                if (IsEnabled)
                    comp.DoEnable();
                else
                    comp.DoDisable();
            }
        }

        public void Enable(bool status)
        {
            if (IsEnabled == status)
                return;
            IsEnabled = status;
            OnChange();
        }

    }
}
