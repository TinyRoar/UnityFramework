using UnityEngine;

// Original by https://github.com/Znoey/UnityFramework
public class Math {
 
	public static float Lerp (float from, float to, float value) {
		if (value < 0.0f)
			return from;
		else if (value > 1.0f)
			return to;
		return (to - from) * value + from;
	}
 
	public static float LerpUnclamped (float from, float to, float value) {
		return (1.0f - value)*from + value*to;
	}
 
	public static float InverseLerp (float from, float to, float value) {
		if (from < to) {
			if (value < from)
				return 0.0f;
			else if (value > to)
				return 1.0f;
		}
		else {
			if (value < to)
				return 1.0f;
			else if (value > from)
				return 0.0f;
		}
		return (value - from) / (to - from);
	}
 
	public static float InverseLerpUnclamped (float from, float to, float value) {
		return (value - from) / (to - from);
	}
 
	public static float SmoothStep (float from, float to, float value) {
		if (value < 0.0f)
			return from;
		else if (value > 1.0f)
			return to;
		value = value*value*(3.0f - 2.0f*value);
		return (1.0f - value)*from + value*to;
	}
 
	public static float SmoothStepUnclamped (float from, float to, float value) {
		value = value*value*(3.0f - 2.0f*value);
		return (1.0f - value)*from + value*to;
	}
 
	public static float SuperLerp (float from, float to, float from2, float to2, float value) {
		if (from2 < to2) {
			if (value < from2)
				value = from2;
			else if (value > to2)
				value = to2;
		}
		else {
			if (value < to2)
				value = to2;
			else if (value > from2)
				value = from2;	
		}
		return (to - from) * ((value - from2) / (to2 - from2)) + from;
	}
 
	public static float SuperLerpUnclamped (float from, float to, float from2, float to2, float value) {
		return (to - from) * ((value - from2) / (to2 - from2)) + from;
	}
 
	public static Color ColorLerp (Color c1, Color c2, float value) {
		if (value > 1.0f)
			return c2;
		else if (value < 0.0f)
			return c1;
		return new Color (	c1.r + (c2.r - c1.r)*value, 
							c1.g + (c2.g - c1.g)*value, 
							c1.b + (c2.b - c1.b)*value, 
							c1.a + (c2.a - c1.a)*value );
	}
 
	public static Vector2 Vector2Lerp (Vector2 v1, Vector2 v2, float value) {
		if (value > 1.0f)
			return v2;
		else if (value < 0.0f)
			return v1;
		return new Vector2 (v1.x + (v2.x - v1.x)*value, 
							v1.y + (v2.y - v1.y)*value );		
	}
 
	public static Vector3 Vector3Lerp (Vector3 v1, Vector3 v2, float value) {
		if (value > 1.0f)
			return v2;
		else if (value < 0.0f)
			return v1;
		return new Vector3 (v1.x + (v2.x - v1.x)*value, 
							v1.y + (v2.y - v1.y)*value, 
							v1.z + (v2.z - v1.z)*value );
	}
 
	public static Vector4 Vector4Lerp (Vector4 v1, Vector4 v2, float value) {
		if (value > 1.0f)
			return v2;
		else if (value < 0.0f)
			return v1;
		return new Vector4 (v1.x + (v2.x - v1.x)*value, 
							v1.y + (v2.y - v1.y)*value, 
							v1.z + (v2.z - v1.z)*value,
							v1.w + (v2.w - v1.w)*value );
	}
 
}