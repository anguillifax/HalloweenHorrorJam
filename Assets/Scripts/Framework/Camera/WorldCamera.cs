using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class WorldCamera : MonoBehaviour
	{
		// Fields

		public static WorldCamera instance;

		public Transform target = null;

		private CameraBase cam;
		[SerializeField] private ScreenshakeTool screenshake;
		private Vector2 basePos;
		private float baseZoom;
		private float zoomOffset;
		public RampedValue chargeZoom = default;
		public AnimationCurve fireShakeStrength = default;
		public SimpleTimer fireShakeTimer = new SimpleTimer(1);
		public ScreenshakeEffect testShake = default;
		private readonly ScreenshakeEffect fireShake = new ScreenshakeEffect();

		// Properties

		public ScreenshakeTool Screenshake
		{
			get => screenshake;
		}

		public float ZoomOffset
		{
			get => zoomOffset;
			set => zoomOffset = value;
		}

		public bool IsCharging
		{
			get; set;
		}

		// Methods

		private void Awake()
		{
			instance = this;
			cam = new CameraBase(transform, GetComponent<Camera>());
			baseZoom = cam.Zoom;
			screenshake.AddShake(fireShake);
			screenshake.AddShake(testShake);
			fireShakeTimer.Stop();
		}

		private void LateUpdate()
		{
			if (target != null)
				basePos = target.position;

			screenshake.Update();

			if (IsCharging)
				chargeZoom.Grow();
			else
				chargeZoom.Decay();

			fireShakeTimer.Update(Time.deltaTime);
			fireShake.strength = fireShakeStrength.Evaluate(fireShakeTimer.ElapsedTime);

			cam.Position = basePos + screenshake.CurrentOffset;
			cam.RotationDeg = screenshake.CurrentRotationDeg;
			cam.Zoom = baseZoom + zoomOffset + chargeZoom.Current;
		}

		public void TriggerFire()
		{
			fireShakeTimer.Start();
		}
	}
}