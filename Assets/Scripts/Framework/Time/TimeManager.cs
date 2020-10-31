using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	public static class TimeManager
	{
		// =========================================================
		// Representation
		// =========================================================

		private static readonly HashSet<TimeDilation> dilations = new HashSet<TimeDilation>();
		private static float freezeTimer = 0f;
		private static bool paused = false;

		// =========================================================
		// Utility
		// =========================================================

		private static void UpdateTimescale()
		{
			if (paused || freezeTimer > 0)
			{
				Time.timeScale = 0;
			}
			else
			{
				float value = 1f;
				foreach (var x in dilations)
					value *= x.Rate;
				Time.timeScale = value;
			}
		}

		// =========================================================
		// Public Interface
		// =========================================================

		public static bool Paused
		{
			get => paused;
			set
			{
				paused = value;
				UpdateTimescale();
			}
		}

		public static void AddTimeDilation(TimeDilation dilation)
		{
			dilations.Add(dilation);
			UpdateTimescale();
		}

		public static void RemoveTimeDilation(TimeDilation dilation)
		{
			dilations.Remove(dilation);
			UpdateTimescale();
		}

		public static void ResetAllEffects()
		{
			dilations.Clear();
			freezeTimer = 0;
			UpdateTimescale();
		}

		public static void StartFreeze(float duration)
		{
			freezeTimer += duration;
		}

		// =========================================================
		// Internal Interface
		// =========================================================

		internal static void Update()
		{
			if (freezeTimer > 0)
				freezeTimer -= Time.unscaledDeltaTime;
			UpdateTimescale();
		}
	}
}