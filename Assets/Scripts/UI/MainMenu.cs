using UnityEngine;

namespace Game.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
			SceneManager.Instance.LoadRuntimeScene(0);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
