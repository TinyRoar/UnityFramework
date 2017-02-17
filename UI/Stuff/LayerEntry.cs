using System;
using UnityEngine;
using System.Collections;

namespace TinyRoar.Framework
{
    [Serializable]
    public class LayerEntry
    {

        public Layer Layer{ get; set; }
        public LayerConfig LayerConfig;
        public UIAction Action;
        public GameObject GameObject { get; set; }
        public View View { get; set; }

        public LayerEntry(string layerName, GameObject gameObject, LayerConfig layerConfig)
        {
            var layer = StringToLayer(layerName);

            if (layer == Layer.None)
            {
                Debug.LogWarning("Layer " + layerName + " not found in ENUM :'(");
                return;
            }

            Layer = layer;
            Action = UIAction.Hide;
            GameObject = gameObject;
            LayerConfig = layerConfig;
            View = gameObject.GetComponent<View>();
        }

        public static Layer StringToLayer(string layerName)
        {
            try
            {
                return (Layer) Enum.Parse(typeof (Layer), layerName);

            }
            catch (Exception e)
            {
                Debug.LogWarning("Layer " + layerName + " not set in ENUM");
            }
            return global::Layer.None;
        }


    }
}
