using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HHGame.GameScripts
{
	internal static class FixLines
	{
		[MenuItem("CONTEXT/LineRenderer/Fix Lines")]
		private static void Fix(MenuCommand command)
		{
			LineRenderer lr = (LineRenderer)command.context;
			for (int i = 0; i < lr.positionCount; i++)
			{
				var v = lr.GetPosition(i);
				v.z = 0;
				lr.SetPosition(i, v);
			}
		}
	}
}