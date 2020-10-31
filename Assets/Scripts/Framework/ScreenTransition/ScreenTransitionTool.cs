using UnityEngine;

namespace HHGame
{
	public class ScreenTransitionTool
	{
		// =========================================================
		// Variables
		// =========================================================

		private Vector2 _position;
		private float _progress;
		private ScreenTransitionType _type;
		private bool _dirty;

		// =========================================================
		// Properties
		// =========================================================

		public Vector2 ScreenPosition
		{
			get => _position;
			set
			{
				_position = value;
				_dirty = true;
			}
		}

		public float Progress
		{
			get => _progress;
			set
			{
				_progress = value;
				_dirty = true;
			}
		}
		public ScreenTransitionType TransitionType
		{
			get => _type;
			set
			{
				_type = value;
				_dirty = true;
			}
		}

		internal bool Dirty
		{
			get => _dirty;
		}
	}
}