using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using TMPro;
using UnityEngine.Profiling;

public class FPSCounter : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI _Text;

	float deltaTime = 0.0f;
	public bool setTargetFPSTo60 = true;

	void Start()
	{
		if (setTargetFPSTo60)
			Application.targetFrameRate = 60;

	}
	
	void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        long mem = Profiler.GetMonoUsedSizeLong() / 1024 / 1024;

        // Once every 3 frames
        if (Time.frameCount % 3 != 0)
			return;

		float msec = deltaTime * 1000.0f;
		//float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0} ms ({1:0} mb)", msec,mem);

		if (_Text)
			_Text.text = text;
	}


}
