using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class GlobalSoundSource : MonoBehaviour
	{
		private static GlobalSoundSource instance;

		private AudioSource source;

		public static void Play(AudioClip clip, float volume = 1f)
		{
			instance.source.PlayOneShot(clip, volume);
		}

		private void Awake()
		{
			instance = this;
			source = GetComponent<AudioSource>();
		}
	}
}