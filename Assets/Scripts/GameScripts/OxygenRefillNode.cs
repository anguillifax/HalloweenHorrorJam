using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class OxygenRefillNode : MonoBehaviour
	{
		public bool found;
		public Vector2 glowRange = new Vector2(0.2f, 1);
		public AudioClip soundTouch = default;
		private Light2D glow;

		private bool IsCurrent
		{
			get => SpawnManager.Position == (Vector2)transform.position;
		}

		private void Awake()
		{
			glow = GetComponent<Light2D>();
		}

		private void Update()
		{
			glow.intensity = Mathf.MoveTowards(glow.intensity, IsCurrent ? glowRange.y : glowRange.x, 1f * Time.deltaTime);
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				if (!found)
				{
					GetComponent<Light2D>().enabled = true;
					found = true;
				}

				if (!IsCurrent)
				{
					GlobalSoundSource.Play(soundTouch);
				}
				SpawnManager.Position = transform.position;
			}
		}
	}
}