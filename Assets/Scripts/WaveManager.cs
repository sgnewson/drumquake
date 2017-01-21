﻿using System.Collections;
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
	public Text HitResultUI;
	public GameObject WavePrefabLocal;
	public List<Wave> WaveList;
	public GameObject ReferenceWave;
	public Material redCircle;
	public Material blueCircle;
	public Material greenCircle;
	public Material yellowCircle;

	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep};
	public LerpMode TransitionLerpMode;

	void Awake() {
		WaveList = new List<Wave> ();
		WaveScoreUI = GameObject.Find("WaveScoreUI").GetComponent<Text>();

		InvokeRepeating ("SpawnWave", waveSpawnDelay, waveStartInterval);
	}

	void SpawnWave() {
		XboxButton waveType;

		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefabLocal, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		newWaveGameObject.SetActive (true);

		if (WaveCount % 4 == 0) {
			waveType = XboxButton.A;
			newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = greenCircle;
		} else if (WaveCount % 3 == 0) {
			waveType = XboxButton.B;
			newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = redCircle;
		} else if (WaveCount % 2 == 0) {
			waveType = XboxButton.X;
			newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = blueCircle;
		} else {
			waveType = XboxButton.Y;
			newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = yellowCircle;
		}
			
		Wave newWave = new Wave (waveType, waveStartRadius, newWaveGameObject);
		WaveList.Add (newWave);

		WaveCount++;
	}

	void Update() {

		List<Wave> WavesToKeep = new List<Wave> ();

		foreach (Wave wave in WaveList) {
			if (wave.Percentage > 0.1f) {
				wave.Percentage -= waveShrinkRate * Time.deltaTime;
				wave.WavePrefab.transform.localScale = new Vector3 (wave.Percentage, wave.Percentage, wave.Percentage);
				WavesToKeep.Add (wave);
			} else {
				if (!wave.AttemptedHit) {
					WaveIgnored ();
				}
				GameObject.Destroy (wave.WavePrefab);
			}
		}
			
		WaveList = WavesToKeep;
		WaveScore = Mathf.Max(0, WaveScore);

//		Debug.Log ("Wave Score: " + WaveScore);
		WaveScoreUI.text = "Wave Score: " + WaveScore;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		Wave wave = WaveList [0];
		Debug.Log ("percentage: " + wave.Percentage);

		float delta = Mathf.Abs (0.2f - wave.Percentage);

		if (wave.Button != buttonPressed) {
			WaveIncorrect ();
			return;
		}

		if (delta < 0.03f) {
			WaveHit (buttonPressed);
		} else if (delta < 0.07f) {
			WaveClose (buttonPressed);
		} else {
			WaveMiss (buttonPressed);
		}
		wave.AttemptedHit = true;
			
	}

	private void  WaveHit (XboxButton buttonPressed)
	{
		string resultText = "HIT :) " + buttonPressed;
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.green;
		WaveScore += 10;

		DisplayHitResult (resultText);
	}

	private void  WaveClose (XboxButton buttonPressed)
	{
		string resultText = "CLOSE :| " + buttonPressed;
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.yellow;
		WaveScore += 2;

		DisplayHitResult (resultText);
	}

	private void  WaveMiss (XboxButton buttonPressed)
	{
		string resultText = "MISS :( " + buttonPressed;
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
		WaveScore -= 5;

		DisplayHitResult (resultText);
	}

	private void  WaveIgnored ()
	{
		string resultText = "IGNORED :( ";
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
		WaveScore -= 2;

		DisplayHitResult (resultText);
	}

	private void  WaveIncorrect ()
	{
		string resultText = "INCORRECT!! ";
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
		WaveScore -= 7;

		DisplayHitResult (resultText);
	}

	private void DisplayHitResult (string resultText)
	{
		Debug.Log (resultText);
		HitResultUI.text = resultText;
	}

}
