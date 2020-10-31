using UnityEngine;

namespace HHGame {

	/// <summary>
	/// Sets the current game object as DoNotDestroyOnLoad.
	/// </summary>
	public class SetDoNotDestroyOnLoad : MonoBehaviour {

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

	}

}