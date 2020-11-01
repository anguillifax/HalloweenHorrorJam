using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class DoorState : MonoBehaviour
	{
		public static DoorState instance;

		public int locks = 2;
		public Sprite[] sprites = default;
		public LoadSceneTransition transition = default;
		public AudioClip clip;

		public static void UnlockPiece()
		{
			--instance.locks;
			instance.Repaint();
		}

		private void Repaint()
		{
			GetComponent<SpriteRenderer>().sprite = sprites[locks];
		}

		private void Awake()
		{
			instance = this;
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("Player") && locks == 0)
			{
				GlobalSoundSource.Play(clip);
				ScreenTransition.BeginTransition(transition.LoadScene);
				Destroy(this);
			}
		}
	}
}