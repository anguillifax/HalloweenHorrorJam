using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHGame.GameScripts
{
	internal class MainMenuScript : MonoBehaviour
	{
		public LoadSceneTransition transition = default;

		public void Play()
		{
			ScreenTransition.BeginTransition(transition.LoadScene);
		}

		public void Quit()
		{
			Application.Quit();
		}
	}
}