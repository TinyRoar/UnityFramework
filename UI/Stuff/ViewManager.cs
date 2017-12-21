using UnityEngine;
using TinyRoar.Framework;

namespace TinyRoar.Framework
{
    public sealed class ViewManager : Singleton<ViewManager>
    {
        public void Init()
        {
            Events.Instance.OnLayerChange -= OnLayerChange;
            Events.Instance.OnLayerChange += OnLayerChange;
            if(InitManager.StaticDebug)
                Debug.Log("ViewManager init");
        }

        void OnDestroy()
        {
            Events.Instance.OnLayerChange -= OnLayerChange;
        }

        private void OnLayerChange(Layer layer, UIAction action)
        {

            // get LayerEntry by layer
            LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);

            if (layerEntry.View == null)
            {
                if(InitManager.StaticDebug)
                    Debug.LogWarning("View" + layer.ToString() + " not found :'(");
                    return;
            }

            if (action == UIAction.Show)
                layerEntry.View.onShow();
            else
                layerEntry.View.onHide();

        }
    }

}