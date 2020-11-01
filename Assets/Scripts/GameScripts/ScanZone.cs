using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	[Serializable]
	internal class ScanZone
	{
		public Collider2D area = default;
		public LayerMask mask = default;

		public void Scan(Predicate<Collider2D> match, Action<Collider2D> action)
		{
			Collider2D[] hits = new Collider2D[32];
			int count = area.OverlapCollider(new ContactFilter2D() { layerMask = mask }, hits);

			for (int i = 0; i < count; i++)
			{
				RaycastHit2D rc = Physics2D.Linecast(area.transform.position, hits[i].transform.position, mask);
				bool hasLoS = rc.collider == hits[i];
				Debug.DrawLine(area.transform.position, hits[i].transform.position, hasLoS ? Color.red : Color.white);
				if (hasLoS)
				{
					if (match(hits[i]))
					{
						action(hits[i]);
					}
				}
			}
		}
	}
}