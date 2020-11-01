using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HHGame.GameScripts
{
	internal class LungeEnemy : MonoBehaviour, IHitTarget
	{
		// =========================================================
		// Types
		// =========================================================

		public enum State
		{
			Wander, Follow, LungeWait, Lunge, Stun, StayHere, Die, Roar
		}

		// =========================================================
		// Fields
		// =========================================================

		[Header("Common")]
		public State state;
		public TrackFollower trackFollow = default;
		public WanderLoop wander = default;
		public ScanZone scanner = default;
		public GameObject visibility = default;

		[Header("Wander")]
		public float incrementDist = 0.5f;
		public float velocity = 5;
		public float velocityDeg = 20;

		[Header("Follow")]
		public float followVel = 5;
		public float followAccel = 8;
		public float followRangeTrigger = 4;

		[Header("Lunge Wait")]
		public SimpleTimer waitTimer;
		public float waitDecel = 5;
		public float waitRotateDeg = 30f;
		public float waitCloseAngleDeg = 10f;

		[Header("Lunge")]
		public SimpleTimer lungeTimer = default;
		public AnimationCurve lungeVel = default;

		[Header("Stun")]
		public SimpleTimer stunTimer = default;

		[Header("Stay Here")]
		public float stayReagroRange = 8f;

		[Header("Roar")]
		public SimpleTimer roarTimer = default;

		[Header("Die")]
		public SimpleTimer dieTimer = default;
		public float dieDecel = 20;

		private Rigidbody2D body;
		private FrameAnimator anim;
		private bool _enraged;

		// =========================================================
		// Properties
		// =========================================================

		public bool Enraged
		{
			get => _enraged;
			set
			{
				_enraged = value;
				visibility.SetActive(value);
			}
		}

		// =========================================================
		// Methods
		// =========================================================

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			anim = GetComponent<FrameAnimator>();
		}

		private void Start()
		{
			state = State.Wander;
			trackFollow.Init();
			body.position = trackFollow.CurrentPosition;
			Enraged = false;
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

			scanner.Scan(x => x.CompareTag("Player"), x =>
			{
				state = State.Follow;
				Enraged = true;
			});
		}

		private void StayHere()
		{
			body.velocity = Vector2.zero;
			scanner.Scan(x => x.CompareTag("Player"), x => state = State.Follow);

			if (PlayerController.instance != null && Vector2.Distance(PlayerController.instance.transform.position, transform.position) < stayReagroRange)
			{
				StunBegin();
			}
		}

		private void Follow()
		{
			if (PlayerController.instance != null)
			{
				Vector2 target = PlayerController.instance.transform.position;
				Vector2 setpoint = followVel * (target - body.position).normalized;
				body.velocity = Vector2.MoveTowards(body.velocity, setpoint, followAccel * Time.fixedDeltaTime);

				float angDeg = body.rotation;
				if (PlayerController.instance != null)
				{
					angDeg = -Vector2.SignedAngle((PlayerController.instance.transform.position - transform.position).normalized, Vector2.up);
				}
				body.MoveRotation(Mathf.MoveTowardsAngle(body.rotation, angDeg, waitRotateDeg * Time.fixedDeltaTime));

				if (Vector2.Distance(target, body.position) < followRangeTrigger)
				{
					state = State.LungeWait;
					waitTimer.Start();
				}
			}
			else
			{
				state = State.Wander;
			}
		}

		private void LungeWait()
		{
			waitTimer.Update(Time.fixedDeltaTime);
			body.velocity = Vector2.MoveTowards(body.velocity, Vector2.zero, waitDecel * Time.fixedDeltaTime);
			float angDeg = body.rotation;
			if (PlayerController.instance != null)
			{
				angDeg = -Vector2.SignedAngle((PlayerController.instance.transform.position - transform.position).normalized, Vector2.up);
			}
			body.MoveRotation(Mathf.MoveTowardsAngle(body.rotation, angDeg, waitRotateDeg * Time.fixedDeltaTime));
			if (waitTimer.Done && Mathf.Abs(Mathf.DeltaAngle(body.rotation, angDeg)) < waitCloseAngleDeg)
			{
				state = State.Lunge;
				lungeTimer.Start();
			}
			if (PlayerController.instance == null)
			{
				state = State.StayHere;
			}
		}

		private void Lunge()
		{
			lungeTimer.Update(Time.fixedDeltaTime);
			body.velocity = transform.up * lungeVel.Evaluate(lungeTimer.NormalizedProgress);
			if (lungeTimer.Done)
			{
				StunBegin();
			}
		}

		private void StunBegin()
		{
			state = State.Stun;
			stunTimer.Start();
		}

		private void Stun()
		{
			stunTimer.Update(Time.fixedDeltaTime);
			state = PlayerController.instance != null ? State.Follow : State.StayHere;
		}

		private void Die()
		{
			dieTimer.Update(Time.fixedDeltaTime);
			body.velocity = Vector2.MoveTowards(body.velocity, Vector2.zero, dieDecel * Time.fixedDeltaTime);
			if (dieTimer.Done)
			{
				Destroy(gameObject);
				visibility.SetActive(false);
			}
		}

		private void Roar()
		{
			roarTimer.Update(Time.fixedDeltaTime);
			if (roarTimer.Done)
			{
				state = State.Follow;
			}
		}

		private void FixedUpdate()
		{
			switch (state)
			{
				case State.Wander: Wander(); break;
				case State.Follow: Follow(); break;
				case State.LungeWait: LungeWait(); break;
				case State.Lunge: Lunge(); break;
				case State.Stun: Stun(); break;
				case State.StayHere: StayHere(); break;
				case State.Die: Die(); break;
				case State.Roar: Roar(); break;
			}
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				if (Enraged)
				{
					col.GetComponent<PlayerController>().Kill();
				}
				else
				{
					Enraged = true;
					state = State.Roar;
					roarTimer.Start();
				}
			}
		}

		void IHitTarget.Hit(bool doesDamage)
		{
			if (doesDamage && (state == State.Stun || state == State.Lunge))
			{
				state = State.Die;
				dieTimer.Start();
			}
			else
			{
				Enraged = true;
				StunBegin();
			}
		}
	}
}