using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	/// <summary>
	/// Modifies the global timescale to a certain rate.
	/// </summary>
	[Serializable]
	public class TimeDilation
	{
		// =========================================================
		// Representation
		// =========================================================

		[SerializeField]
		[Tooltip("The timescale modifer")]
		[Min(0.001f)]
		private float rate = 1.0f;

		// =========================================================
		// Properties
		// =========================================================

		/// <summary>
		/// The timescale modifier.
		/// 
		/// <para>
		/// If this time dilation was applied in isolation, then <see
		/// cref="Time.timeScale"/> would exactly match.
		/// </para>
		/// </summary>
		public float Rate
		{
			get => rate;
			set
			{
				if (value < 0)
					throw new ArgumentException("Rate must be positive.");
				if (value == 0)
					throw new ArgumentException("Cannot set rate to 0. Use the pause property to freeze the game.");

				rate = value;
			}
		}

		// =========================================================
		// Public Interface
		// =========================================================

		public TimeDilation() : this(1)
		{
		}

		public TimeDilation(float rate) {
			Rate = rate;
		}

		public static TimeDilation operator+(TimeDilation a, TimeDilation b)
		{
			return new TimeDilation(a.rate * b.rate);
		}
	}
}