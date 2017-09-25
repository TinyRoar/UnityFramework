using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine;
using UnityEngine.UI;

public class DeactivateButtonOnLayer : MonoBehaviour {

    public LayerConfig DeactivateOnLayer;
    private Button btn;

	void Awake ()
    {
        btn = GetComponent<Button>();
        Events.Instance.OnLayerChange += LayerChange;
	}

    private void LayerChange(Layer layer, UIAction action)
    {
        if (layer == LayerManager.Instance.GetLayerEntry(DeactivateOnLayer).Layer)
        {
            if (action == UIAction.Show)
                btn.interactable = false;

            if (action == UIAction.Hide)
            {
                btn.interactable = true;
            }
        }
    }
}
