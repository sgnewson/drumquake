using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave {

	public float Percentage;
	public GameObject WavePrefab;

	public Wave(float percentage, GameObject prefab) {
		Percentage = percentage;
		WavePrefab = prefab;
	}
}
