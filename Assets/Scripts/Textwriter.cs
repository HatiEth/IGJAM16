using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class Textwriter : MonoBehaviour {
	class TextWriterData {
		public float scale;
		public char letter;
		public float remainingScaleTime;
	}

	private string CurrentText = "";
	public string TargetText = "Hello i am a person. This is a very cool question or statement, for example: my dog died last month! nbtpznsabetnrliesutiarhesutesutianrsetu irn uidarengenudiane iarnegline dinrae iaesudie iae rianelgine diarne glien iaen penis";
	private float TargetTextSize;

	public float StartScale = 30f;
	public float ScaleDownTimeS = 0.3f;
	public float SecondsPerLetter = 0.3f;

	List<TextWriterData> writerData = new List<TextWriterData>();

	void Awake()
	{
		TargetTextSize = GetComponent<Text>().fontSize;
		writerData.Capacity = TargetText.Length;
	}

	public void StartText(string text)
	{
		writerData.Clear();
		TargetText = text;
		MainThreadDispatcher.StartUpdateMicroCoroutine(WriteText());
	}


	IEnumerator WriteText()
	{
		float totalTime = TargetText.Length * SecondsPerLetter;
		float localTime = 0f;
		float timeTillNextLetter = 0f;
		int LettersSave = 0;

		while (localTime < totalTime)
		{
			if(timeTillNextLetter <= 0 && LettersSave < TargetText.Length)
			{

				var twd = new TextWriterData();
				twd.letter = TargetText[LettersSave];
				twd.remainingScaleTime = ScaleDownTimeS;
				twd.scale = StartScale;
				writerData.Add(twd);

				++LettersSave;
				timeTillNextLetter += SecondsPerLetter;
			}

			CurrentText = "";
			foreach (var letter in writerData)
			{
				letter.remainingScaleTime -= Time.deltaTime;
				letter.scale = Mathf.Lerp(TargetTextSize, StartScale, letter.remainingScaleTime / ScaleDownTimeS);
				CurrentText += "<size="+letter.scale+">"+letter.letter+"</size>";
			}

			GetComponent<Text>().text = CurrentText;

			yield return null;
			timeTillNextLetter -= Time.deltaTime;
		}
	}
}
