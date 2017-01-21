using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class DrumControlMaster : MonoBehaviour {

	public WaveManager WaveManager;

	void Update () {
		if (XCI.GetButtonDown (XboxButton.A) == true) {
			WaveManager.HandleDrumPress ();
		}

		if (XCI.GetButtonDown (XboxButton.B) == true) {
			Debug.Log ("B pressed");
		}

		if (XCI.GetButtonDown (XboxButton.X) == true) {
			Debug.Log ("X pressed");
		}

		if (XCI.GetButtonDown (XboxButton.Y) == true) {
			Debug.Log ("Y pressed");
		}
	}
}
