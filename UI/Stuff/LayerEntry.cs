using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class LayerEntry
{

    public Layer Layer;
    public UIAction Action;
    public GameObject GameObject
    {
        get;
        set;
    }
    public LayerConfig LayerConfig
    {
        get;
        set;
    }

    public LayerEntry(string layerName, GameObject gameObject)
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
        LayerConfig = gameObject.GetComponent<LayerConfig>();
    }

    public static Layer StringToLayer(string layerName)
    {
        try
        {
            return (Layer)Enum.Parse(typeof(Layer), layerName);

        }
        catch (Exception e)
        {
            Debug.LogWarning("Layer " + layerName + " not set in ENUM");
        }
        return global::Layer.None;
    }


}
