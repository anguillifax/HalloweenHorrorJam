using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class PlayerController : MonoBehaviour
	{
		// =========================================================
		// Types
		// =========================================================

		public enum State
		{
			Swim, Attack, Die
		}

		// =========================================================
		// Fields
		// =========================================================

		public static PlayerController instance;
		public static bool isDead;

		[Header("Common")]
		public State state;
		public SimpleTimer cooldown = default;

		[Header("Swim")]
		public float swimVel = 8;
		public float swimVelCharge = 4;
		public RampedValue swimAccel = default;
		public SimpleTimer swimChargeTime = default;

		[Header("Attack")]
		public GameObject attackPrefabBlast = default;
		public Transform attackMuzzleOrigin = default;
		public SimpleTimer attackDuration = default;
		public float attackHit = 0.4f;
		public AnimationCurve attackCurve = default;
		public float attackInputVel = 2;
		public float attackInputAccel = 10;
		public float attackDecayAccel = 10;
		public float attackCooldown = 0.4f;
		public Light2D attackLumLight = default;
		public AnimationCurve attackLumFactor = default;
		public SimpleTimer attackLumTimer = default;
		private Vector2 attackCurDecay;
		private Vector2 attackCurInput;
		private Vector2 attackDir;

		[Header("Die")]
		public GameObject diePrefab = default;
		public GameObject dieShake = default;
		public SimpleTimer dieTimer = default;
		public float dieAccel = 20;

		[Header("Sound")]
		public AudioSource soundMove = default;
		public AudioSource soundCharge = default;
		public AudioClip soundFire = default;
		public AudioClip soundDie = default;

		private Vector2 inputAxes;
		private Vector2 inputAxesClamp;
		private float inputAttackTimestamp;
		private bool inputAttack;

		private Rigidbody2D body;
		private SpriteRenderer spriteRen;
		private FrameAnimator animator;
		private Camera mainCam;

		// =========================================================
		// Properties
		// =========================================================

		private Vector2 LookDir
		{
			get => (mainCam.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
		}

		private float LookAngleDeg
		{
			get => -Vector2.SignedAngle(LookDir, Vector2.up);
		}

		private WorldCamera WorldCam
		{
			get => WorldCamera.instance;
		}

		// =========================================================
		// Interface
		// =========================================================

		public void Kill()
		{
			if (state != State.Die)
			{
				Instantiate(dieShake);
				isDead = true;
				state = State.Die;
				GlobalSoundSource.Play(soundDie);
			}
		}

		// =========================================================
		// Setup
		// =========================================================

		private void Awake()
		{
			instance = this;
			isDead = false;

			body = GetComponent<Rigidbody2D>();
			spriteRen = GetComponent<SpriteRenderer>();
			animator = GetComponent<FrameAnimator>();
			mainCam = Camera.main;
		}

		private void Start()
		{
			WorldCamera.instance.target = transform;
			body.velocity = Vector2.zero;
			inputAttackTimestamp = -1000;
			cooldown.Stop();
			attackLumTimer.Stop();
			StateSwimBegin();
		}

		// =========================================================
		// Update
		// =========================================================

		private void Update()
		{
			inputAxes = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			inputAxesClamp = Vector2.ClampMagnitude(inputAxes, 1f);
			if (Input.GetButtonDown("Fire1"))
			{
				inputAttackTimestamp = Time.time;
			}
			inputAttack = Input.GetButton("Fire1");

			attackLumTimer.Update(Time.deltaTime);
			attackLumLight.intensity = attackLumFactor.Evaluate(attackLumTimer.NormalizedProgress);

			if (state == State.Swim && inputAxes.sqrMagnitude > 0 && !soundMove.isPlaying)
				soundMove.Play();

			soundCharge.volume = Mathf.MoveTowards(soundCharge.volume, inputAttack ? 1 : 0, 3 * Time.fixedDeltaTime);
		}

		// =========================================================
		// FixedUpdate
		// =========================================================

		private void StateSwimBegin()
		{
			state = State.Swim;

			swimAccel.SetMin();
			swimChargeTime.Start();
		}

		private void StateSwim()
		{
			if (inputAxes.sqrMagnitude > 0 && !inputAttack)
				swimAccel.Grow();
			else
				swimAccel.Decay();

			Vector2 setpoint = (inputAttack ? swimVelCharge : swimVel) * inputAxesClamp;
			body.velocity = Vector2.MoveTowards(body.velocity, setpoint, swimAccel.Current * Time.fixedDeltaTime);

			body.MoveRotation(-Vector2.SignedAngle(LookDir, Vector2.up));

			if (swimChargeTime.Done && !inputAttack)
			{
				StateAttackBegin();
			}

			if (inputAttack)
			{
				swimChargeTime.Update(Time.fixedDeltaTime);
			}
			else
			{
				swimChargeTime.Start();
			}

			WorldCam.IsCharging = inputAttack;
		}

		private void StateAttackBegin()
		{
			state = State.Attack;

			attackDuration.Start();
			attackCurInput = Vector2.zero;
			attackCurDecay = body.velocity;
			attackDir = LookDir;
			WorldCam.TriggerFire();
			Instantiate(attackPrefabBlast, attackMuzzleOrigin.position, Quaternion.Euler(0, 0, LookAngleDeg));
			attackLumTimer.Start();
			GlobalSoundSource.Play(soundFire);
		}

		private void StateAttack()
		{
			attackDuration.Update(Time.fixedDeltaTime);

			attackCurDecay = Vector2.MoveTowards(attackCurDecay, Vector2.zero, attackDecayAccel * Time.fixedDeltaTime);
			attackCurInput = Vector2.MoveTowards(attackCurInput, inputAxesClamp * attackInputVel, attackInputAccel * Time.fixedDeltaTime);
			Vector2 moveVel = attackCurve.Evaluate(attackDuration.ElapsedTime) * attackDir;
			body.velocity = attackCurDecay + attackCurInput + moveVel;

			if (attackDuration.ElapsedTime > attackHit)
			{
				body.MoveRotation(LookAngleDeg);
			}

			if (attackDuration.Done)
			{
				cooldown.Duration = attackCooldown;
				cooldown.Start();
				StateSwimBegin();
			}
		}

		private void FixedUpdate()
		{
			cooldown.Update(Time.fixedDeltaTime);
			switch (state)
			{
				case State.Swim: StateSwim(); break;
				case State.Attack: StateAttack(); break;
				case State.Die:
					{
						body.velocity = Vector2.MoveTowards(body.velocity, Vector2.zero, dieAccel * Time.fixedDeltaTime);
						dieTimer.Update(Time.fixedDeltaTime);

						if (dieTimer.Done)
						{
							Instantiate(diePrefab);
							Destroy(gameObject);
						}
					}
					break;
			}
		}
	}
}