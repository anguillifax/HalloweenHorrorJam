using UnityEngine;

namespace HHGame.GameScripts
{
	internal class GamePause : MonoBehaviour
	{
		private bool _paused;

		public GameObject pauseCanvas = default;

		private bool Paused
		{
			get => _paused;
			set
			{
				if (_paused != value)
				{
					pauseCanvas.SetActive(value);
					Time.timeScale = value ? 0 : 1;
					_paused = value;
				}
			}
		}

		private void Awake()
		{
			_paused = true;
			Paused = false;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Paused = !Paused;
			}
		}
	}
}