using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class TextScroll : MonoBehaviour
	{
		public float initDelay = 3f;
		public SimpleTimer timer = new SimpleTimer(5);

		private TextMeshProUGUI tm;
		private bool running;

		private void Awake()
		{
			tm = GetComponent<TextMeshProUGUI>();
			tm.maxVisibleCharacters = 0;
		}

		private void Begin()
		{
			running = true;
		}

		private void Start()
		{
			Invoke(nameof(Begin), initDelay);
		}

		private void Update()
		{
			if (running)
			{
				timer.Update(Time.deltaTime);
				tm.maxVisibleCharacters = Mathf.CeilToInt(timer.NormalizedProgress * tm.text.Length);
			}
		}
	}
}