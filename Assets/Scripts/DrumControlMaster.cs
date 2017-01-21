using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class DrumControlMaster : MonoBehaviour {

	public WaveManager WaveManager;
	public Light FeedbackLight;

	void Update () {

		if (XCI.GetButtonDown (XboxButton.B) == true || Input.GetKeyDown(KeyCode.Alpha1)) {
			WaveManager.HandleDrumPress(XboxButton.B);
			FeedbackLight.color = Color.red;
		}

		if (XCI.GetButtonDown (XboxButton.Y) == true || Input.GetKeyDown(KeyCode.Alpha2)) {
			WaveManager.HandleDrumPress(XboxButton.Y);
			FeedbackLight.color = Color.yellow;
		}

		if (XCI.GetButtonDown (XboxButton.X) == true || Input.GetKeyDown(KeyCode.Alpha3)) {
			WaveManager.HandleDrumPress(XboxButton.X);
			FeedbackLight.color = Color.blue;
		}

		if (XCI.GetButtonDown (XboxButton.A) == true || Input.GetKeyDown(KeyCode.Alpha4)) {
			WaveManager.HandleDrumPress(XboxButton.A);
			FeedbackLight.color = Color.green;
		}
	}
}
