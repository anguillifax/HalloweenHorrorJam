using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	[Serializable]
	internal class RampedValue
	{
		// =========================================================
		// Representation
		// =========================================================

		[SerializeField] private float max = 5;
		[SerializeField] private float min = 0;
		[SerializeField] private float growSpeed = 3;
		[SerializeField] private float decaySpeed = 2;
		[SerializeField] private float current;

		// =========================================================
		// Properties
		// =========================================================

		public float Current
		{
			get => current;
		}

		// =========================================================
		// Methods
		// =========================================================

		public void SetMin()
		{
			current = min;
		}

		public void SetMax()
		{
			current = max;
		}

		public void Grow()
		{
			current = Mathf.MoveTowards(current, max, growSpeed * Time.fixedDeltaTime);
		}

		public void Decay()
		{
			current = Mathf.MoveTowards(current, min, decaySpeed * Time.fixedDeltaTime);
		}
	}
}