using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class WaveManager : MonoBehaviour {

	public GameObject WavePrefab;
	public float DelayBetweenWaves;
	public List<Wave> WaveList;
	public GameObject ReferenceWave;

	public float WaveDurationSeconds;
	float currentTime;

	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep};
	public LerpMode TransitionLerpMode;

	void Awake() {
		WaveList = new List<Wave> ();
		StartCoroutine ("SpawnWave");
	}

	IEnumerator SpawnWave() {
		yield return new WaitForSeconds (DelayBetweenWaves);

		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		Wave newWave = new Wave (0.5f, newWaveGameObject);

		WaveList.Add (newWave);

		StartCoroutine ("SpawnWave");
	}

	void Update() {

		List<Wave> WavesToKeep = new List<Wave> ();

		foreach (Wave wave in WaveList) {
			float per = wave.Percentage;
			if (per > 0f) {
				wave.Percentage -= Time.deltaTime;
				wave.WavePrefab.transform.localScale = new Vector3 (wave.Percentage, wave.Percentage, wave.Percentage);
				WavesToKeep.Add (wave);
			} else {
				GameObject.Destroy (wave.WavePrefab);
			}
		}
			
		WaveList = WavesToKeep;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		foreach (Wave wave in WaveList) {
			Debug.Log ("percentage: " + wave.Percentage);

			float delta = Mathf.Abs (0.2f - wave.Percentage);

			if (delta < 0.05f) {
				Debug.Log ("HIT " + buttonPressed);
				ReferenceWave.GetComponent<Renderer> ().material.color = Color.green;
			} else if (delta < 0.10f) {
				Debug.Log ("CLOSE " + buttonPressed);
				ReferenceWave.GetComponent<Renderer> ().material.color = Color.yellow;
			} else {
				Debug.Log ("MISS " + buttonPressed);
				ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
			}

		}
	}
}
