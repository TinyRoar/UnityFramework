using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetDragThreshold : MonoBehaviour {

	void Awake ()
    {
        GetComponent<EventSystem>().pixelDragThreshold = Screen.width / 30;
    }
}
