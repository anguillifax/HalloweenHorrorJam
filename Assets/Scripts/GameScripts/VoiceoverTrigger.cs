using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class VoiceoverTrigger : MonoBehaviour
	{
		public float initWait = 2;
		public float rate = 5;
		private AudioSource source;
		private bool muting;

		private void Awake()
		{
			source = GetComponent<AudioSource>();
		}

		private void BeginPlay()
		{
			source.Play();
		}

		private void Start()
		{
			Invoke(nameof(BeginPlay), initWait);
		}

		private void Update()
		{
			if (muting)
				source.volume = Mathf.MoveTowards(source.volume, 0, rate * Time.deltaTime);
		}

		public void OnClick()
		{
			muting = true;
		}
	}
}