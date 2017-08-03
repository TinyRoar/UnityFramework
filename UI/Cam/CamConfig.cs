using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

namespace TinyRoar.Framework
{
    public class CamConfig : MonoSingleton<CamConfig>
    {
        [SerializeField]
        private bool Enabled = true;

        void Start()
        {
            OnChange();
        }

        private void OnChange()
        {
            ICam[] comps = this.GetComponents<ICam>();
            foreach (var comp in comps)
            {
                if (Enabled)
                    comp.DoEnable();
                else
                    comp.DoDisable();
            }
        }

        public void SetEnabled(bool enabled)
        {
            if (Enabled == enabled)
                return;
            Enabled = enabled;
            OnChange();
        }

    }
}
