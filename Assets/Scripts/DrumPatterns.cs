using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class DrumPatterns {

	private static DrumPattern EasyPattern;
	private static DrumPattern MediumPattern;
	private static DrumPattern MediumHardPattern;
	private static DrumPattern HardPattern;


	public static DrumPattern GetCurrentPattern(int score) {
		// use the score to determine the correct pattern and return it
		if (score < 150) {
			return EasyPattern;
		} else if (score < 300) {
			return MediumPattern;
		} else if (score < 450) {
			return MediumHardPattern;
		} else { 
			return HardPattern;
		}
	}	

	public static void SetupPatterns() {
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
		MediumPattern.Dict.Add (10, XboxButton.A);
		MediumPattern.Dict.Add (12, XboxButton.A);
		MediumPattern.Dict.Add (14, XboxButton.X);
		MediumPattern.Dict.Add (16, XboxButton.A);

		MediumHardPattern = new DrumPattern ();
		MediumHardPattern.Dict.Add (2, XboxButton.A);
		MediumHardPattern.Dict.Add (4, XboxButton.A);
		MediumHardPattern.Dict.Add (6, XboxButton.X);
		MediumHardPattern.Dict.Add (8, XboxButton.Y);
		MediumHardPattern.Dict.Add (10, XboxButton.A);
		MediumHardPattern.Dict.Add (12, XboxButton.A);
		MediumHardPattern.Dict.Add (14, XboxButton.X);
		MediumHardPattern.Dict.Add (16, XboxButton.Y);

		HardPattern = new DrumPattern ();
		HardPattern.Dict.Add (2, XboxButton.A);
		HardPattern.Dict.Add (4, XboxButton.A);
		HardPattern.Dict.Add (6, XboxButton.X);
		HardPattern.Dict.Add (7, XboxButton.Y);
		HardPattern.Dict.Add (10, XboxButton.A);
		HardPattern.Dict.Add (12, XboxButton.A);
		HardPattern.Dict.Add (14, XboxButton.X);
		HardPattern.Dict.Add (15, XboxButton.Y);
	}
		
	public class DrumPattern {
		public Dictionary<int, XboxButton> Dict;

		public DrumPattern() {
			Dict = new Dictionary<int, XboxButton>();
		}
	}
}
