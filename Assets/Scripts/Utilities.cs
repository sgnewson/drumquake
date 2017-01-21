using UnityEngine;
using System.Collections;

public static class Utilities {

	// min and max are the input range, a and b are the range you want your result to be within
	public static float ValueToRange(float x, float min, float max, float a, float b) {
		float r = (((b - a) * (x - min)) / (max - min)) + a;
		return r;
	}

	public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
	{
		Vector2 diference = vec2 - vec1;
		float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
		return Vector2.Angle(Vector2.right, diference) * sign;
	}

	/*
		 * Returns the angle between two vectors on a plane with the specified normal
		 * */
	public static float GetOrthogonalAngle(Vector3 v1, Vector3 v2, Vector3 normal) {
		Vector3.OrthoNormalize(ref normal, ref v1);
		Vector3.OrthoNormalize(ref normal, ref v2);
		return Vector3.Angle(v1, v2);
	}

	//Breadth-first search
	public static Transform FindDeepChild(this Transform aParent, string aName)
	{
		var result = aParent.Find(aName);
		if (result != null)
			return result;
		foreach(Transform child in aParent)
		{
			result = child.FindDeepChild(aName);
			if (result != null)
				return result;
		}
		return null;
	}

}
