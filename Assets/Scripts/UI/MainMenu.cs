using UnityEngine;

namespace Game.UI
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.Instance.LoadRuntimeScene();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
