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
        [SerializeField]
        private UIAction reactToAction;

        private Button button
        {
            get;
            set;
        }

        void Start()
        {
            if(reactToLayer == Layer.None)
                return;

            Events.Instance.OnLayerChange += LayerChange;
            button = this.GetComponent<Button>();
        }

        void OnDestroy()
        {
            Events.Instance.OnLayerChange -= LayerChange;
        }

        void LayerChange(Layer layer, UIAction action)
        {
            if (layer == reactToLayer && action == reactToAction)
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
