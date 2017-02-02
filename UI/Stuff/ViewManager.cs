using UnityEngine;
using TinyRoar.Framework;

public sealed class ViewManager : Singleton<ViewManager>
{
    public void Init()
    {
        Events.Instance.OnLayerChange -= OnLayerChange;
        Events.Instance.OnLayerChange += OnLayerChange;
        Debug.Log("ViewManager init");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Events.Instance.OnLayerChange -= OnLayerChange;
    }

    private void OnLayerChange(Layer layer, UIAction action)
    {

        // get LayerEntry by layer
        LayerEntry layerEntry = LayerManager.Instance.GetLayerEntry(layer);

        if (layerEntry.View == null)
        {
            Debug.LogError("Layer " + layer.ToString() + ": View not found :'(");
            return;
        }

        if(action == UIAction.Show)
            layerEntry.View.onShow();
        else
            layerEntry.View.onHide();

    }
}
