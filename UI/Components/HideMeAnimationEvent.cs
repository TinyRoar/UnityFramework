using UnityEngine;
using System.Collections;

public class HideMeAnimationEvent : MonoBehaviour
{

    public void HideMe()
    {
        string layerName = this.name;
        Layer layer = LayerEntry.StringToLayer(layerName);
        UIManager.Instance.Switch(layer, UIAction.Hide);
    }

}
