﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace HHGame.GameScripts
{
	internal class OnHitGlow : MonoBehaviour, IHitTarget
	{
		void IHitTarget.Hit(bool doesDamage)
		{
			GetComponentInChildren<Light2D>().enabled = true;
		}
	}
}