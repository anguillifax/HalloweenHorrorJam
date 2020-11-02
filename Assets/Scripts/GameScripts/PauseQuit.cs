using UnityEngine;

namespace HHGame.GameScripts
{
	internal class PauseQuit : MonoBehaviour
	{
		public void QuitGame()
		{
			Application.Quit();
		}
	}
}