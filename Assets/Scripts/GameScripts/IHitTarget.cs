using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal interface IHitTarget
	{
		void Hit(bool doesDamage);
	}
}