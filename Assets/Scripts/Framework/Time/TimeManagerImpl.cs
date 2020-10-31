using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HHGame
{
	internal class TimeManagerImpl : MonoBehaviour
	{
		// =========================================================
		// Callbacks
		// =========================================================

		private static void ClearDilations(Scene scene, LoadSceneMode mode)
		{
			if (mode == LoadSceneMode.Single)
			{
				TimeManager.ResetAllEffects();
			}
		}

		// =========================================================
		// Implementation
		// =========================================================

		private void Awake()
		{
			SceneManager.sceneLoaded += ClearDilations;
		}

		private void OnDestroy()
		{
			SceneManager.sceneLoaded -= ClearDilations;
		}

		private void Update()
		{
			TimeManager.Update();
		}
	}
}