using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHGame.GameScripts
{
	internal class StartupScript : MonoBehaviour
	{
		public int mainMenuId = 1;
		public float duration = 1f;
		public ScreenTransitionType type = ScreenTransitionType.Fade;

		private IEnumerator TransitionCoroutine(ScreenTransitionTool tool)
		{
			tool.Progress = 1f;
			tool.TransitionType = type;

			var scene = SceneManager.LoadSceneAsync(1);
			yield return new WaitUntil(() => scene.isDone);

			SimpleTimer timer = new SimpleTimer(duration);
			timer.Start();
			while (timer.Running)
			{
				timer.Update(Time.deltaTime);
				tool.Progress = 1 - timer.NormalizedProgress;
				yield return null;
			}
			tool.Progress = 0;
		}

		private void Start()
		{
			ScreenTransition.BeginTransition(TransitionCoroutine);
		}
	}
}