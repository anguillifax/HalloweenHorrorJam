using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class OnClickLoad : MonoBehaviour
	{
		public LoadSceneTransition transition = default;
		public float waitTime = 2;
		private bool listening;

		public GameObject listener = default;

		private void Start()
		{
			Invoke(nameof(SetListen), waitTime);
		}

		private void SetListen()
		{
			listening = true;
		}

		private void Update()
		{
			if (listening && Input.GetMouseButtonDown(0))
			{
				ScreenTransition.BeginTransition(transition.LoadScene);
				if (listener != null) listener.SendMessage("OnClick");
				Destroy(this);
			}
		}
	}
}