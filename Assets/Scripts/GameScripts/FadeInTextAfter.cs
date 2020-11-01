using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class FadeInTextAfter : MonoBehaviour
	{
		public float duration = 2;
		public float fadeSpeed = 1;
		private SimpleTimer timer;

		public TextMeshProUGUI tm = default;

		private void Awake()
		{
			timer = new SimpleTimer(duration);
			timer.Start();
		}

		private void Update()
		{
			timer.Update(Time.deltaTime);
			if (timer.Done)
			{
				var c = tm.color;
				c.a = Mathf.MoveTowards(c.a, 1f, fadeSpeed * Time.fixedDeltaTime);
				tm.color = c;
			}
		}
	}
}