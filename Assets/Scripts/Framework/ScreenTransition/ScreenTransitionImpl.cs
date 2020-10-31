using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HHGame
{
	internal class ScreenTransitionImpl : MonoBehaviour
	{
		// =========================================================
		// Variables
		// =========================================================

		public Material refMaterial = default;

		private readonly ScreenTransitionTool iface = new ScreenTransitionTool();
		private RawImage img;
		private Material mat;

		// =========================================================
		// Methods
		// =========================================================

		private void Awake()
		{
			if (ScreenTransition.impl != null)
			{
				Debug.LogWarning("There can only be screen transition implementation active. Destroying copy.");
				Destroy(this);
				return;
			}
			ScreenTransition.impl = this;

			mat = new Material(refMaterial);
			img = gameObject.AddComponent<RawImage>();
			img.material = mat;

			mat.SetFloat("_Progress", 0.0f);
			mat.SetVector("_Position", Vector4.zero);
			mat.SetInt("_Mode", 0);
			OnGameResize();
			img.SetMaterialDirty();

			ScreenUtil.Resized += OnGameResize;
		}

		private void OnDestroy()
		{
			ScreenUtil.Resized -= OnGameResize;
		}

		private void OnGameResize()
		{
			mat.SetVector("_Size", new Vector4(Screen.width, Screen.height, 0, 0));
		}

		private void Update()
		{
			if (iface.Dirty)
			{
				mat.SetFloat("_Progress", iface.Progress);
				mat.SetVector("_Position", iface.ScreenPosition);
				mat.SetInt("_Mode", (int)iface.TransitionType);
				img.SetMaterialDirty();
			}
		}

		public void BeginTransition(Func<ScreenTransitionTool, IEnumerator> func)
		{
			StopAllCoroutines();
			StartCoroutine(func(iface));
		}
	}
}