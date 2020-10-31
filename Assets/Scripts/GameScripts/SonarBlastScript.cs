using System;
using System.Collections.Generic;
using UnityEngine;

namespace HHGame.GameScripts
{
	internal class SonarBlastScript : MonoBehaviour
	{
		public float segmentCount = 11;
		public float halfSpreadDeg = 30;
		public GameObject blastPrefab = default;

		public Collider2D hitArea = default;

		private HashSet<IHitTarget> blacklist;

		private void Start()
		{
			blacklist = new HashSet<IHitTarget>();

			Collider2D[] hits = new Collider2D[32];
			int hitCount = hitArea.OverlapCollider(default, hits);
			for (int i = 0; i < hitCount; i++)
			{
				IHitTarget hitObj = hits[i].GetComponent<IHitTarget>();
				if (hitObj != null)
				{
					hitObj.Hit(true);
					blacklist.Add(hitObj);
				}
			}

			for (int i = 0; i < segmentCount; i++)
			{
				float angleDeg = -halfSpreadDeg + i * 2 * halfSpreadDeg / (segmentCount - 1);
				var go = Instantiate(blastPrefab, transform.position, transform.rotation * Quaternion.Euler(0, 0, angleDeg));
				var script = go.GetComponent<SonarBlastSegmentScript>();
				script.parity = i;
				script.blacklist = blacklist;
			}

			Destroy(gameObject);
		}
	}
}