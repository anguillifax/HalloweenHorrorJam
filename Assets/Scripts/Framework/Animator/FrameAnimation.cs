using System;
using UnityEngine;

namespace HHGame
{
	/// <summary>
	/// Holds a sequence of sprites to display in a frame-by-frame animation.
	/// </summary>
	[CreateAssetMenu(menuName = "Momentum/Frame Animation Clip")]
	public class FrameAnimation : ScriptableObject
	{
		public float framerate = 24;
		public bool loop = true;
		public Sprite[] frames = new Sprite[0];

		/// <summary>
		/// Verifies the invariants of a frame animation. The clip must contain
		/// at least one frame, must have a positive framerate, and may not
		/// have sprite frames assigned to null. An exception is thrown if any
		/// of these invariants are not upheld.
		/// </summary>
		///
		/// <exception cref="Exception" />
		public void AssertValid()
		{
			if (frames.Length == 0)
			{
				throw new Exception($"Animation clip `{name}` has no frames");
			}

			if (framerate <= 0)
			{
				throw new Exception($"Animation clip `{name}` has an invalid framerate");
			}

			foreach (Sprite item in frames)
			{
				if (item == null)
				{
					throw new Exception($"Animation clip `{name}` has a frame with no assigned sprite.");
				}
			}
		}
	}
}