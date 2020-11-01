using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HHGame.GameScripts
{
	internal class OxygenMeter : MonoBehaviour
	{
		public static OxygenMeter instance;

		public float oxygen;
		public float maxOxygen = 10;
		public Image image = default;
		public float refillRate = 3;
		private bool triggered;

		public void Refill()
		{
			oxygen = Mathf.MoveTowards(oxygen, maxOxygen, refillRate * Time.deltaTime);
		}

		public bool IsDead
		{
			get => oxygen < 0;
		}

		private void Awake()
		{
			instance = this;
		}

		private void Start()
		{
			Reset();
		}

		public void Reset()
		{
			oxygen = maxOxygen;
			triggered = false;
		}

		private void Update()
		{
			oxygen -= Time.deltaTime;
			image.fillAmount = (oxygen / maxOxygen);

			if (IsDead && !triggered)
			{
				PlayerController.instance.Kill();
				triggered = true;
			}
		}
	}
}