using System;
using System.Collections;

namespace HHGame
{
	public static class ScreenTransition
	{
		// =========================================================
		// Variables
		// =========================================================

		internal static ScreenTransitionImpl impl;

		// =========================================================
		// Methods
		// =========================================================

		public static void BeginTransition(Func<ScreenTransitionTool, IEnumerator> coroutine)
		{
			impl.BeginTransition(coroutine);
		}
	}
}