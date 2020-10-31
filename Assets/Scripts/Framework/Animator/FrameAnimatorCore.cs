using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	/// <summary>
	/// Plays back frame animations at a regular rate.
	/// 
	/// <para>
	/// The animator plays a single clip at a time by repeatedly setting the
	/// image on the sprite renderer to the current frame. The sprite renderer
	/// must be on the same gameobject as the frame animator.
	/// </para>
	/// 
	/// <para>
	/// The animator can be scheduled to play multiple clips in sequence. Each
	/// clip will be played exactly once before moving on to the next clip. The
	/// final animation in the queue is looped repeatedly until a user starts a
	/// new clip sequence.
	/// </para>
	/// </summary>
	///
	/// <remarks>
	/// This version is called "core" because it can be used to implement more
	/// complex frame animators, such as the common frame animator.
	/// </remarks>
	[RequireComponent(typeof(SpriteRenderer))]
	public class FrameAnimatorCore : MonoBehaviour
	{
		private const float MinPlaybackSpeed = 0.01f;

		// =========================================================
		// Representation
		// =========================================================

		[Tooltip("The animations that are scheduled to play (readonly)")]
		[SerializeField]
		private List<FrameAnimation> queue = new List<FrameAnimation>();

		[Tooltip("Controls the playback speed of all clips")]
		[SerializeField]
		[Min(MinPlaybackSpeed)]
		private float timescale = 1f;

		[Tooltip("Current frame to show (readonly)")]
		[SerializeField]
		private int frame;

		[Tooltip("The default animation clip to play on startup")]
		[SerializeField]
		private FrameAnimation DefaultClip = null;

		[Tooltip("Whether the animator should be affected by the global engine timescale")]
		[SerializeField]
		private bool useUnscaledTime = false;

		private float timeAccum;
		private bool finishedFlag;
		private SpriteRenderer sr;

		// =========================================================
		// Properties
		// =========================================================

		/// <summary>
		/// The current animation playing.
		/// </summary>
		public FrameAnimation CurrentAnimation
		{
			get => queue[0];
		}

		/// <summary>
		/// The last animation in the queue. After all other animation in the
		/// queue are played once, this animation will be played on loop.
		/// </summary>
		public FrameAnimation LastQueuedAnimation
		{
			get => queue[queue.Count - 1];
		}

		/// <summary>
		/// True if the subject is facing toward the right. Assigning to this
		/// property will flip the orientation of the sprite renderer.
		/// </summary>
		public bool TowardRight
		{
			get => !sr.flipX;
			set => sr.flipX = !value;
		}

		/// <summary>
		/// Controls the playback speed of all clips.
		/// </summary>
		public float Timescale
		{
			get => timescale;
			set
			{
				if (value <= MinPlaybackSpeed)
					timescale = MinPlaybackSpeed;
				else
					timescale = value;
			}
		}

		/// <summary>
		/// The current frame the animator is on. This number is in the range
		/// [0, length-1] where length is the number of frames in the
		/// aniamtion.
		/// </summary>
		public int CurrentFrame
		{
			get => frame;
		}

		public bool Finished
		{
			get => finishedFlag;
		}

		// =========================================================
		// Utility
		// =========================================================

		private void CheckValid(FrameAnimation anim)
		{
			if (anim == null)
				throw new ArgumentException("Animation clip not found");
			anim.AssertValid();
		}

		/// <summary>
		/// Interrupt the current clip and play a new sequence of animations
		/// starting at the given frame.
		/// 
		/// <para>
		/// Each animation in the sequence is played exactly once before moving
		/// on to the next frame. There must be at least one animation clip in
		/// the sequence, otherwise an exception is thrown. None of the clips
		/// may be null.
		/// </para>
		/// 
		/// <para>
		/// Only the first clip is starts from the specified initial frame. All
		/// following clips will start from frame 0.
		/// </para>
		/// 
		/// <para>
		/// This method verifies the invariants of a frame animation. See <see
		/// cref="FrameAnimation.AssertValid"/> for details.
		/// </para>
		/// </summary>
		/// 
		/// <param name="initialFrame">Frame to start the first clip on.</param>
		/// <param name="clips">A sequence of clips to be played one after another.</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="IndexOutOfRangeException"/>
		public void BeginSequenceAtFrame(int initialFrame, params FrameAnimation[] clips)
		{
			if (clips == null)
				throw new ArgumentNullException();

			if (clips.Length == 0)
				throw new ArgumentException("Clip sequence cannot be empty");

			foreach (FrameAnimation anim in clips)
				CheckValid(anim);

			if (!(0 <= initialFrame && initialFrame < clips[0].frames.Length))
				throw new IndexOutOfRangeException("Initial frame out of bounds of initial clip");

			queue.Clear();
			queue.AddRange(clips);
			frame = initialFrame;
			finishedFlag = false;
		}

		/// <summary>
		/// Interrupt the current clip and play a new sequence of animations.
		/// 
		/// <para>
		/// Each animation in the sequence is played exactly once before moving
		/// on to the next frame. There must be at least one animation clip in
		/// the sequence, otherwise an exception is thrown. None of the clips
		/// may be null.
		/// </para>
		/// 
		/// <para>
		/// This method verifies the invariants of a frame animation. See <see
		/// cref="FrameAnimation.AssertValid"/> for details.
		/// </para>
		/// </summary>
		/// 
		/// <param name="clips">A sequence of clips to be played one after another.</param>
		/// <exception cref="ArgumentException"/>
		/// <exception cref="ArgumentNullException"/>
		public void BeginSequence(params FrameAnimation[] clips)
		{
			BeginSequenceAtFrame(0, clips);
		}

		// =========================================================
		// Implementation
		// =========================================================

		protected void Awake()
		{
			sr = GetComponent<SpriteRenderer>();
			if (DefaultClip == null)
			{
				Debug.LogError("A default clip must be assigned", this);
				enabled = false;
			}
		}

		protected void OnEnable()
		{
			try
			{
				BeginSequence(DefaultClip);
			}
			catch (Exception e)
			{
				enabled = false;
				Debug.LogException(e, this);
			}
		}

		protected void Update()
		{
			if (TimeManager.Paused) return;

			timeAccum += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * timescale;

			while (!finishedFlag)
			{
				float delay = 1f / CurrentAnimation.framerate;
				if (timeAccum < delay) break;

				++frame;
				if (frame >= CurrentAnimation.frames.Length)
				{
					// Move to the next clip if there is one
					if (queue.Count > 1)
					{
						queue.RemoveAt(0);
					}

					// Loop if not on last clip and clip has loop set to false
					if (queue.Count == 1 && !CurrentAnimation.loop)
					{
						frame = CurrentAnimation.frames.Length - 1;
						finishedFlag = true;
					}
					else
					{
						frame = 0;
					}
				}

				timeAccum -= delay;
			}

			sr.sprite = CurrentAnimation.frames[frame];
		}
	}
}