using UnityEngine;

namespace HHGame
{
	public class CameraBase
	{
		// =========================================================
		// Representation
		// =========================================================

		private readonly Transform transform;
		private readonly Camera cam;

		// =========================================================
		// Properties
		// =========================================================

		public Vector2 Position
		{
			get => transform.localPosition;
			set => transform.localPosition = new Vector3(value.x, value.y, transform.localPosition.z);
		}

		public float RotationDeg
		{
			get => transform.localEulerAngles.z;
			set => transform.localEulerAngles = new Vector3(0, 0, value);
		}

		public float Zoom
		{
			get => cam.orthographicSize;
			set => cam.orthographicSize = value;
		}

		public Camera Component
		{
			get => cam;
		}

		// =========================================================
		// Methods
		// =========================================================

		public CameraBase(Transform transform, Camera cam)
		{
			this.transform = transform;
			this.cam = cam;
		}
	}
}