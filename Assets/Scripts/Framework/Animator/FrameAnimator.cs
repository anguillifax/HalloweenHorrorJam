using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HHGame
{
	/// <summary>
	/// Plays frame-by-frame animations from a folder in <c>Resources</c>.
	/// 
	/// <para>
	/// The animator will automatically play a transition clips that match the
	/// naming convention <c>ClipFrom to ClipTo</c>.
	/// </para>
	/// 
	/// <para>
	/// For additional details, see the base class <see cref="FrameAnimatorCore"/>.
	/// </para>
	/// </summary>
	public class FrameAnimator : FrameAnimatorCore
	{
		// =========================================================
		// Representation
		// =========================================================

		[SerializeField]
		[Tooltip("The subfolder in Resources to read animation clips from")]
		private string animationDirectory = string.Empty;

		private Dictionary<string, FrameAnimation> clipBank = null;

		// =========================================================
		// Properties
		// =========================================================

		/// <summary>
		/// Where the frame animator is reading animation clips from.
		/// </summary>
		public string AnimationDirectory
		{
			get => AnimationDirectory;
		}

		// =========================================================
		// Utility
		// =========================================================

		private FrameAnimation GetClip(string id)
		{
			if (clipBank.TryGetValue(id, out var v))
				return v;
			else
				return null;
		}

		/// <summary>
		/// Interrupt the currently playing clips and play a new clip.
		/// 
		/// <para>
		/// The animator will automatically play a transition clip between two
		/// clips if the transition exists. If no transition is found, the animator
		/// will jump directly to playing the specified clip. Any clip named in
		/// the format <c>ClipFrom to ClipTo</c>, is considered a transition.
		/// </para>
		/// 
		/// </summary>
		/// <param name="clipName"></param>
		public void Play(string clipName)
		{
			if (!enabled) return;

			if (LastQueuedAnimation.name != clipName)
			{
				FrameAnimation transition = GetClip($"{LastQueuedAnimation.name} to {clipName}");
				if (transition == null)
				{
					GetClip($"Any to {clipName}");
				}

				FrameAnimation end = GetClip(clipName);

				if (end == null)
					throw new ArgumentException($"Animation clip `{clipName}` not found");

				if (transition == null)
					BeginSequence(end);
				else
					BeginSequence(transition, end);
			}
		}

		// =========================================================
		// Implementation
		// =========================================================

		new protected void Awake()
		{
			base.Awake();

			//if (string.IsNullOrWhiteSpace(animationDirectory))
			//{
			//	Debug.LogError("Frame animator had no animation directory set.");
			//	enabled = false;
			//	return;
			//}

			if (clipBank == null)
			{
				clipBank = Resources.LoadAll<FrameAnimation>(animationDirectory)
					.ToDictionary(x => x.name, x => x);
				if (clipBank.Count == 0)
				{
					Debug.LogWarning("Frame animator directory contained no clips. Check if path is correct.", this);
				}
			}
		}
	}
}