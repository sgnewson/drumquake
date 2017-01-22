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

	public Tower tower;

	public float HeartBeatInMilliseconds = 300f;
	int beatCount;

	private int WaveScore = 0;
	private int EarthquakeTimerCount = EarthquakeTimerStartCount;

	public Text EarthquakeTimerUI;
	public Text WaveScoreUI;
	public Image HitResult;
	public Text BuilderScoreUI;

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

	DrumPatterns.DrumPattern CurrentPattern;

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

	private enum DrumHitQuality { Hit, Close, Miss, Ignored, Incorrect };
	public enum LerpMode {Linear, EaseOut, EaseIn, Exponential, SmoothStep, SmootherStep};
	public LerpMode TransitionLerpMode;

	void Awake() {
		ColorMap = new Dictionary<XboxButton, AudioStuff> ();
		ColorMap.Add (XboxButton.A, new AudioStuff(GreenSound, baseGreenVolume, greenCircle, 10));
		ColorMap.Add (XboxButton.B, new AudioStuff(RedSound, baseRedVolume, redCircle, 50));
		ColorMap.Add (XboxButton.X, new AudioStuff(BlueSound, baseBlueVolume, blueCircle, 20));
		ColorMap.Add (XboxButton.Y, new AudioStuff(YellowSound, baseYellowVolume, yellowCircle, 30));

		audioSource = this.GetComponent<AudioSource> ();
		WaveList = new List<Wave> ();

		DrumPatterns.SetupPatterns ();
		CurrentPattern = DrumPatterns.GetCurrentPattern(WaveScore);

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
			CurrentPattern = DrumPatterns.GetCurrentPattern(WaveScore);
			beatCount = 1;
		}

		//Debug.Log ("Beat count: " + beatCount);

		if (CurrentPattern.Dict.ContainsKey (beatCount) == true) {
			SpawnWave (CurrentPattern.Dict [beatCount]);
		}
	}

	void SpawnWave(XboxButton waveType) {
		GameObject newWaveGameObject = (GameObject) GameObject.Instantiate (WavePrefabLocal, ReferenceWave.transform.position, Quaternion.identity);
		newWaveGameObject.SetActive (true);

		newWaveGameObject.transform.Find("WavePrefab").gameObject.GetComponent<Renderer> ().material = ColorMap[waveType].circle;
			
		Wave newWave = new Wave (waveType, waveStartRadius, newWaveGameObject);
		WaveList.Add (newWave);
	}

	void EarthquakeCountdown() {
			
        if (EarthquakeTimerCount == 0)
        {
//			EarthquakeTimerUI.text = "Countdown: 0";
			if (GameManager.PlayOn) {
				Invoke ("StartEarthquake", 5f);
			}
			GameManager.PlayOn = false;
			tower.HandleBeforeEarthquake ();
        }
        else 
        {
			EarthquakeTimerCount--;

			tower.HandleAfterEarthquake ();

            GameManager.PlayOn = true;
            EarthquakeTimerUI.text = "Countdown: " + EarthquakeTimerCount;
        }
		BuilderScoreUI.text = "Builder Score: " + tower.GetBuilderScore ();

	}

	void StartEarthquake() {
		MainCamera.GetComponent<Earthquake>().EarthquakeMeDaddy(WaveScore / 1000f);
//		MainCamera.GetComponent<Earthquake>().EarthquakeMeDaddy(0.03f);
//		WaveScore = 0;

		EarthquakeTimerUI.text = "Earthquake!!!!";
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

		WaveScoreUI.text = "Magnitude: " + WaveScore/100f;
	}

	public void HandleDrumPress(XboxButton buttonPressed) {
		if (WaveList.Count == 0) {
			WaveMiss (buttonPressed);
			return;
		}

		Wave wave = WaveList [0];
//		Debug.Log ("percentage: " + wave.Percentage);

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
		GameManager.LastHitType = buttonPressed;

		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * HitVolumeMultiplier;
		audioSource.pitch = basePitch * HitPitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Perfect;

		WaveScore = GetNewScore(WaveScore, buttonPressed, DrumHitQuality.Hit);

		IntensifyLight (2f);
	}

	private void  WaveClose (XboxButton buttonPressed)
	{
		GameManager.LastHitType = buttonPressed;

		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * CloseVolumeMultiplier;
		audioSource.pitch = basePitch * ClosePitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Good;

		WaveScore = GetNewScore(WaveScore, buttonPressed, DrumHitQuality.Close);

		IntensifyLight (1.3f);
	}

	private void  WaveMiss (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Miss;

		WaveScore = GetNewScore(WaveScore, buttonPressed, DrumHitQuality.Miss);
	}

	private void  WaveIgnored ()
	{
		HitResult.sprite = Miss;
		WaveScore = GetNewScore(WaveScore, XboxButton.DPadDown, DrumHitQuality.Ignored); //DPadDown is just so we have something here
	}

	private void  WaveIncorrect (XboxButton buttonPressed)
	{
		audioSource.clip = ColorMap[buttonPressed].thisClip;
		audioSource.volume = ColorMap[buttonPressed].Volume * MissVolumeMultiplier;
		audioSource.pitch = basePitch * MissPitchMultiplier;
		audioSource.Play ();

		HitResult.sprite = Miss;
	
		WaveScore = GetNewScore(WaveScore, buttonPressed, DrumHitQuality.Incorrect);
	}

	private int GetNewScore(int currentScore, XboxButton pressed, DrumHitQuality quality) {
		switch (quality) {
		case DrumHitQuality.Hit: 
			return currentScore + ColorMap[pressed].hitScore;
		case DrumHitQuality.Close:
			return currentScore + ColorMap[pressed].hitScore/5;
		case DrumHitQuality.Miss:
			return currentScore - ColorMap[pressed].hitScore/2;
		case DrumHitQuality.Ignored:
			return currentScore - 5; //Can't use button pressed because none was pressed when ignored
		case DrumHitQuality.Incorrect:
			return currentScore - ColorMap[pressed].hitScore/2;
		default:
			return currentScore;
		}
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
		public int hitScore;

		public AudioStuff(AudioClip c, float v, Material circle, int hitScore) {
			thisClip = c;
			Volume = v;
			this.circle = circle;
			this.hitScore = hitScore;
		}
	}
}