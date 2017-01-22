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
	const int earthquakeTimerStartCount = 60;

	public float HeartBeatInMilliseconds = 300f;
	int beatCount;

	private float WaveScore = 0f;
	private int WaveCount = 0;
	private int EarthquakeTimerCount = earthquakeTimerStartCount;

	public Text EarthquakeTimerUI;
	public Text WaveScoreUI;
	public Text HitResultUI;

	public GameObject WavePrefabLocal;
	public List<Wave> WaveList;
	public GameObject ReferenceWave;
	public MoveCamera MainCamera;

	public Material redCircle;
	public Material blueCircle;
	public Material greenCircle;
	public Material yellowCircle;

	Dictionary<XboxButton, AudioStuff> ColorMap;

	public Dictionary<int, DrumPattern> AllPatterns;
	public DrumPattern EasyPattern;
	DrumPattern CurrentPattern;

	Dictionary<XboxButton, AudioStuff> clipForButton;

	[Range (0f, 1f)] public float MissPitchMultiplier;
	[Range (0f, 1f)] public float MissVolumeMultiplier;

	[Range (0f, 1f)] public float ClosePitchMultiplier;
	[Range (0f, 1f)] public float CloseVolumeMultiplier;

	[Range (0f, 1f)] public float HitPitchMultiplier;
	[Range (0f, 1f)] public float HitVolumeMultiplier;

	public AudioClip RedSound;
	public AudioClip YellowSound;
	public AudioClip BlueSound;
	public AudioClip GreenSound;

	[Range (0f, 1f)] public float baseRedVolume;
	[Range (0f, 1f)] public float baseYellowVolume;
	[Range (0f, 1f)] public float baseBlueVolume;
	[Range (0f, 1f)] public float baseGreenVolume;

	[Range (-3f, 3f)] public float basePitch;

	[Range (0f, 2f)] public float speedMultiplier;

	AudioSource audioSource;

	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep};
	public LerpMode TransitionLerpMode;

	void Awake() {
		ColorMap = new Dictionary<XboxButton, AudioStuff> ();
		ColorMap.Add (XboxButton.A, new AudioStuff(GreenSound, baseGreenVolume, greenCircle));
		ColorMap.Add (XboxButton.B, new AudioStuff(RedSound, baseRedVolume, redCircle));
		ColorMap.Add (XboxButton.X, new AudioStuff(BlueSound, baseBlueVolume, blueCircle));
		ColorMap.Add (XboxButton.Y, new AudioStuff(YellowSound, baseYellowVolume, yellowCircle));

		audioSource = this.GetComponent<AudioSource> ();
		WaveList = new List<Wave> ();

		SetupPatterns ();

		InvokeRepeating ("Heartbeat", 0f, HeartBeatInMilliseconds / 1000f);

		InvokeRepeating ("EarthquakeCountdown", 0f, 1f);
		beatCount = 0;
	}

	void Heartbeat() {
		beatCount++;

		Debug.Log ("Beat count: " + beatCount);

		if (CurrentPattern.Dict.ContainsKey (beatCount) == true) {
			SpawnWave (CurrentPattern.Dict [beatCount]);
		}
	}

	void SpawnWave(XboxButton waveType) {

		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefabLocal, new Vector3 (0f, 0f, 0f), Quaternion.identity);
		newWaveGameObject.SetActive (true);

		newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = ColorMap[waveType].circle;
			
		Wave newWave = new Wave (waveType, waveStartRadius, newWaveGameObject);
		WaveList.Add (newWave);
	}

	void EarthquakeCountdown() {
		if (EarthquakeTimerCount <= 0) {
			MainCamera.Shake (WaveScore / 1000);
			EarthquakeTimerCount = earthquakeTimerStartCount;
		} else {
			EarthquakeTimerCount--;
		}
		EarthquakeTimerUI.text = "Time to quake: " + EarthquakeTimerCount;
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

		WaveScoreUI.text = "Wave Score: " + WaveScore;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		if (WaveList.Count == 0) {
			WaveMiss (buttonPressed);
			return;
		}

		Wave wave = WaveList [0];
//		Debug.Log ("percentage: " + wave.Percentage);

		float delta = Mathf.Abs (0.2f - wave.Percentage);

		if (wave.Button != buttonPressed) {
			WaveIncorrect (buttonPressed);
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
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * HitVolumeMultiplier;
		audioSource.pitch = basePitch * HitPitchMultiplier;
		audioSource.Play ();

		string resultText = "HIT :) " + buttonPressed;
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.green;
		WaveScore += 10;

		DisplayHitResult (resultText);
	}

	private void  WaveClose (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * CloseVolumeMultiplier;
		audioSource.pitch = basePitch * ClosePitchMultiplier;
		audioSource.Play ();

		string resultText = "CLOSE :| " + buttonPressed;
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.yellow;
		WaveScore += 2;

		DisplayHitResult (resultText);
	}

	private void  WaveMiss (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

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

	private void  WaveIncorrect (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

		string resultText = "INCORRECT!! ";
		ReferenceWave.GetComponent<Renderer> ().material.color = Color.red;
		WaveScore -= 7;

		DisplayHitResult (resultText);
	}

	private void DisplayHitResult (string resultText)
	{
//		Debug.Log (resultText);
		HitResultUI.text = resultText;
	}

	void SetupPatterns() {
		AllPatterns = new Dictionary<int, DrumPattern> ();

		EasyPattern = new DrumPattern ();
		EasyPattern.Dict.Add (8, XboxButton.A);
		EasyPattern.Dict.Add (16, XboxButton.A);
		EasyPattern.Dict.Add (24, XboxButton.A);
		EasyPattern.Dict.Add (32, XboxButton.A);

		CurrentPattern = EasyPattern;
	}

	public class AudioStuff {
		public AudioClip thisClip;
		public float Volume;
		public Material circle;

		public AudioStuff(AudioClip c, float v, Material circle) {
			thisClip = c;
			Volume = v;
			this.circle = circle;
		}
	}

	public class DrumPattern {
		public Dictionary<int, XboxButton> Dict;

		public DrumPattern() {
			Dict = new Dictionary<int, XboxButton>();
		}
	}

}