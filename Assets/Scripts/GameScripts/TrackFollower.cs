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
		public int startId = 0;

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

		public void Init()
		{
			for (int i = 0; i < track.positionCount; i++)
			{
				curid = i;
				if (Vector2.Distance(CurrentVertex, NextVertex) == 0)
				{
					Debug.LogWarning("Overlapping points", track);
				}
			}
			curid = startId;
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