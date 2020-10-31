using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	public static class ScreenUtil
	{
		public static event Action Resized;

		internal static void InvokeResize()
		{
			var cpy = Resized;
			cpy?.Invoke();
		}
	}
}