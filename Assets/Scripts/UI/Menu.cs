using UnityEngine;

namespace Game.UI
{
    public class MenuUI : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        public GameObject Menu;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        void Resume()
        {
            Menu.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        void Pause()
        {
			// Don't open menu if currently on game-over screen.
			if (GameManager.Instance.InGameOver)
			{
				return;
			}

			Menu.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }
}
