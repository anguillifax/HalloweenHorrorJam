using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class ShakeCurve : MonoBehaviour
	{
		public AnimationCurve curve = default;
		public SimpleTimer timer = default;

		private readonly ScreenshakeEffect shake = new ScreenshakeEffect();

		private void Awake()
		{
			WorldCamera.instance.Screenshake.AddShake(shake);
		}

		private void OnDestroy()
		{
			WorldCamera.instance.Screenshake.RemoveShake(shake);
		}

		private void Update()
		{
			timer.Update(Time.deltaTime);

			shake.strength = curve.Evaluate(timer.NormalizedProgress);

			if (timer.Done)
			{
				Destroy(gameObject);
			}
		}
	}
}