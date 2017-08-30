using System.Collections;
using System.Collections.Generic;
using TinyRoar.Framework;
using UnityEngine;
using UnityEngine.UI;

public class DeactivateButtonOnLayer : MonoBehaviour {

    public LayerConfig DeactivateOnLayer;
    private Button btn;

	void Start ()
    {
        btn = GetComponent<Button>();
        Events.Instance.OnLayerChange += LayerChange;
	}

    private void LayerChange(Layer layer, UIAction action)
    {
        if (layer == LayerManager.Instance.GetLayerEntry(DeactivateOnLayer).Layer)
        {
            // make sure button is enabled
            btn.enabled = true;

            if (action == UIAction.Show)
                btn.interactable = false;

            if (action == UIAction.Hide)
                btn.interactable = true;
        }
    }
}
