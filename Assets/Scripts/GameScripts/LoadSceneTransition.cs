using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHGame.GameScripts
{
	[Serializable]
	internal class LoadSceneTransition
	{
		public int id = default;
		public float duration = 1f;

		public IEnumerator LoadWorld(ScreenTransitionTool tool)
		{
			SimpleTimer timer = new SimpleTimer(duration);

			tool.TransitionType = ScreenTransitionType.Fade;

			timer.Start();
			while(timer.Running)
			{
				timer.Update(Time.deltaTime);
				tool.Progress = timer.NormalizedProgress;
				yield return null;
			}

			var wait = SceneManager.LoadSceneAsync(id);
			yield return new WaitUntil(() => wait.isDone);
			
			timer.Start();
			while(timer.Running)
			{
				timer.Update(Time.deltaTime);
				tool.Progress = 1 - timer.NormalizedProgress;
				yield return null;
			}
			tool.Progress = 0;
			Debug.Log("Finish transition");
		}
	}
}