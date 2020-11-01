using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class DiveTimer : MonoBehaviour
	{
		public LoadSceneTransition transition = default;
		public SimpleTimer timer = new SimpleTimer(60 * 5);

		public TextMeshProUGUI tm = default;

		private void Start()
		{
			timer.Start();
		}

		private void Update()
		{
			timer.Update(Time.deltaTime);

			tm.text = TimeSpan.FromSeconds(timer.RemainingTime).ToString(@"mm\:ss");

			if (timer.Done)
			{
				ScreenTransition.BeginTransition(transition.LoadScene);
				enabled = false;
			}
		}
	}
}