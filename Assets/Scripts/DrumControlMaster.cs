using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class DrumControlMaster : MonoBehaviour {

	void Update () {
		if (XCI.GetButtonDown (XboxButton.A) == true) {
			Debug.Log ("A pressed");
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

		if (XCI.GetButtonDown (XboxButton.DPadUp) == true) {
			Debug.Log ("D pad up pressed");
		}

		if (XCI.GetButtonDown (XboxButton.DPadRight) == true) {
			Debug.Log ("D pad right pressed");
		}

		if (XCI.GetButtonDown (XboxButton.DPadDown) == true) {
			Debug.Log ("D pad down pressed");
		}

		if (XCI.GetButtonDown (XboxButton.DPadLeft) == true) {
			Debug.Log ("D pad left pressed");
		}

		//Debug.Log("Left stick x: " + XCI.GetAxis(XboxAxis.LeftStickX) + " y: " + XCI.GetAxis(XboxAxis.LeftStickY) + " trigger: " + XCI.GetAxis(XboxAxis.LeftTrigger));
		//Debug.Log("Right stick x: " + XCI.GetAxis(XboxAxis.RightStickX) + " y: " + XCI.GetAxis(XboxAxis.RightStickY) + " trigger: " + XCI.GetAxis(XboxAxis.RightTrigger));



	}
}
