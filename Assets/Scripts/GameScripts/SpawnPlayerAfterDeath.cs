using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class SpawnPlayerAfterDeath : MonoBehaviour
	{
		public float delay = 10f;
		public GameObject playerPrefab = default;

		private void Spawn()
		{
			Instantiate(playerPrefab, SpawnManager.Position, Quaternion.identity);
			Destroy(gameObject);
		}

		private void Start()
		{
			Invoke(nameof(Spawn), delay);
		}
	}
}