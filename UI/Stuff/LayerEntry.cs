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

        public LayerEntry(string layerName, Transform transform, LayerConfig layerConfig)
        {
            if (transform == null)
            {
                var msg = "Layer " + layerName + " hasn't a child named 'Container' :'(";
                throw new Exception(msg);
            }

            if (layerConfig == null)
            {
                var msg = "Layer " + layerName + " hasn't a 'LayerConfig' Script on it :'(";
                throw new Exception(msg);
            }

            var layer = StringToLayer(layerName);
            if (layer == Layer.None)
            {
                var msg = "Layer " + layerName + " not found in ENUM :'(";
                throw new Exception(msg);
            }

            Layer = layer;
            Action = UIAction.Hide;
            GameObject = transform.gameObject;
            LayerConfig = layerConfig;
            View = transform.transform.parent.GetComponent<View>();
        }

    internal T GetView<T>() where T : class
    {
      try
      {
        return View as T;
      }
      catch (InvalidCastException)
      {
        return default(T);
      }
    }

    public static Layer StringToLayer(string layerName)
        {
            try
            {
                return (Layer) Enum.Parse(typeof (Layer), layerName);

            }
            catch (Exception)
            {
                Debug.LogWarning("Layer " + layerName + " not set in ENUM");
            }
            return global::Layer.None;
        }


    }
}
