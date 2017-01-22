using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earthquake : MonoBehaviour
{
    public Tower tower;
    private Vector3 originPosition;
    private Quaternion originRotation;
    public float shake_decay;
    public float shake_intensity;

	public GameObject BasePlate;
	int baseShakeCount;
	Vector3 intialBasePos;
	float elapsedShakeTime;

	private float shakeMultiplier;

    public float speed = 0.1f;

	SoundEffectManager soundEffectManager;

	void Awake() {
		soundEffectManager = GameObject.Find ("SoundEffectManager").GetComponent<SoundEffectManager> ();
	}

	public void EarthquakeMeDaddy(float magnitude) {
		Debug.Log ("Quaking========");
		soundEffectManager.PlayEarthquakeSound ();
		foreach (Block block in tower.glueBlockMatrix)
		{
			if (block != null)
			{
				block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				block.GetComponent<Rigidbody>().useGravity = true;
			}
		}

		Shake(magnitude);
	}

	void Start() {
		originPosition = transform.position;
		originRotation = transform.rotation;
	}

    void Update()
    {
        UpdateCameraPosition();
        UpdateCameraShake();
    }

    void UpdateCameraPosition()
    {
        var y = Input.GetAxis("Vertical");
        var x = Input.GetAxis("Horizontal");
        var delta = new Vector3(x, y, 0);
        this.transform.Translate(delta * speed);
    }

    void UpdateCameraShake()
    {
		if (shake_intensity > 0) {
			var cameraShake = shake_intensity + .05f;
			gameObject.transform.position = originPosition + Random.insideUnitSphere * cameraShake * .5f;
			gameObject.transform.rotation = new Quaternion (
				originRotation.x + Random.Range (-cameraShake, cameraShake) * .1f, 
				originRotation.y + Random.Range (-cameraShake, cameraShake) * .2f, 
				originRotation.z + Random.Range (-cameraShake, cameraShake) * .2f, 
				originRotation.w + Random.Range (-cameraShake, cameraShake) * .2f);
			shake_intensity -= shake_decay;
		} else {
			transform.position = originPosition;
			transform.rotation = originRotation;
		}
    }

    public void Shake(float shakeIntensity)
    {
		shakeMultiplier = shakeIntensity * 10 + 0.1f;
        Debug.Log("Quake Magnitude: " + shakeIntensity);
		originPosition = transform.position;
		originRotation = transform.rotation;

		intialBasePos = BasePlate.transform.position;

		elapsedShakeTime = 0f;
		baseShakeCount = 0;
		Invoke ("StopShakeBase", 10);
		InvokeRepeating ("ShakeBase", 0f, 0.2f);

        shake_intensity = shakeIntensity;
        shake_decay = 0.002f;
    }

	void ShakeBase() {
		baseShakeCount++;

		if (baseShakeCount % 2 == 0) {
			// powerful
			float xOffset = Random.Range (-0.2f*shakeMultiplier, 0.2f*shakeMultiplier);
			float yOffset = Random.Range (-0.1f, 0.1f);
			float zOffset = Random.Range (-0.2f*shakeMultiplier, 0.2f*shakeMultiplier);

			BasePlate.transform.position = new Vector3 (BasePlate.transform.position.x + xOffset, BasePlate.transform.position.y + yOffset, BasePlate.transform.position.z + zOffset);

		} else {
			BasePlate.transform.position = intialBasePos;
		}
	}

	void StopShakeBase() {
//		for (int x = 0; x < tower.gridHeight; x++) {
//			for (int y = 0; y < tower.gridWidth; y++) {
//				var towerGridSlot = tower.gridCells [x, y];
//				if (towerGridSlot != null && towerGridSlot.block != null) {
//					if (towerGridSlot.block.fellOffBase) {
//						continue;
//					}
//					
//					towerGridSlot.block.gameObject.transform.position = tower.gridCells [x, y].block.InitialPosition;
//					towerGridSlot.block.gameObject.transform.rotation = Quaternion.identity;
//					towerGridSlot.block.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
//					towerGridSlot.block.GetComponent<Rigidbody>().useGravity = false;
//				}
//			}
//		}

		CancelInvoke ("ShakeBase");
	}
}
