using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame
{
	/// <summary>
	/// Send an interaction to all target objects without repeats.
	/// </summary>
	internal class InteractOnceUtil
	{
		//private readonly HashSet<T> blacklist = new HashSet<T>();

		//public void Reset()
		//{
		//	blacklist.Clear();
		//}

		//public void Interact(Interaction interaction, IEnumerable<Component> candidates, Action successCallback = null)
		//{
		//	foreach (var t in candidates)
		//	{
		//		var i = t.GetComponent<IInteractionTarget>();
		//		if (i != null && !blacklist.Contains(i))
		//		{
		//			blacklist.Add(i);
		//			i.Interact(interaction);
		//			successCallback?.Invoke();
		//		}
		//	}
		//}
	}
}