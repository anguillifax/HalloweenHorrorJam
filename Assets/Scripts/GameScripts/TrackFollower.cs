using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	[Serializable]
	internal class TrackFollower
	{
		// Fields

		public LineRenderer track = default;
		private int curid;

		// Properties

		public Vector2 CurrentPosition
		{
			get; private set;
		}

		public Vector2 CurrentDirection
		{
			get; private set;
		}

		private Vector2 CurrentVertex
		{
			get => track.GetPosition(curid) + track.transform.position;
		}

		private Vector2 NextVertex
		{
			get => track.GetPosition((curid + 1) % track.positionCount) + track.transform.position;
		}

		// Methods

		public void Init(Vector2 pos)
		{
			int closest = 0;
			for (int i = 1; i < track.positionCount; i++)
			{
				if (Vector2.Distance(track.GetPosition(i), pos) < Vector2.Distance(track.GetPosition(closest), pos))
				{
					closest = i;
				}
			}
			curid = closest;
			Update();
		}

		private void Update()
		{
			CurrentPosition = NextVertex;
			CurrentDirection = (NextVertex - CurrentVertex).normalized;
		}

		public void Next()
		{
			curid = (curid + 1) % track.positionCount;
			Update();
		}
	}
}