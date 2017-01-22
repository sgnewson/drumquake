using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour {

	AudioSource audioSource;

	public AudioClip PickupBlockSound;
	public AudioClip PlaceBlockSound;
	public AudioClip EarthquakeSound;
	public AudioClip[] TowerFallSounds;

	float initialVolume;

	void Awake() {
		audioSource = this.GetComponent<AudioSource> ();
		initialVolume = audioSource.volume;
	}

	public void PlayPickupBlockSound() {
		audioSource.volume = initialVolume;
		audioSource.PlayOneShot (PickupBlockSound);
	}

	public void PlayPlaceBlockSound() {
		audioSource.volume = initialVolume;
		audioSource.PlayOneShot (PlaceBlockSound);
	}

	public void PlayEarthquakeSound() {
		audioSource.volume = initialVolume;
		audioSource.PlayOneShot (EarthquakeSound);
	}

	public void PlayTowerFallSound() {
		audioSource.volume = initialVolume * 0.6f;
		int ind = (int)Mathf.Floor(Random.Range (0, TowerFallSounds.Length));
		audioSource.PlayOneShot(TowerFallSounds [ind]);
	}
}
