using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class SonarBlastSegmentScript : MonoBehaviour
	{
		public HashSet<IHitTarget> blacklist = null;
		[HideInInspector] public int parity;

		public int spawnMod = 3;
		public SimpleTimer lifetime = default;
		public float velocity = 6;
		public AnimationCurve lightRamp = default;
		public SimpleTimer trailDelay = default;
		public GameObject trailPrefab = default;
		public GameObject trailBgPrefab = default;
		public GameObject bubbleParticles = default;
		public float hitLifetimeDecay = 0.3f;

		private Rigidbody2D body;
		private Light2D glow;

		private void Awake()
		{
			body = GetComponent<Rigidbody2D>();
			glow = GetComponent<Light2D>();
			Instantiate(bubbleParticles, transform.position, Quaternion.identity).GetComponent<BubbleSystemFollow>().parent = transform;
		}

		private void Start()
		{
			lifetime.Start();
			trailDelay.Start();
			body.velocity = transform.rotation * new Vector2(0, velocity);
		}

		private void Update()
		{
			trailDelay.Update(Time.deltaTime);
			if (trailDelay.Done)
			{
				parity = (parity + 1) % spawnMod;
				if (parity == 0)
				{
					var go = Instantiate(trailPrefab, transform.position, Quaternion.identity);
					go.GetComponent<LightGlow>().factor = 1 - lifetime.NormalizedProgress;
				}
				if (parity == 1)
				{
					var go = Instantiate(trailBgPrefab, transform.position, Quaternion.identity);
					go.GetComponent<LightGlow>().factor = 1 - lifetime.NormalizedProgress;
				}
				trailDelay.Start();
			}
		}

		private void FixedUpdate()
		{
			lifetime.Update(Time.fixedDeltaTime);
			glow.intensity = lightRamp.Evaluate(lifetime.NormalizedProgress);

			if (lifetime.Done)
			{
				Destroy(gameObject);
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			lifetime.Update(hitLifetimeDecay);
		}

		private void OnTriggerEnter2D(Collider2D col)
		{
			IHitTarget hitObj = col.GetComponent<IHitTarget>();
			if (hitObj != null)
			{
				if (!blacklist.Contains(hitObj))
				{
					blacklist.Add(hitObj);
					hitObj.Hit(false);
				}
			}
		}
	}
}