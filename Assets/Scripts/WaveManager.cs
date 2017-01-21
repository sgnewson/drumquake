using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {

	const float waveStartInterval = 1.0f;
	const float waveSpawnDelay = 0.5f;
	const float waveStartRadius = 1f;
	const float waveShrinkRate = 0.5f;

	private float WaveScore = 0f;
	private int WaveCount = 0;

	public Text WaveScoreUI;
	public GameObject WavePrefab;
	public List<Wave> WaveList;
	public GameObject ReferenceWave;

	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep};
	public LerpMode TransitionLerpMode;

	void Awake() {
		WaveList = new List<Wave> ();
		WaveScoreUI = GameObject.Find("WaveScoreUI").GetComponent<Text>();

		InvokeRepeating ("SpawnWave", waveSpawnDelay, waveStartInterval);
	}

	void SpawnWave() {
		XboxButton waveType;

		if (WaveCount % 2 == 0) {
			waveType = XboxButton.A;
		} else {
			waveType = XboxButton.B;
		}

		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefab, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		Wave newWave = new Wave (waveType, waveStartRadius, newWaveGameObject);
		WaveList.Add (newWave);

		WaveCount++;
	}

	void Update() {

		List<Wave> WavesToKeep = new List<Wave> ();

		foreach (Wave wave in WaveList) {
			if (wave.Percentage > 0.05f) {
				wave.Percentage -= waveShrinkRate * Time.deltaTime;
				wave.WavePrefab.transform.localScale = new Vector3 (wave.Percentage, wave.Percentage, wave.Percentage);
				WavesToKeep.Add (wave);
			} else {
				GameObject.Destroy (wave.WavePrefab);
				WaveScore = Mathf.Max(0, WaveScore - 2);
			}
		}
			
		WaveList = WavesToKeep;

		Debug.Log ("Wave Score: " + WaveScore);
		WaveScoreUI.text = "Wave Score: " + WaveScore;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		Wave wave = WaveList [0];
		Debug.Log ("percentage: " + wave.Percentage);

		float delta = Mathf.Abs (0.2f - wave.Percentage);

		if (delta < 0.05f) {
			Debug.Log ("HIT " + buttonPressed);
			ReferenceWave.GetComponent<Renderer> ().material.color = Color.green;
			WaveScore += 10;
		} else if (delta < 0.10f) {
			Debug.Log ("CLOSE " + buttonPressed);
			ReferenceWave.GetComponent<Renderer> ().material.color = Color.yellow;
			WaveScore += 2;
		} else {
			Debug.Log ("MISS " + buttonPressed);
			ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
			WaveScore -= 5;
		}
	}
}
