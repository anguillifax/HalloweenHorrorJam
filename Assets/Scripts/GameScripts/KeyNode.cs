﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class KeyNode : MonoBehaviour
	{
		public AudioClip clip = default;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("Player"))
			{
				GlobalSoundSource.Play(clip);
				DoorState.UnlockPiece();
				Destroy(gameObject);
			}
		}
	}
}