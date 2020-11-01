using UnityEngine;

namespace HHGame.GameScripts
{
	internal class DisableTrackVisibility : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<LineRenderer>().enabled = false;
		}
	}
}