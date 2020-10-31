using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class LightGlow : MonoBehaviour
	{
		public float factor = 1f;

		public SimpleTimer lifetime = default;
		public AnimationCurve ramp = default;
		public float baseGlow = 0.2f;

		public Light2D glow = default;

		private void Start()
		{
			glow.color = UnityEngine.Random.ColorHSV(0, 1, 0.2f, 0.4f, 0.9f, 1f);
			lifetime.Start();
		}

		private void Update()
		{
			lifetime.Update(Time.deltaTime);
			glow.intensity = baseGlow * ramp.Evaluate(lifetime.NormalizedProgress);

			if (lifetime.Done)
			{
				Destroy(gameObject);
			}
		}
	}
}