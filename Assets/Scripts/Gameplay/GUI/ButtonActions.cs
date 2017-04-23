using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Ldp {
    [RequireComponent(typeof(Button))]
    public class ButtonActions : MonoBehaviour {
        // Actions that a button can do
        public GameObject menuToOpen;
        public bool isPopup;
        public bool customMenu;

        void Start() {
            Button thisButton = GetComponent<Button>();
            // LATER
            thisButton.onClick.AddListener(() =>
            {
                AssetsHolderManager.Get.ButtonClick();
            });

            if (menuToOpen != null && customMenu)
            {
                thisButton.onClick.AddListener(() =>
                {
                    MenuManager.Get.Open(menuToOpen, isPopup);
                });
            }
        }

        public void Play()
        {
            SceneManager.LoadScene("MainGameplay");
        }

        public void Replay()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Pause()
        {
            GameManager.Get.Pause();
            if (GameManager.Get.isPaused)
            {
                MenuManager.Get.Open(menuToOpen, isPopup);
            } else
            {
                MenuManager.Get.Close();
            }
        }

        public void Back()
        {
            MenuManager.Get.Close();
        }

        public void Surrender()
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync("MainMenu");
            GameManager.Get.isGameOver = true;
        }

        public void Quit()
        {
            GameManager.Get.Quit();
        }
    }
}