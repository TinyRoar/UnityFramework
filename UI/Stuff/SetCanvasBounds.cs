using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SetCanvasBounds : MonoBehaviour {

	private Rect lastSafeArea = new Rect(0, 0, 0, 0);

	void ApplySafeArea(Rect area)
	{
		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;

		RectTransform rectTransform = GetComponent<RectTransform> ();
		rectTransform.anchorMin = anchorMin;
		rectTransform.anchorMax = anchorMax;

		lastSafeArea = area;
	}

	void Update ()
	{
		#if UNITY_IOS
		Rect safeArea = Screen.safeArea;

		if (safeArea != lastSafeArea) {
			ApplySafeArea (safeArea);
	
		}
		#endif
	}
}
