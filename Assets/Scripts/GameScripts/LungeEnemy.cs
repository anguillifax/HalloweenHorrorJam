using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
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
			Wander, Follow, LungeWait, Lunge, Stun, StayHere, Die, Roar, Dummy
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
		public Light2D glow = default;
		public Vector2 glowRange = new Vector2(0.3f, 1);
		public GameObject lungeShake = default;

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
		public float stayRewakeDelay = 0.8f;

		[Header("Roar")]
		public SimpleTimer roarTimer = default;

		[Header("Die")]
		public GameObject diePrefab = default;
		public float diePrefabLifetime = 2f;
		public SimpleTimer dieTimer = default;
		public float dieDecel = 20;

		[Header("Sound")]
		public AudioClip soundEnrage = default;
		public AudioClip soundMove = default;
		public AudioClip soundMove2 = default;
		public AudioClip soundCharge = default;
		public AudioClip soundStun = default;
		public AudioClip soundDie = default;

		private Rigidbody2D body;
		private FrameAnimator anim;
		private AudioSource source;
		private bool _enraged;
		private bool hasPlayedMoveSound = false;

		// =========================================================
		// Properties
		// =========================================================

		private void PlayEnrage()
		{
			source.PlayOneShot(soundEnrage, 0.7f);
		}

		public bool Enraged
		{
			get => _enraged;
			set
			{
				if (!_enraged && value)
				{
					Invoke(nameof(PlayEnrage), Random.Range(0, 0.2f));
					Instantiate(lungeShake);
				}
				_enraged = value;
				visibility.SetActive(value);
			}
		}

		private bool Vulnerable
		{
			get => state == State.Lunge || state == State.Stun;
		}

		// =========================================================
		// Setup
		// =========================================================

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			anim = GetComponent<FrameAnimator>();
			source = GetComponent<AudioSource>();
		}

		private void Start()
		{
			state = State.Wander;
			trackFollow.Init(transform.position);
			body.position = trackFollow.CurrentPosition;
			Enraged = false;
		}

		// =========================================================
		// Update
		// =========================================================

		private void Update()
		{
			glow.intensity = Vulnerable ? glowRange.y : glowRange.x;
		}

		// =========================================================
		// Fixed Update
		// =========================================================

		private void Wander()
		{
			wander.Update();

			if (wander.Current == WanderLoop.Mode.Move)
			{
				if (!hasPlayedMoveSound)
				{
					source.PlayOneShot(Random.value < 0.5f ? soundMove : soundMove2, 0.05f);
					hasPlayedMoveSound = true;
				}
				if (Vector2.Distance(body.position, trackFollow.CurrentPosition) < incrementDist)
				{
					trackFollow.Next();
				}
				body.MovePosition(
					Vector2.MoveTowards(body.position, trackFollow.CurrentPosition, velocity * Time.fixedDeltaTime));
				body.MoveRotation(
					Mathf.MoveTowardsAngle(body.rotation, Vector2.SignedAngle(Vector2.up, trackFollow.CurrentDirection), velocityDeg * Time.fixedDeltaTime));
			}
			else
			{
				hasPlayedMoveSound = false;
			}

			scanner.Scan(x => x.CompareTag("Player"), x =>
			{
				state = State.Follow;
				Enraged = true;
			});
		}

		private void Reawaken()
		{
			Enraged = true;
			StunBegin();
		}

		private void StayHere()
		{
			body.velocity = Vector2.zero;

			if (PlayerController.instance != null
				&& Vector2.Distance(PlayerController.instance.transform.position, transform.position) < stayReagroRange
				&& !PlayerController.isDead)
			{
				Invoke(nameof(Reawaken), stayRewakeDelay);
				state = State.Dummy;
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
				source.PlayOneShot(soundCharge, 0.8f);
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
			source.PlayOneShot(soundStun);
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
			source.volume = Mathf.MoveTowards(source.volume, 0, 2f * Time.fixedDeltaTime);
			if (dieTimer.Done)
			{
				Destroy(gameObject);
				Destroy(Instantiate(diePrefab, transform.position, transform.rotation), diePrefabLifetime);
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
			if (PlayerController.isDead)
			{
				state = State.StayHere;
				Enraged = false;
			}
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
					Enraged = false;
					state = State.StayHere;
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
			if (doesDamage && Vulnerable)
			{
				if (state != State.Die)
				{
					state = State.Die;
					GlobalSoundSource.Play(soundDie);
					dieTimer.Start();
				}
			}
			else
			{
				Enraged = true;
				StunBegin();
			}
		}
	}
}