using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class Wave {

	public XboxButton Button;
	public float Percentage;
	public GameObject WavePrefab;
	public bool AttemptedHit;

	public Wave(XboxButton button, float percentage, GameObject prefab) {
		Button = button;
		Percentage = percentage;
		WavePrefab = prefab;
		AttemptedHit = false;
	}
}
