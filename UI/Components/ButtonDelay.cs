using UnityEngine;
using UnityEngine.UI;
using TinyRoar.Framework;

namespace TinyRoar.Framework
{
    public class ButtonDelay : MonoBehaviour
    {
        [SerializeField]
        private float time = 1.0f;
        [SerializeField]
        private Layer reactToLayer;

        private Button button
        {
            get;
            set;
        }

        void Start()
        {
            if(reactToLayer == Layer.None)
                return;

            //Inputs.Instance.OnLayerChange += LayerChange;
            button = this.GetComponent<Button>();
        }

        void OnDestroy()
        {
            //Inputs.Instance.OnLayerChange -= LayerChange;
        }

        void LayerChange(Layer oldLayer, Layer newLayer)
        {
            if (newLayer == reactToLayer)
            {
                // disable
                button.interactable = false;

                // enable after some time
                Timer.Instance.Add(time, delegate
                {
                    button.interactable = true;
                });
            }
        }

    }
}
