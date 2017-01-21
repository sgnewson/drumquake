using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

	public GameObject WavePrefab;
	public float DelayBetweenWaves;
	public List<Wave> WaveList;

	public float WaveDurationSeconds;
	float currentTime;

	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep}
	public LerpMode TransitionLerpMode;

	void Awake() {
		WaveList = new List<Wave> ();
		StartCoroutine ("SpawnWave");
	}

	IEnumerator SpawnWave() {
		yield return new WaitForSeconds (DelayBetweenWaves);

		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		Wave newWave = new Wave (1f, newWaveGameObject);

		WaveList.Add (newWave);

		StartCoroutine ("SpawnWave");
	}

	void Update() {

		foreach (Wave wave in WaveList) {
			float per = wave.Percentage;
			if (per <= 0f) {
				// Wave is in the middle so destroy it

			} else {
				wave.Percentage -= Time.deltaTime;
				wave.WavePrefab.transform.localScale = new Vector3 (wave.Percentage, wave.Percentage, wave.Percentage);
			}
		}

	}
}
