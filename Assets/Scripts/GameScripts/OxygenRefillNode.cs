using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class OxygenRefillNode : MonoBehaviour
	{
		public bool found;

		private void OnTriggerStay2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				if (!found)
				{
					GetComponent<Light2D>().enabled = true;
					found = true;
				}
				//OxygenMeter.instance.Refill();
				SpawnManager.Position = transform.position;
			}
		}
	}
}