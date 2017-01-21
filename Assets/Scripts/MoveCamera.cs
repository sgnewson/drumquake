using UnityEngine;

public class MoveCamera : MonoBehaviour {
    
    public float speed = 0.1f;

	// Update is called once per frame
	void Update () {
        var y = Input.GetAxis("Vertical");
        var x = Input.GetAxis("Horizontal");

        var delta = new Vector3(x, y, 0);

        this.transform.Translate(delta * speed);
	}
}
