using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HHGame.GameScripts
{
	internal class LungeEnemy : MonoBehaviour
	{
		// =========================================================
		// Types
		// =========================================================

		public enum State
		{
			Wander, Lunge, Stun, Die
		}

		// =========================================================
		// Fields
		// =========================================================

		[Header("Common")]
		public State state;
		public TrackFollower trackFollow = default;
		public WanderLoop wander = default;
		public ScanZone scanner = default;

		[Header("Wander")]
		public float incrementDist = 0.5f;
		public float velocity = 5;
		public float velocityDeg = 20;

		[Header("Lunge")]
		public float lungeWait = 0.4f;

		private Rigidbody2D body;

		// =========================================================
		// Methods
		// =========================================================

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
		}

		private void Start()
		{
			state = State.Wander;
			trackFollow.Init();
			body.position = trackFollow.CurrentPosition;
		}

		private void SearchPlayer()
		{
			scanner.Scan(x => x.CompareTag("Player"), x =>
			{
				Debug.Log(name + " found player");
				StartCoroutine(LungeCoroutine());
			});
		}

		private void Wander()
		{
			wander.Update();
			if (wander.Current == WanderLoop.Mode.Move)
			{
				if (Vector2.Distance(body.position, trackFollow.CurrentPosition) < incrementDist)
				{
					trackFollow.Next();
				}
				body.MovePosition(
					Vector2.MoveTowards(body.position, trackFollow.CurrentPosition, velocity * Time.fixedDeltaTime));
				body.MoveRotation(
					Mathf.MoveTowardsAngle(body.rotation, Vector2.SignedAngle(Vector2.up, trackFollow.CurrentDirection), velocityDeg * Time.fixedDeltaTime));
			}
			SearchPlayer();
		}
		
		private IEnumerator LungeCoroutine()
		{
			//state = State.Lunge;

			yield return new WaitForSeconds(lungeWait);
		}

		private void FixedUpdate()
		{
			switch (state)
			{
				case State.Wander: Wander(); break;
				case State.Lunge: break;
				case State.Stun:
					break;
				case State.Die:
					break;
			}
		}
	}
}