using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class DrumPatterns {

	private static DrumPattern Pattern1;
	private static DrumPattern Pattern2;
	private static DrumPattern Pattern3;
	private static DrumPattern Pattern4;


	public static DrumPattern GetCurrentPattern(int score) {
		// use the score to determine the correct pattern and return it
		if (score < 150) {
			return Pattern1;
		} else if (score < 300) {
			return Pattern2;
		} else if (score < 450) {
			return Pattern3;
		} else { 
			return Pattern4;
		}
	}	

	public static void SetupPatterns() {
		Pattern1 = new DrumPattern ();
		Pattern1.Dict.Add (2, XboxButton.A);
		Pattern1.Dict.Add (4, XboxButton.A);
		Pattern1.Dict.Add (6, XboxButton.A);
		Pattern1.Dict.Add (8, XboxButton.A);
		Pattern1.Dict.Add (10, XboxButton.A);
		Pattern1.Dict.Add (12, XboxButton.A);
		Pattern1.Dict.Add (14, XboxButton.A);
		Pattern1.Dict.Add (16, XboxButton.A);

		Pattern2 = new DrumPattern ();
		Pattern2.Dict.Add (2, XboxButton.A);
		Pattern2.Dict.Add (4, XboxButton.A);
		Pattern2.Dict.Add (6, XboxButton.X);
		Pattern2.Dict.Add (8, XboxButton.A);
		Pattern2.Dict.Add (10, XboxButton.A);
		Pattern2.Dict.Add (12, XboxButton.A);
		Pattern2.Dict.Add (14, XboxButton.X);
		Pattern2.Dict.Add (16, XboxButton.A);

		Pattern3 = new DrumPattern ();
		Pattern3.Dict.Add (2, XboxButton.A);
		Pattern3.Dict.Add (4, XboxButton.A);
		Pattern3.Dict.Add (6, XboxButton.X);
		Pattern3.Dict.Add (8, XboxButton.Y);
		Pattern3.Dict.Add (10, XboxButton.A);
		Pattern3.Dict.Add (12, XboxButton.A);
		Pattern3.Dict.Add (14, XboxButton.X);
		Pattern3.Dict.Add (16, XboxButton.Y);

		Pattern4 = new DrumPattern ();
		Pattern4.Dict.Add (2, XboxButton.A);
		Pattern4.Dict.Add (4, XboxButton.A);
		Pattern4.Dict.Add (6, XboxButton.X);
		Pattern4.Dict.Add (7, XboxButton.Y);
		Pattern4.Dict.Add (10, XboxButton.A);
		Pattern4.Dict.Add (12, XboxButton.A);
		Pattern4.Dict.Add (14, XboxButton.X);
		Pattern4.Dict.Add (15, XboxButton.Y);
	}
		
	public class DrumPattern {
		public Dictionary<int, XboxButton> Dict;

		public DrumPattern() {
			Dict = new Dictionary<int, XboxButton>();
		}
	}
}
