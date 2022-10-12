using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TheRuleScript : MonoBehaviour {

		public KMBombInfo bomb;
		// public KMAudio audio;

		public KMSelectable[] rowButtons;
		public KMSelectable buttonS;

		public Material[] offOnStates;
		public Material[] checkStates;

		// public AudioClip[] sounds;

		public TextMesh display;

		public Renderer[] rows = new Renderer[26]; // 8 7 6 5
			/*
			0 1 2 3 4 5 6 7
			 8 9 A B C D E
			  F G H I J K
				 L M N O P
			*/

		private bool[] rowBools = new bool[26];
		private bool[] rowBoolsCorrect = new bool[26];

		private int ruleNumber;
		private bool[] ruleBinary = new bool[4];

		private int pressIndex;

		private bool animating;

		static int moduleIdCounter = 1;
		int moduleId;
		private bool moduleSolved;

		void Awake() {
				moduleId = moduleIdCounter++;

				foreach(KMSelectable element in rowButtons) {
						element.OnInteract += delegate() {
								if (moduleSolved || animating) return false;
								pressIndex = Array.IndexOf(rowButtons, element);
								ToggleSquare();
								return false;
						};
				}

				buttonS.OnInteract += delegate() {
						buttonS.AddInteractionPunch();
						if (moduleSolved || animating) return false;
						StartCoroutine(Check());
						return false;
				};
		}

		void Start() {
				ResetModule();
		}

		void PrintStates() {
				string printout = "";
				for(int i = 0; i < 8; i++) {
						if(rowBools[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n ";
				for(int i = 8; i < 15; i++) {
						if(rowBools[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n  ";
				for(int i = 15; i < 21; i++) {
						if(rowBools[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n   ";
				for(int i = 21; i < 26; i++) {
						if(rowBools[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}

				Debug.LogFormat("[The Rule #{0}]" + "\n" + printout, moduleId);
		}

		void PrintSolution() {
				string printout = "";
				for(int i = 0; i < 8; i++) {
						if(rowBoolsCorrect[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n ";
				for(int i = 8; i < 15; i++) {
						if(rowBoolsCorrect[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n  ";
				for(int i = 15; i < 21; i++) {
						if(rowBoolsCorrect[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}
				printout += "\n   ";
				for(int i = 21; i < 26; i++) {
						if(rowBoolsCorrect[i]) {
								printout += "1 ";
						} else {
								printout += "0 ";
						}
				}

				Debug.LogFormat("[The Rule #{0}]" + "\n" + printout, moduleId);
		}

		void UpdateSquares() {
				for(int i = 0; i < 26; i++) {
						if(rowBools[i]) {
								rows[i].material = offOnStates[1];
						} else {
								rows[i].material = offOnStates[0];
						}
				}
		}

		void ResetModule() {
				Debug.LogFormat("[The Rule #{0}] Resetting module...", moduleId);

				GenerateRule();

				for(int i = 0; i < 8; i++) {
						rowBools[i] = (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5);
						rowBoolsCorrect[i] = rowBools[i];
				}

				for(int i = 8; i < 26; i++) {
						rowBools[i] = false;
						rowBoolsCorrect[i] = false;
				}

				UpdateSquares();
				DetermineSolution();

				Debug.LogFormat("[The Rule #{0}] Solution is:", moduleId);
				PrintSolution();
		}

		void GenerateRule() {
				ruleNumber = UnityEngine.Random.Range(1,15);
				if(ruleNumber < 10) {
						display.text = "0" + ruleNumber.ToString();
				} else {
						display.text = ruleNumber.ToString();
				}

				ruleBinary[0] = ruleNumber >= 8;
				ruleBinary[1] = ruleNumber % 8 >= 4;
				ruleBinary[2] = ruleNumber % 4 >= 2;
				ruleBinary[3] = ruleNumber % 2 == 1;

				Debug.LogFormat("[The Rule #{0}] Rule Number is " + ruleNumber, moduleId);
				Debug.LogFormat("[The Rule #{0}] (1,1) yields " + ruleNumber / 8, moduleId);
				Debug.LogFormat("[The Rule #{0}] (1,0) yields " + (ruleNumber % 8) / 4, moduleId);
				Debug.LogFormat("[The Rule #{0}] (0,1) yields " + (ruleNumber % 4) / 2, moduleId);
				Debug.LogFormat("[The Rule #{0}] (0,0) yields " + ruleNumber % 2, moduleId);
		}

		void DetermineSolution() {
				for(int i = 0; i < 7; i++) {
						if(ruleBinary[0]) {
								if(rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+8] = true;
								}
						}
						if(ruleBinary[1]) {
								if(rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+8] = true;
								}
						}
						if(ruleBinary[2]) {
								if(!rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+8] = true;
								}
						}
						if(ruleBinary[3]) {
								if(!rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+8] = true;
								}
						}
				}

				for(int i = 8; i < 14; i++) {
						if(ruleBinary[0]) {
								if(rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+7] = true;
								}
						}
						if(ruleBinary[1]) {
								if(rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+7] = true;
								}
						}
						if(ruleBinary[2]) {
								if(!rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+7] = true;
								}
						}
						if(ruleBinary[3]) {
								if(!rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+7] = true;
								}
						}
				}

				for(int i = 15; i < 20; i++) {
						if(ruleBinary[0]) {
								if(rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+6] = true;
								}
						}
						if(ruleBinary[1]) {
								if(rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+6] = true;
								}
						}
						if(ruleBinary[2]) {
								if(!rowBoolsCorrect[i] && rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+6] = true;
								}
						}
						if(ruleBinary[3]) {
								if(!rowBoolsCorrect[i] && !rowBoolsCorrect[i+1]) {
										rowBoolsCorrect[i+6] = true;
								}
						}
				}
		}

		void ToggleSquare() {
				rowBools[pressIndex + 8] = !rowBools[pressIndex + 8];
				UpdateSquares();
		}

		IEnumerator Check() {
				animating = true;
				bool correct = true;
				Debug.LogFormat("[The Rule #{0}] Submitted the following:", moduleId);
				PrintStates();

				for(int i = 8; i < 26; i++) {
						if(rowBools[i] == rowBoolsCorrect[i]) {
								rows[i].material = checkStates[0];
						} else {
								rows[i].material = checkStates[1];
								correct = false;
						}

						yield return new WaitForSeconds(0.1f);
				}

				yield return new WaitForSeconds(1f);

				if(correct) {
						display.text = ":)";
						moduleSolved = true;
						GetComponent<KMBombModule>().HandlePass();
						Debug.LogFormat("[The Rule #{0}] Module solved!", moduleId);
				} else {
						GetComponent<KMBombModule>().HandleStrike();
						Debug.LogFormat("[The Rule #{0}] Incorrect solution, strike given!", moduleId);
						ResetModule();
				}
				animating = false;
		}

		// TwitchPlays Code
		#pragma warning disable 414
    		private string TwitchHelpMessage = "Type '!{0} # # #' to toggle squares in reading order. Type '!{0} submit' to submit your answer.";
		#pragma warning restore 414

		IEnumerator ProcessTwitchCommand(string input) {

			if(Regex.IsMatch(input, @"^\s*submit\s*$", RegexOptions.IgnoreCase)) {
				yield return null;
				buttonS.OnInteract();
				bool correct = true;
				for (int i = 8; i < 26; i++) {
					if (rowBools[i] != rowBoolsCorrect[i]) {
						correct = false;
						break;
					}
				}
				if (!correct)
					yield return "strike";
				else
					yield return "solve";
				yield break;
			}

				string[] parameters = input.Split(' ');
				var buttonsToPress = new List<KMSelectable>();
				foreach (string param in parameters) {
						if(param.EqualsIgnoreCase("1")) {
								buttonsToPress.Add(rowButtons[0]);
						} else if(param.EqualsIgnoreCase("2")) {
								buttonsToPress.Add(rowButtons[1]);
						} else if(param.EqualsIgnoreCase("3")) {
								buttonsToPress.Add(rowButtons[2]);
						} else if(param.EqualsIgnoreCase("4")) {
								buttonsToPress.Add(rowButtons[3]);
						} else if(param.EqualsIgnoreCase("5")) {
								buttonsToPress.Add(rowButtons[4]);
						} else if(param.EqualsIgnoreCase("6")) {
								buttonsToPress.Add(rowButtons[5]);
						} else if(param.EqualsIgnoreCase("7")) {
								buttonsToPress.Add(rowButtons[6]);
						} else if(param.EqualsIgnoreCase("8")) {
								buttonsToPress.Add(rowButtons[7]);
						} else if(param.EqualsIgnoreCase("9")) {
								buttonsToPress.Add(rowButtons[8]);
						} else if(param.EqualsIgnoreCase("10")) {
								buttonsToPress.Add(rowButtons[9]);
						} else if(param.EqualsIgnoreCase("11")) {
								buttonsToPress.Add(rowButtons[10]);
						} else if(param.EqualsIgnoreCase("12")) {
								buttonsToPress.Add(rowButtons[11]);
						} else if(param.EqualsIgnoreCase("13")) {
								buttonsToPress.Add(rowButtons[12]);
						} else if(param.EqualsIgnoreCase("14")) {
								buttonsToPress.Add(rowButtons[13]);
						} else if(param.EqualsIgnoreCase("15")) {
								buttonsToPress.Add(rowButtons[14]);
						} else if(param.EqualsIgnoreCase("16")) {
								buttonsToPress.Add(rowButtons[15]);
						} else if(param.EqualsIgnoreCase("17")) {
								buttonsToPress.Add(rowButtons[16]);
						} else if(param.EqualsIgnoreCase("18")) {
								buttonsToPress.Add(rowButtons[17]);
						} else {
								yield break;
						}
				}

				yield return null;
				yield return buttonsToPress;
		}

		IEnumerator TwitchHandleForcedSolve()
		{
			if (animating)
			{
				bool correct = true;
				for (int i = 8; i < 26; i++) {
					if (rowBools[i] != rowBoolsCorrect[i]) {
						correct = false;
						break;
					}
				}
				if (!correct)
				{
					moduleSolved = true;
					animating = false;
					StopAllCoroutines();
					GetComponent<KMBombModule>().HandlePass();
					yield break;
				}
			}
			else
			{
				for (int i = 8; i < 26; i++) {
					if (rowBools[i] != rowBoolsCorrect[i]) {
						rowButtons[i - 8].OnInteract();
						yield return new WaitForSeconds(0.1f);
					}
				}
				buttonS.OnInteract();
			}
			while (!moduleSolved) yield return true;
		}
}
