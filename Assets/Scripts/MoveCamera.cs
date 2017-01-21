using UnityEngine;

public class MoveCamera : MonoBehaviour {
    

	private Vector3 originPosition;
	private Quaternion originRotation;
	public float shake_decay;
	public float shake_intensity;

    public float speed = 0.1f;

	void Update () {
		UpdateCameraPosition ();
		UpdateCameraShake ();
	}

	void UpdateCameraPosition ()
	{
		var y = Input.GetAxis ("Vertical");
		var x = Input.GetAxis ("Horizontal");
		var delta = new Vector3 (x, y, 0);
		this.transform.Translate (delta * speed);
	}

	void UpdateCameraShake ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Shake ();
		}
		if (shake_intensity > 0) {
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			transform.rotation = new Quaternion (originRotation.x + Random.Range (-shake_intensity, shake_intensity) * .2f, originRotation.y + Random.Range (-shake_intensity, shake_intensity) * .2f, originRotation.z + Random.Range (-shake_intensity, shake_intensity) * .2f, originRotation.w + Random.Range (-shake_intensity, shake_intensity) * .2f);
			shake_intensity -= shake_decay;
		}
	}

	void Shake(){
		originPosition = transform.position;
		originRotation = transform.rotation;
		shake_intensity = .3f;
		shake_decay = 0.002f;
	}

}
