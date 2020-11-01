using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HHGame.GameScripts
{
	[Serializable]
	internal class WanderLoop
	{
		public enum Mode
		{
			Move, Wait
		}

		public Vector2 moveTime = new Vector2(0.5f, 1.2f);
		public Vector2 waitTime = new Vector2(0.4f, 0.9f);

		[SerializeField] private SimpleTimer timer = new SimpleTimer();

		public Mode Current
		{
			get; private set;
		}

		public void Update()
		{
			timer.Update(Time.fixedDeltaTime);

			switch (Current)
			{
				case Mode.Move:
					if (timer.Done)
					{
						Current = Mode.Wait;
						timer.Start();
						timer.Duration = Random.Range(waitTime.x, waitTime.y);
					}
					break;
				case Mode.Wait:
					if (timer.Done)
					{
						Current = Mode.Move;
						timer.Start();
						timer.Duration = Random.Range(moveTime.x, moveTime.y);
					}
					break;
			}
		}
	}
}