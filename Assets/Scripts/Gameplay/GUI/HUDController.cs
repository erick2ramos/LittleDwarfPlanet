using UnityEngine;
using UnityEngine.UI;

namespace Ldp
{
    public class HUDController : MonoBehaviour
    {
        public RectTransform fuelFill;
        public Text timerCountdown;
        public Text scoreText;

        private float initialFuelFillHeight;

        void Start()
        {
            initialFuelFillHeight = fuelFill.sizeDelta.y;
            // Looking to initialize the GameManager singleton
        }

        void Update()
        {
            if (!GameManager.Get.isPaused)
            {
                // Handles the fuel bar on the HUD
                fuelFill.sizeDelta = new Vector2(fuelFill.sizeDelta.x,
                (GameManager.Get.Player.currentFuelQty / GameManager.Get.Player.maxFuelQty) * initialFuelFillHeight);

                timerCountdown.text = GameManager.Get.playTimer.ToString("0");
                if (GameManager.Get.playTimer <= 20f)
                {
                    timerCountdown.color = new Color32(255, 0, 0, 255);
                }

                scoreText.text = GameManager.Get.Score.PartialScore.ToString("00000");
            }
        }
    }
}