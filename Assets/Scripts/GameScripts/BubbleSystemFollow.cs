using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class BubbleSystemFollow : MonoBehaviour
	{
		public Transform parent = default;
		public float waitTime = 10f;

		private void LateUpdate()
		{
			if (parent != null)
			{
				transform.position = parent.position;
			}
			else
			{
				Destroy(gameObject, waitTime);
				Destroy(this);
			}
		}
	}
}