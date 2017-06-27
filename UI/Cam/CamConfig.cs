using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

namespace TinyRoar.Framework
{
    public class CamConfig : MonoSingleton<CamConfig>
    {

        [SerializeField] private bool Enabled = true;

        //public CamMovement CamMovement { get; set; }

        void Start()
        {
            if (Enabled)
            {
                OnChange();
            }

            /*
            CamMovement camMovement = this.GetComponent<CamMovement>();
            if (camMovement != null)
            {
                camMovement.DoDisable();
            }*/

            //CamMovement = this.GetComponent<CamMovement>();

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
