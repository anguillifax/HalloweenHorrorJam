using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHGame
{
	// =========================================================
	// Exported Interface
	// =========================================================

	[Serializable]
	public class ScreenshakeEffect
	{
		[Min(0)] public float strength = 0.2f;
	}

	// =========================================================
	// Implementation
	// =========================================================

	[Serializable]
	public class ScreenshakeTool
	{
		// =========================================================
		// Variables
		// =========================================================

		public float offsetAmount = 1;
		public float rotationAmountDeg = 5;
		public float power = 2;
		public float maxStrength = 7;
		public float offsetNoiseSpeed = 2;
		public float rotationNoiseSpeed = 2;
		public bool logStrength;

		private Vector2 output;
		private float outputDeg;
		private readonly List<ScreenshakeEffect> directs = new List<ScreenshakeEffect>();

		// =========================================================
		// Properties
		// =========================================================

		public Vector2 CurrentOffset
		{
			get => output;
		}

		public float CurrentRotationDeg
		{
			get => outputDeg;
		}

		// =========================================================
		// Methods
		// =========================================================

		private float GetPerlin(float x, float y)
		{
			return 2 * Mathf.PerlinNoise(x, y) - 1;
		}

		private float CalculateStrength()
		{
			float strength = 0f;
			strength += directs.Sum(x => Mathf.Max(0, x.strength));
			if (strength > maxStrength) strength = maxStrength;
			if (logStrength) Debug.Log("Strength: " + strength);
			return strength;
		}

		public void AddShake(ScreenshakeEffect shake)
		{
			directs.Add(shake);
		}

		public void RemoveShake(ScreenshakeEffect shake)
		{
			directs.Remove(shake);
		}

		public void Update()
		{
			float factor = Mathf.Pow(CalculateStrength(), power);

			output = factor * offsetAmount * new Vector2(
				GetPerlin(Time.time * offsetNoiseSpeed, 0),
				GetPerlin(0, Time.time * offsetNoiseSpeed));
			outputDeg = factor * rotationAmountDeg * GetPerlin(Time.time * rotationNoiseSpeed + 100, Time.time * rotationNoiseSpeed);
		}
	}
}