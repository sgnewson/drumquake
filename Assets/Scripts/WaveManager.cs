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
	const int EarthquakeTimerStartCount = 60;
	const int CutSceneTimerCount = 10;

	public float HeartBeatInMilliseconds = 300f;
	int beatCount;

	private float WaveScore = 0f;
	private int EarthquakeTimerCount = EarthquakeTimerStartCount;

	public Text EarthquakeTimerUI;
	public Text WaveScoreUI;
	public Image HitResult;

	public Sprite Perfect;
	public Sprite Good;
	public Sprite Miss;

	public Light FeedbackLight;
	float initialFeedbackLightIntensity;

	public GameObject WavePrefabLocal;
	public List<Wave> WaveList;
	public GameObject ReferenceWave;
	public Camera MainCamera;

	public Material redCircle;
	public Material blueCircle;
	public Material greenCircle;
	public Material yellowCircle;

	Dictionary<XboxButton, AudioStuff> ColorMap;

	public DrumPattern EasyPattern;
	public DrumPattern MediumPattern;
	public DrumPattern MediumHardPattern;
	public DrumPattern HardPattern;

	DrumPattern CurrentPattern;

	Dictionary<XboxButton, AudioStuff> clipForButton;

	float MissPitchMultiplier = 0.604f;
	float MissVolumeMultiplier = 0.662f;

	float ClosePitchMultiplier = 0.738f;
	float CloseVolumeMultiplier = 0.8f;

	float HitPitchMultiplier = 1f;
	float HitVolumeMultiplier = 1f;

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
		beatCount = 1;

		initialFeedbackLightIntensity = FeedbackLight.intensity;
	}

	void Heartbeat() {
		beatCount++;

		if (!GameManager.PlayOn) {
			return;
		}

		if (beatCount > 16) {
			// Check score and set the next pattern appropriately
			CurrentPattern = GetCurrentPattern();
			beatCount = 1;
		}

		//Debug.Log ("Beat count: " + beatCount);

		if (CurrentPattern.Dict.ContainsKey (beatCount) == true) {
			SpawnWave (CurrentPattern.Dict [beatCount]);
		}
	}

	DrumPattern GetCurrentPattern() {
		// use the score to determine the correct pattern and return it
		if (WaveScore < 150) {
			return EasyPattern;
		} else if (WaveScore < 300) {
			return MediumPattern;
		} else if (WaveScore < 450) {
			return MediumHardPattern;
		} else { 
			return HardPattern;
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
		EarthquakeTimerCount--;

        if (EarthquakeTimerCount <= 0)
        {
            MainCamera.GetComponent<Earthquake>().Shake(WaveScore / 1000);
			WaveScore = 0;
            EarthquakeTimerCount = EarthquakeTimerStartCount + CutSceneTimerCount;
            GameManager.PlayOn = false;
            EarthquakeTimerUI.text = "Earthquake!!!!";
        }
        else if (EarthquakeTimerCount <= EarthquakeTimerStartCount)
        {
            GameManager.PlayOn = true;
            EarthquakeTimerUI.text = "Countdown: " + EarthquakeTimerCount;
        }
	}

	void Update() {
		List<Wave> WavesToKeep = new List<Wave> ();

		foreach (Wave wave in WaveList) {
			if (wave.Percentage > 0.18f) {
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

		WaveScoreUI.text = "Magnitude: " + WaveScore/100;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		if (WaveList.Count == 0) {
			WaveMiss (buttonPressed);
			return;
		}

		Wave wave = WaveList [0];
		Debug.Log ("percentage: " + wave.Percentage);

		float delta = Mathf.Abs (0.24f - wave.Percentage);

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

		HitResult.sprite = Perfect;

		WaveScore += 10;

		IntensifyLight (2f);
	}

	private void  WaveClose (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * CloseVolumeMultiplier;
		audioSource.pitch = basePitch * ClosePitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Good;

		WaveScore += 2;

		IntensifyLight (1.3f);
	}

	private void  WaveMiss (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Miss;

		WaveScore -= 5;
	}

	private void  WaveIgnored ()
	{
		HitResult.sprite = Miss;
		WaveScore -= 2;
	}

	private void  WaveIncorrect (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Miss;
	
		WaveScore -= 7;
	}

	void SetupPatterns() {
		EasyPattern = new DrumPattern ();
		EasyPattern.Dict.Add (2, XboxButton.A);
		EasyPattern.Dict.Add (4, XboxButton.A);
		EasyPattern.Dict.Add (6, XboxButton.A);
		EasyPattern.Dict.Add (8, XboxButton.A);
		EasyPattern.Dict.Add (10, XboxButton.A);
		EasyPattern.Dict.Add (12, XboxButton.A);
		EasyPattern.Dict.Add (14, XboxButton.A);
		EasyPattern.Dict.Add (16, XboxButton.A);

		MediumPattern = new DrumPattern ();
		MediumPattern.Dict.Add (2, XboxButton.A);
		MediumPattern.Dict.Add (4, XboxButton.A);
		MediumPattern.Dict.Add (6, XboxButton.X);
		MediumPattern.Dict.Add (8, XboxButton.A);
		MediumPattern.Dict.Add (9, XboxButton.X);
		MediumPattern.Dict.Add (10, XboxButton.A);
		MediumPattern.Dict.Add (12, XboxButton.A);
		MediumPattern.Dict.Add (14, XboxButton.X);
		MediumPattern.Dict.Add (16, XboxButton.A);

		MediumHardPattern = new DrumPattern ();
		MediumHardPattern.Dict.Add (2, XboxButton.A);
		MediumHardPattern.Dict.Add (4, XboxButton.A);
		MediumHardPattern.Dict.Add (5, XboxButton.Y);
		MediumHardPattern.Dict.Add (6, XboxButton.X);
		MediumHardPattern.Dict.Add (8, XboxButton.A);
		MediumHardPattern.Dict.Add (10, XboxButton.A);
		MediumHardPattern.Dict.Add (12, XboxButton.X);
		MediumHardPattern.Dict.Add (14, XboxButton.A);

		HardPattern = new DrumPattern ();
		HardPattern.Dict.Add (2, XboxButton.A);
		HardPattern.Dict.Add (4, XboxButton.A);
		HardPattern.Dict.Add (5, XboxButton.Y);
		HardPattern.Dict.Add (6, XboxButton.X);
		HardPattern.Dict.Add (8, XboxButton.A);
		HardPattern.Dict.Add (9, XboxButton.X);
		HardPattern.Dict.Add (10, XboxButton.A);
		HardPattern.Dict.Add (11, XboxButton.Y);
		HardPattern.Dict.Add (12, XboxButton.X);
		HardPattern.Dict.Add (13, XboxButton.Y);
		HardPattern.Dict.Add (14, XboxButton.A);

		CurrentPattern = EasyPattern;
	}

	void IntensifyLight(float multiplier) {
		FeedbackLight.intensity *= multiplier;
		StartCoroutine ("resetLightIntensity");
	}

	IEnumerator resetLightIntensity() {
		yield return new WaitForSeconds (0.2f);
		FeedbackLight.intensity = initialFeedbackLightIntensity;
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