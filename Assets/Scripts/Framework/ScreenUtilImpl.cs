using UnityEngine;

namespace HHGame
{
	internal class ScreenUtilImpl : MonoBehaviour
	{
		private int lastWidth = -1;
		private int lastHeight = -1;

		private void Update()
		{
			if (Screen.width != lastWidth || Screen.height != lastHeight)
			{
				lastWidth = Screen.width;
				lastHeight = Screen.height;
				ScreenUtil.InvokeResize();
			}
		}
	}
}