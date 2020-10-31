using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHGame.GameScripts
{
	internal class StartupScript : MonoBehaviour
	{
		public LoadSceneTransition transition = default;

		private void Start()
		{
			ScreenTransition.BeginTransition(transition.LoadWorld);
		}
	}
}